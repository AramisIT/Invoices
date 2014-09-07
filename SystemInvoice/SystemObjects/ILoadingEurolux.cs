using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.ApprovalsModification;
using SystemInvoice.Excel;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.DataBase;
using Aramis.DatabaseConnector;
using System;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using Aramis.SystemConfigurations;
using Aramis.UI.WinFormsDevXpress;
using AramisWpfComponents.Excel;
using Excel;
using NPOI.HSSF.Record.Formula.Functions;
using NPOI.SS.UserModel;
using TableViewInterfaces;
using Cell = AramisWpfComponents.Excel.Cell;
using Row = AramisWpfComponents.Excel.Row;

namespace SystemInvoice.SystemObjects
    {
    public enum ElectroluxLoadingTypes
        {
        [DataField(Description = "Новые модели")]
        Nomenclature,

        [DataField(Description = "Разрешительные документы")]
        Approvals,

        [DataField(Description = "База моделей и разрешительных документов")]
        NomenclatureDatabase
        }

    public interface ILoadingEurolux : IAramisModel
        {
        ElectroluxLoadingTypes LoadingType { get; set; }

        [DataField(Description = "Контрагент", ReadOnly = true)]
        IContractor Contractor { get; set; }

        [DataField(Size = 1000)]
        string FindArticleAndModelRegEx { get; set; }

        Table<ILoadingEuroluxRow> Rows { get; }

        Table<ILoadingEuroluxWareFile> Files { get; }
        }

    public interface ILoadingEuroluxRow : ITableRow
        {
        [DataField(Description = "Артикул", Size = 100)]
        string Article { get; set; }

        [DataField(Description = "Модель", Size = Nomenclature.MODEL_SIZE)]
        string Model { get; set; }

        Ref<CustomsCode> CodeInternal { get; set; }

        Ref<Country> Country { get; set; }

        Ref<UnitOfMeasure> MeasureUnit { get; set; }

        Ref<ITradeMark> TradeMark { get; set; }

        Ref<IManufacturer> Producer { get; set; }

        string NameDecl { get; set; }

        string NameInvoice { get; set; }

        double Price { get; set; }

        double NetOfOne { get; set; }

        double GrossOfOne { get; set; }
        }

    public interface ILoadingEuroluxWareFile : ITableRow
        {
        [DataField(Size = 1000, ShowInForm = false)]
        string FullFileName { get; set; }

        [DataField(Description = "Файл", Size = 1000, ReadOnly = true)]
        string ShortFileName { get; set; }

        [DataField(Description = "Папка", Size = 1000, ReadOnly = true)]
        string DirName { get; set; }

        [DataField(Description = "Ошибка", Size = 1000, ReadOnly = true)]
        string ErrorDescription { get; set; }

        [DataField(Description = "Значение Excel", Size = 1000, ReadOnly = true)]
        string ErrorHelpData { get; set; }
        }

    public class LoadingEuroluxBehaviour : Behaviour<ILoadingEurolux>
        {
        class ApprovalDocumentInfo
            {
            public long DocumentType { get; set; }
            public DateTime StartDate { get; set; }
            public string DocumentNumber { get; set; }

            public Approvals ApprovalsDocument { get; set; }

            internal void AddWare(long wareId)
                {
                var dataRow = ApprovalsDocument.Nomenclatures.GetNewRow(ApprovalsDocument);
                dataRow[ApprovalsDocument.ItemNomenclature] = wareId;
                dataRow.AddRowToTable(ApprovalsDocument);
                }
            }

        private StringCacheDictionary customsCodes;
        private StringCacheDictionary approvalsTypes;
        private StringCacheDictionary countries;
        private StringCacheDictionary unitMeasures;

        private Dictionary<string, IManufacturer> producers;
        private Dictionary<string, ITradeMark> tradeMarks;
        private Dictionary<DateTime, Dictionary<string, Approvals>> approvalsCache;

        public LoadingEuroluxBehaviour(ILoadingEurolux item)
            : base(item)
            {
            O.Contractor = CatalogTable.FindByDescription(typeof(IContractor).GetTableName(), "Electrolux") as IContractor;
            }

        private bool loaderIsInitiated;

        private void initLoader()
            {
            if (loaderIsInitiated) return;

            customsCodes = new CatalogCacheCreator<CustomsCode>().GetDescriptionIdCache();
            countries = new CatalogCacheCreator<Country>().GetDescriptionIdCache(descriptionFieldName: "InternationalCode");
            unitMeasures = new CatalogCacheCreator<UnitOfMeasure>().GetDescriptionIdCache(descriptionFieldName: "ShortName");
            approvalsTypes =
                new CatalogCacheCreator<DocumentType>().GetDescriptionIdCache(descriptionFieldName: "QualifierCodeName");

            producers = new CatalogCacheCreator<IManufacturer>().GetDescriptionItemCache(new { Contractor = O.Contractor });
            tradeMarks = new CatalogCacheCreator<ITradeMark>().GetDescriptionItemCache(new { Contractor = O.Contractor });

            approvalsCache = getApprovalCache();

            loaderIsInitiated = true;
            }

        private Dictionary<DateTime, Dictionary<string, Approvals>> getApprovalCache()
            {
            var q = DB.NewQuery(@"select rtrim(cap.documentNumber) number, cast(dateFrom as date) Date, cap.Id
	from Approvals cap
	where cap.MarkForDeleting = 0 and cap.Contractor = @Contractor
	order by dateFrom");
            q.AddInputParameter("Contractor", O.Contractor.Id);
            using (var qResult = q.Select())
                {
                var result = new Dictionary<DateTime, Dictionary<string, Approvals>>();
                while (qResult.Next())
                    {
                    var number = qResult[0] as string;
                    var date = (DateTime)qResult[1];

                    Dictionary<string, Approvals> subDict;
                    if (!result.TryGetValue(date, out subDict))
                        {
                        subDict = new Dictionary<string, Approvals>(new IgnoreCaseStringEqualityComparer());
                        result.Add(date, subDict);
                        }

                    var id = qResult[2].ToInt64();
                    if (!subDict.ContainsKey(number))
                        {
                        subDict.Add(number, A.New<Approvals>(id));
                        }
                    }

                return result;
                }
            }

        class ExcelRow
            {
            public DateTime Date;
            public Row Row;

            public string Article;
            public string Model;

            public int RowNumber;
            public Sheet Sheet;
            }

        internal void LoadWaresDatabaseFromExcel(string fileName, Action<double> notifyProgress)
            {
            initLoader();

            warnings = new StringBuilder();
            warnings.AppendLine();

            O.Rows.Clear();

            ExcelXlsWorkbook book = null;
            if (!ExcelHelper.tryLoad(fileName, out book))
                {
                "Не удалось открыть Excel файл!".NotifyToUser();
                return;
                }

            const int modelIndex = 4;
            const int articleIndex = 20;
            const int dateIndex = 2;

            var waresDictionary = new Dictionary<string, ExcelRow>();
            var total = 0;
            for (int sheetIndex = 0; sheetIndex < book.SheetCount; sheetIndex++)
                {
                total += book[sheetIndex].RowCount;
                }

            var current = 0;
            for (int sheetIndex = 0; sheetIndex < book.SheetCount; sheetIndex++)
                {
                Worksheet sheet = book[sheetIndex];
                for (int rowIndex = 1; rowIndex < sheet.RowCount; rowIndex++)
                    {
                    current++;
                    Row row = sheet[rowIndex];
                    var model = row[modelIndex].Value.ToString().Trim();
                    var article = row[articleIndex].Value.ToString().Trim();

                    if (string.IsNullOrEmpty(article)) continue;

                    DateTime date;
                    if (!getDate(row[dateIndex], rowIndex, sheet.Sheet, out date))
                        {
                        return;
                        }

                    var cache = string.Format("{0}$#^#${1}", model, article);
                    ExcelRow savedRow;
                    if (waresDictionary.TryGetValue(cache, out savedRow))
                        {
                        if (savedRow.Date >= date) continue;

                        savedRow.Date = date;
                        savedRow.Row = row;
                        }
                    else
                        {
                        waresDictionary.Add(cache, new ExcelRow() { Date = date, Row = row, Article = article, Model = model, RowNumber = rowIndex + 1, Sheet = sheet.Sheet });
                        }

                    notifyProgress((double)current / total);
                    }
                }

            book.Dispose();
            //  await Task.Factory.StartNew(() => 
            loadData(waresDictionary.Values.ToList(), notifyProgress);//);
            }

        private void loadData(List<ExcelRow> list, Action<double> notifyProgress)
            {
            const int countryIndex = 15;
            const int producerIndex = 17;
            const int customsCodeIndex = 3;
            const int unitMeasureIndex = 18;
            const int tradeMarkIndex = 16;

            const int nameDeclIndex = 6;
            const int nameInvoiceIndex = 5;

            const int priceIndex = 8;

            const int countIndex = 7;
            const int netIndex = 11;
            const int grossIndex = 12;

            newCatalogItems.Clear();
            newDocumentItems.Clear();
            approvalsDocuments.Clear();


            for (int i = 0; i < list.Count; i++)
                {
                var row = list[i];

                var docRow = O.Rows.Add();
                docRow.Model = row.Model;
                docRow.Article = row.Article;

                docRow.NameDecl = row.Row[nameDeclIndex].Value.ToString().Trim();
                docRow.NameInvoice = row.Row[nameInvoiceIndex].Value.ToString().Trim();

                if (string.IsNullOrEmpty(docRow.NameDecl))
                    {
                    addWarning("Не заполненно наименование декларации!", row);
                    docRow.RemoveFromTable();
                    continue;
                    }
                else if (string.IsNullOrEmpty(docRow.NameInvoice))
                    {
                    addWarning("Не заполненно наименование инвойс!", row);
                    docRow.RemoveFromTable();
                    continue;
                    }

                docRow.CodeInternal.Id = getId(row.Row[customsCodeIndex].Value, customsCodes, "УКТЗЕД", false);

                if (docRow.CodeInternal.Id == 0)
                    {
                    addWarning("Не найден УКТЗЕД!", row);
                    docRow.RemoveFromTable();
                    continue;
                    }

                try
                    {
                    docRow.Price = row.Row[priceIndex].Value.ToString().ConvertToDouble();
                    docRow.NetOfOne = row.Row[netIndex].Value.ToString().ConvertToDouble();
                    docRow.GrossOfOne = row.Row[grossIndex].Value.ToString().ConvertToDouble();
                    var count = row.Row[countIndex].Value.ToString().ConvertToDouble();

                    if (count > 0.0)
                        {
                        docRow.NetOfOne /= count;
                        docRow.GrossOfOne /= count;
                        }
                    else
                        {
                        docRow.NetOfOne = 0.0;
                        docRow.GrossOfOne = 0.0;
                        }

                    docRow.Country.Id = getId(row.Row[countryIndex].Value, countries, "Страна");
                    docRow.MeasureUnit.Id = getId(row.Row[unitMeasureIndex].Value, unitMeasures, "Един. изм.");

                    var producer = row.Row[producerIndex].Value.ToString().TrimEnd();
                    if (string.IsNullOrEmpty(producer))
                        {
                        addWarning("Не заполненно производитель!", row);
                        docRow.RemoveFromTable();
                        continue;
                        }

                    docRow.Producer.Item = getItem<IManufacturer>(producer, producers);
                    docRow.TradeMark.Item = getItem<ITradeMark>(row.Row[tradeMarkIndex].Value.ToString().TrimEnd(), tradeMarks);
                    }
                catch (Exception exp)
                    {
                    string.Format("Ошибка в строке № {0}, на странице \"{1}\".\r\n{2}", row.RowNumber, row.Sheet.SheetName, exp.Message).WarningBox();
                    return;
                    }

                const int approvalsIndexOffset = 21;

                const int approvalNumberIndex = 1;
                const int approvalDateIndex = 2;

                var approvalDocuments = new List<ApprovalDocumentInfo>();
                var documentIndex = 1;
                var currentColumnOffset = approvalsIndexOffset;
                while (true)
                    {
                    var docType = getId(row.Row[currentColumnOffset].Value, approvalsTypes, "Типы док-в", false);
                    if (docType == 0)
                        {
                        break;
                        }

                    var documentNumber = row.Row[currentColumnOffset + approvalNumberIndex].Value.ToString();
                    DateTime date;
                    if (!getDate(row.Row[currentColumnOffset + approvalDateIndex], row.RowNumber - 1, row.Sheet, out date))
                        {
                        date = DateTime.MinValue;
                        }

                    var newDoc = new ApprovalDocumentInfo() { DocumentNumber = documentNumber.Trim(), StartDate = date, DocumentType = docType };
                    if (!newDoc.StartDate.Equals(DateTime.MinValue) || !string.IsNullOrEmpty(newDoc.DocumentNumber))
                        {
                        newDoc.ApprovalsDocument = getApprovalsDocument(newDoc);
                        approvalDocuments.Add(newDoc);
                        }

                    currentColumnOffset += 3;
                    docType = getId(row.Row[currentColumnOffset].Value, approvalsTypes, "Типы док-в", false);
                    if (docType == 0)
                        {
                        currentColumnOffset++;
                        docType = getId(row.Row[currentColumnOffset].Value, approvalsTypes, "Типы док-в", false);
                        if (docType > 0)
                            {
                            Trace.WriteLine("Long string");
                            }
                        else
                            {
                            break;
                            }
                        }

                    documentIndex++;
                    if (documentIndex == 5)
                        {
                        break;
                        }
                    }

                approvalsDocuments.Add(docRow.LineNumber, approvalDocuments);

                notifyProgress((double)i / (double)list.Count);
                }

            "Загрузка завершена!".NotifyToUser();

            if (newCatalogItems.Count > 0)
                {
                var newItemsMessage = new StringBuilder("Создать следующие элементы?");
                newItemsMessage.AppendLine();
                foreach (var item in newCatalogItems)
                    {
                    newItemsMessage.AppendLine(string.Format("{0} ({1})", item.Description, item.ObjInfo.Description));
                    }

                if (!newItemsMessage.ToString().Ask()) return;
                }

            string errorDescription;
            if (!writeToDatabase(notifyProgress, out errorDescription))
                {
                errorDescription.WarningBox();
                }
            }

        private void addWarning(string message, ExcelRow row)
            {
            warnings.AppendLine(string.Format("{0} Страница - {1} № стр. - {2}", message.PadRight(40), row.Sheet.SheetName.PadRight(25), row.RowNumber));
            }

        private Approvals getApprovalsDocument(ApprovalDocumentInfo docInfo)
            {
            var dateTime = docInfo.StartDate;
            var docNumber = docInfo.DocumentNumber;

            Dictionary<string, Approvals> subDict;
            if (!approvalsCache.TryGetValue(dateTime, out subDict))
                {
                subDict = new Dictionary<string, Approvals>(new IgnoreCaseStringEqualityComparer());
                approvalsCache.Add(dateTime, subDict);
                }

            Approvals doc;
            if (!subDict.TryGetValue(docNumber, out doc))
                {
                doc = A.New<Approvals>();
                doc.DateFrom = dateTime.Equals(DateTime.MinValue) ? DateTime.Now : dateTime;
                doc.DateTo = doc.DateFrom.AddYears(2);
                doc.Date = DateTime.Now;
                doc.DocumentNumber = docNumber;
                doc.Contractor = O.Contractor;
                doc.SetRef("DocumentType", docInfo.DocumentType);
                subDict.Add(docNumber, doc);
                newDocumentItems.Add(doc);
                }

            return doc;
            }

        private bool writeToDatabase(Action<double> notifyProgress, out string errorDescription)
            {
            foreach (var item in newCatalogItems)
                {
                var writtenResult = item.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    errorDescription = "Не удалось записать элемент: " + item.Description;
                    return false;
                    }
                }

            foreach (var item in newDocumentItems)
                {
                var writtenResult = item.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    errorDescription = "Не удалось записать элемент: " + item;
                    return false;
                    }
                }

            var rowsCount = O.Rows.RowsCount;
            for (int itemIndex = 0; itemIndex < rowsCount; itemIndex++)
                {
                var row = O.Rows[itemIndex];

                var ware = getWare(row.Article, row.Model);

                ware.SetRef("Country", row.Country.Id);
                ware.SetRef("UnitOfMeasure", row.MeasureUnit.Id);
                ware.SetRef("Manufacturer", row.Producer.Id);
                ware.SetRef("TradeMark", row.TradeMark.Id);
                ware.SetRef("CustomsCodeInternal", row.CodeInternal.Id);
                ware.Contractor = O.Contractor;

                ware.Price = row.Price;
                ware.NetWeightFrom = row.NetOfOne;
                ware.NetWeightTo = row.NetOfOne;
                ware.GrossWeightFrom = row.GrossOfOne;
                ware.GrossWeightTo = row.GrossOfOne;

                ware.NameDecl = row.NameDecl;
                ware.NameInvoice = row.NameInvoice;

                var newWare = ware.IsNew;

                var writtenResult = ware.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    errorDescription = string.Format("Не удалось записать товар: {0}\r\n{1}", ware.Description, ware.LastWrittingError);
                    return false;
                    }

                if (approvalsDocuments.Count > 0)
                    {
                    foreach (var approvalDoc in approvalsDocuments[row.LineNumber])
                        {
                        if (newWare)
                            {
                            approvalDoc.AddWare(ware.Id);
                            }
                        else
                            {
                            var docContainsWare = false;
                            foreach (DataRow docRow in approvalDoc.ApprovalsDocument.Nomenclatures.Rows)
                                {
                                if (ware.Id.Equals(docRow[approvalDoc.ApprovalsDocument.ItemNomenclature]))
                                    {
                                    docContainsWare = true;
                                    break;
                                    }
                                }
                            if (!docContainsWare)
                                {
                                approvalDoc.AddWare(ware.Id);
                                }
                            }

                        writtenResult = approvalDoc.ApprovalsDocument.Write();
                        if (writtenResult != WritingResult.Success)
                            {
                            errorDescription = "Не удалось записать разреш. документ: " + approvalDoc.ToString();
                            return false;
                            }
                        }
                    }
                notifyProgress((double)itemIndex / (double)rowsCount);
                }

            if (warnings != null && warnings.Length > 0)
                {
                "Данные загружены в базу!".NotifyToUser();

                Clipboard.SetText(warnings.ToString());
                "Обнаруженные ошибки в буфере обмена!".NotifyToUser(MessagesToUserTypes.Error);
                }

            errorDescription = string.Empty;
            return true;
            }

        private Nomenclature getWare(string article, string model)
            {
            var q = DB.NewQuery(@"select top 1 Id
	from Nomenclature
	where Contractor = @Contractor and Article = @Article and [Model] = @Model");
            q.AddInputParameter("Contractor", O.Contractor.Id);
            q.AddInputParameter("Article", article);
            q.AddInputParameter("Model", model);

            var wareId = q.SelectInt64();
            var result = A.New<Nomenclature>(wareId);
            result.Article = article;
            result.Model = model;
            return result;
            }

        private List<ICatalog> newCatalogItems = new List<ICatalog>();
        private List<IDocument> newDocumentItems = new List<IDocument>();
        private Dictionary<long, List<ApprovalDocumentInfo>> approvalsDocuments = new Dictionary<long, List<ApprovalDocumentInfo>>();
        private StringBuilder warnings;

        private T getItem<T>(string strValue, Dictionary<string, T> cache) where T : ICatalog
            {
            if (!string.IsNullOrEmpty(strValue))
                {
                T item;
                if (cache.TryGetValue(strValue, out item))
                    {
                    return item;
                    }
                else
                    {
                    item = A.New<T>();
                    item.SetRef("Contractor", O.Contractor.Id);
                    item.Description = strValue;
                    newCatalogItems.Add(item);
                    cache.Add(strValue, item);
                    return item;
                    }
                }

            throw new Exception(string.Format("Не удалось получить поле {0}",
                    SystemConfiguration.DBConfigurationTree[typeof(T).GetTableName()].Description));
            }

        private long getId(object value, StringCacheDictionary cache, string fieldName, bool throwException = true)
            {
            var strValue = value.ToString();
            if (string.IsNullOrEmpty(strValue))
                {
                return 0;
                }
            long id;
            if (cache.TryGetValue(strValue, out id))
                {
                return id;
                }
            if (throwException)
                {
                throw new Exception(string.Format("Не удалось получить поле {0}", fieldName));
                }
            return 0;
            }

        private bool getDate(Cell cell, int rowIndex, Sheet sheet, out DateTime date)
            {
            try
                {
                date = cell.CELL.DateCellValue.Date;
                }
            catch
                {
                try
                    {
                    var strValue = cell.Value.ToString();
                    if (strValue.Contains('/'))
                        {
                        var parts = strValue.Split('/');
                        date = new DateTime(2000 + parts[2].ToInt32(), parts[0].ToInt32(), parts[1].ToInt32());
                        }
                    else
                        {
                        date = strValue.ConvertToDateTime("dd.MM.yyyy").Date;
                        }
                    }
                catch
                    {
                    string.Format("Ошибка получения даты в строке № {1}; страница {0}",
                        sheet.SheetName, (rowIndex + 1)).WarningBox();
                    date = DateTime.MinValue;
                    return false;
                    }
                }

            return true;
            }

        private Regex findArticleAndModelRegEx;

        internal void LoadNewWares(List<string> files, Action<double> notifyProgress)
            {
            initLoader();

            findArticleAndModelRegEx = new Regex(O.FindArticleAndModelRegEx);

            O.Files.Clear();
            notifyProgress(0.0);

            for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
                {
                var fileName = files[fileIndex];
                string errorDescription;
                string errorHelpData;
                if (!tryLoadNewWaresFromExcelFile(fileName, out errorDescription, out errorHelpData))
                    {
                    var errorFileRow = O.Files.Add();
                    errorFileRow.ErrorDescription = errorDescription;
                    errorFileRow.ErrorHelpData = errorHelpData;
                    errorFileRow.FullFileName = fileName;
                    errorFileRow.ShortFileName = Path.GetFileName(fileName);
                    errorFileRow.DirName = Path.GetDirectoryName(fileName);
                    }

                notifyProgress((1.0 + fileIndex) / files.Count);
                }
            }

        class InternalCodesInfo
            {
            public long InternalCodeId { get; set; }

            public string Model { get; set; }

            public string Article { get; set; }

            public InternalCodesInfo Clone()
                {
                return new InternalCodesInfo() { InternalCodeId = InternalCodeId, Model = Model, Article = Article };
                }
            }

        private bool tryLoadNewWaresFromExcelFile(string fileName, out string errorDescription, out string errorHelpData)
            {
            errorHelpData = null;

            DataTable firstSheet = null;
            DataTable secondSheet = null;
            try
                {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                    DataSet result = excelReader.AsDataSet();
                    firstSheet = result.Tables[0];
                    secondSheet = result.Tables[1];
                    }
                }
            catch
                {
                errorDescription = "Не удалось открыть Excel файл!";
                return false;
                }

            Dictionary<string, InternalCodesInfo> internalCodesDictionary;
            if (!loadInternalCodes(firstSheet, out internalCodesDictionary, out errorDescription, out errorHelpData)) return false;

            return loadWares(secondSheet, internalCodesDictionary, out errorDescription, out errorHelpData);
            }

        private bool loadWares(DataTable table, Dictionary<string, InternalCodesInfo> internalCodesDictionary,
            out string errorDescription, out string errorHelpData)
            {
            newCatalogItems.Clear();
            errorHelpData = null;
            O.Rows.Clear();

            for (int rowIndex = 3; rowIndex < table.Rows.Count; rowIndex++)
                {
                var row = table.Rows[rowIndex];

                const int COUNTRY_INDEX = 5;
                const int TRADE_MARK_INDEX = 7;

                var countryId = getId(row[COUNTRY_INDEX], countries, "страна", false);
                var tradeMarkName = row[TRADE_MARK_INDEX].ToString().Trim();

                if (countryId == 0)
                    {
                    if (string.IsNullOrEmpty(tradeMarkName))
                        {
                        // empty row, just skipping
                        continue;
                        }
                    else
                        {
                        errorDescription = string.Format("Не найдена страна! Стр. № {0}", (rowIndex + 1));
                        errorHelpData = row[COUNTRY_INDEX].ToString();
                        return false;
                        }
                    }

                if (string.IsNullOrEmpty(tradeMarkName))
                    {
                    errorDescription = string.Format("Пустая торговая марка. Стр. № {0}", (rowIndex + 1));
                    return false;
                    }

                const int PRODUCER_INDEX = 6;
                var producerName = row[PRODUCER_INDEX].ToString().Trim();

                if (string.IsNullOrEmpty(producerName))
                    {
                    errorDescription = string.Format("Пустой производитель. Стр. № {0}", (rowIndex + 1));
                    return false;
                    }

                const int INVOICE_NAME_INDEX = 3;
                var invoiceName = row[INVOICE_NAME_INDEX].ToString();

                const int DECLARATION_NAME_INDEX = 4;
                var declarationName = row[DECLARATION_NAME_INDEX].ToString();

                if (string.IsNullOrEmpty(invoiceName)
                    && string.IsNullOrEmpty(declarationName))
                    {
                    errorDescription = string.Format("Не указано ни название инвойса ни название декларации. Стр. № {0}", (rowIndex + 1));
                    return false;
                    }

                if (string.IsNullOrEmpty(invoiceName))
                    {
                    invoiceName = declarationName;
                    }
                else if (string.IsNullOrEmpty(declarationName))
                    {
                    declarationName = invoiceName;
                    }

                const int PRICE_INDEX = 10;
                const int GROSS_INDEX = 8;
                const int NET_INDEX = 9;
                const int COUNT_INDEX = 13;
                var docRow = O.Rows.Add();

                docRow.NameInvoice = invoiceName;
                docRow.NameDecl = invoiceName;

                try
                    {
                    docRow.Price = row[PRICE_INDEX].ToString().ConvertToDouble();
                    docRow.NetOfOne = row[NET_INDEX].ToString().ConvertToDouble();
                    docRow.GrossOfOne = row[GROSS_INDEX].ToString().ConvertToDouble();
                    var count = row[COUNT_INDEX].ToString().ConvertToDouble();

                    if (count > 0.0)
                        {
                        docRow.NetOfOne /= count;
                        docRow.GrossOfOne /= count;
                        docRow.Price /= count;
                        }
                    else
                        {
                        docRow.NetOfOne = 0.0;
                        docRow.GrossOfOne = 0.0;
                        docRow.Price = 0.0;
                        }

                    docRow.Country.Id = countryId;
                    docRow.MeasureUnit.Id = getDefaultMeasureUnitId();

                    docRow.Producer.Item = getItem<IManufacturer>(producerName, producers);
                    docRow.TradeMark.Item = getItem<ITradeMark>(tradeMarkName, tradeMarks);
                    }
                catch (Exception exp)
                    {
                    errorDescription = string.Format("Ошибка в строке № {0}, на странице спецификация.\r\n{1}", (rowIndex + 1), exp.Message);
                    return false;
                    }

                const int ID_INDEX = 1;
                var idValue = row[ID_INDEX].ToString();

                InternalCodesInfo internalCodesInfo;
                if (!internalCodesDictionary.TryGetValue(idValue, out internalCodesInfo))
                    {
                    errorDescription = string.Format("Не найден связывающий артикул. Стр. № {0}", (rowIndex + 1));
                    errorHelpData = idValue;
                    return false;
                    }

                docRow.CodeInternal.Id = internalCodesInfo.InternalCodeId;
                docRow.Article = internalCodesInfo.Article;
                docRow.Model = internalCodesInfo.Model;
                }

            errorDescription = null;
            var result = true ||
                writeToDatabase((progress) => { }, out errorDescription);
            return result;
            }

        private long defaultMeasureUnitId;
        private long getDefaultMeasureUnitId()
            {
            if (defaultMeasureUnitId == 0)
                {
                defaultMeasureUnitId = getId("шт", unitMeasures, "Един. изм.");
                }
            return defaultMeasureUnitId;
            }

        private bool loadInternalCodes(DataTable internalCodesTable, out Dictionary<string, InternalCodesInfo> internalCodesDictionary,
            out string errorDescription, out string errorHelpData)
            {
            errorHelpData = null;
            internalCodesDictionary = new Dictionary<string, InternalCodesInfo>();

            var rowsCount = internalCodesTable.Rows.Count;
            for (int rowIndex = 3; rowIndex < rowsCount; rowIndex++)
                {
                var row = internalCodesTable.Rows[rowIndex];
                var inputString = row[3].ToString();

                const int customsCodeIndex = 1;
                var codeId = getId(row[customsCodeIndex], customsCodes, "УКТЗЕД", false);

                if (codeId == 0)
                    {
                    var internalCodeValue = row[customsCodeIndex].ToString();
                    if (string.IsNullOrEmpty(internalCodeValue))
                        {
                        continue;
                        }
                    errorDescription = string.Format("Стр. № {0}. Не найден УКТЗЕД", (rowIndex + 1));
                    errorHelpData = row[customsCodeIndex].ToString();
                    return false;
                    }

                var matches = findArticleAndModelRegEx.Matches(inputString);
                foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                    var info = new InternalCodesInfo()
                    {
                        Model = match.Groups["Model"].Value,
                        Article = match.Groups["Article"].Value.Replace(" ", ""),
                        InternalCodeId = codeId
                    };

                    if (string.IsNullOrEmpty(info.Article))
                        {
                        errorDescription = string.Format("Стр. № {0}. Не найден артикул", (rowIndex + 1));
                        return false;
                        }

                    if (string.IsNullOrEmpty(info.Model))
                        {
                        errorDescription = string.Format("Стр. № {0}. Не найдена модель", (rowIndex + 1));
                        return false;
                        }

                    if (internalCodesDictionary.ContainsKey(info.Article))
                        {
                        errorDescription = string.Format("Стр. № {0}. Повтор артикула", (rowIndex + 1));
                        return false;
                        }

                    if (internalCodesDictionary.ContainsKey(info.Model))
                        {
                        errorDescription = string.Format("Стр. № {0}. Повтор модели", (rowIndex + 1));
                        return false;
                        }

                    internalCodesDictionary.Add(info.Article, info);
                    internalCodesDictionary.Add(info.Model, info);
                    }
                }

            errorDescription = null;
            return true;
            }
        }

    }
