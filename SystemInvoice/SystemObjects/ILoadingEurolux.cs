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
using SystemInvoice.Utils;
using SystemInvoice.Utils.Excel;
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

        private StringCacheDictionary nomnclatureCache;

        private StringCacheDictionary customsCodes;
        private StringCacheDictionary approvalsTypes;
        private StringCacheDictionary countries;
        private StringCacheDictionary unitMeasures;

        private Dictionary<string, IManufacturer> producers;
        private Dictionary<string, ITradeMark> tradeMarks;

        private Dictionary<string, Approvals> certificatesCache;

        private Dictionary<DateTime, Dictionary<string, Approvals>> approvalsCache;

        public LoadingEuroluxBehaviour(ILoadingEurolux item)
            : base(item)
            {
            O.Contractor = ELECTROLUX_CONTRACTOR;
            }

        private static IContractor _ELECTROLUX_CONTRACTOR;
        public static IContractor ELECTROLUX_CONTRACTOR
            {
            get
                {
                return _ELECTROLUX_CONTRACTOR ??
                    (_ELECTROLUX_CONTRACTOR = CatalogTable.FindByDescription(typeof(IContractor).GetTableName(), "Electrolux") as IContractor);
                }
            }

        private bool loaderIsInitiated;

        private void initLoader()
            {
            if (loaderIsInitiated) return;

            if (O.LoadingType != ElectroluxLoadingTypes.Approvals)
                {
                customsCodes = new CatalogCacheCreator<CustomsCode>().GetDescriptionIdCache();
                countries = new CatalogCacheCreator<Country>().GetDescriptionIdCache(descriptionFieldName: "InternationalCode");
                unitMeasures = new CatalogCacheCreator<UnitOfMeasure>().GetDescriptionIdCache(descriptionFieldName: "ShortName");
                approvalsTypes = new CatalogCacheCreator<IDocumentType>().GetDescriptionIdCache(descriptionFieldName: "QualifierCodeName");

                producers = new CatalogCacheCreator<IManufacturer>().GetDescriptionItemCache(new { Contractor = O.Contractor });
                tradeMarks = new CatalogCacheCreator<ITradeMark>().GetDescriptionItemCache(new { Contractor = O.Contractor });

                if (O.LoadingType == ElectroluxLoadingTypes.NomenclatureDatabase)
                    {
                    approvalsCache = getApprovalCache();
                    }
                else if (O.LoadingType == ElectroluxLoadingTypes.Nomenclature)
                    {
                    if (string.IsNullOrEmpty(O.FindArticleAndModelRegEx))
                        {
                        O.FindArticleAndModelRegEx =
                            @"(?i)(?x)       м\s*о\s*д\s*е\s*л\s*ь[\s-]+  (?<Model>    [^\s,]+)               (   .*?     (   а\s*р\s*т \s* \.  \s*  |  артикул  \s*  )            (?<Article>       [^-]*   )  )?";
                        }
                    }
                }
            else
                {
                var declarationIdList = DB.NewQuery("select cap.Id from DocumentType cap where cap.QualifierCodeName = 5112 and cap.MarkForDeleting = 0").SelectToList<long>();
                declarationType = A.New<IDocumentType>(declarationIdList.Count == 1 ? declarationIdList.First() : 0);

                var certificateTypeIdList = DB.NewQuery("select cap.Id from DocumentType cap where cap.QualifierCodeName = 5111 and cap.MarkForDeleting = 0").SelectToList<long>();
                certificateType = DocumentTypeHelper.GetCertificateType();

                nomnclatureCache = new CatalogCacheCreator<Nomenclature>().GetDescriptionIdCache(new { Contractor = O.Contractor }, "Model");
                certificatesCache = new CatalogCacheCreator<Approvals>().GetDescriptionItemCache(new { Contractor = O.Contractor, DocumentType = certificateType }, "DocumentNumber");
                declarationsCache = new CatalogCacheCreator<Approvals>().GetDescriptionItemCacheIncludeRepeatedItems(new { Contractor = O.Contractor, DocumentType = declarationType }, "DocumentNumber");
                }

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
            public IExcelRow Row;

            public string Article;
            public string Model;

            public int RowNumber;
            public IExcelSheet Sheet;
            }

        internal void LoadWaresDatabaseFromExcel(string fileName, Action<double> notifyProgress)
            {
            initLoader();

            warnings = new StringBuilder();
            warnings.AppendLine();

            O.Rows.Clear();

            using (IExcelReader excelReader = new NativeExcelReader())
                {
                if (!excelReader.OpenFile(fileName))
                    {
                    "Не удалось открыть Excel файл!".NotifyToUser();
                    return;
                    }

                const int modelIndex = 4;
                const int articleIndex = 20;
                const int dateIndex = 2;

                var waresDictionary = new Dictionary<string, ExcelRow>();
                var total = 0;
                for (int sheetIndex = 0; sheetIndex < excelReader.SheetsCount; sheetIndex++)
                    {
                    total += excelReader[sheetIndex].RowsCount;
                    }

                var current = 0;
                int rowsSkipped = 0;
                for (int sheetIndex = 0; sheetIndex < excelReader.SheetsCount; sheetIndex++)
                    {
                    var sheet = excelReader[sheetIndex];
                    for (int rowIndex = 1; rowIndex < sheet.RowsCount; rowIndex++)
                        {
                        current++;
                        var row = sheet[rowIndex];
                        var model = row.GetString(modelIndex);
                        var article = row.GetString(articleIndex);

                        if (string.IsNullOrEmpty(article))
                            {
                            rowsSkipped++;
                            if (rowsSkipped > 10)
                                {
                                break;
                                }
                            continue;
                            }

                        var date = row.GetDate(dateIndex);
                        if (date.IsEmpty())
                            {
                            string.Format("Ошибка получения даты в строке № {1}; страница {0}", sheet.Name, (rowIndex + 1)).WarningBox();
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
                            waresDictionary.Add(cache,
                                new ExcelRow()
                                    {
                                        Date = date,
                                        Row = row,
                                        Article = article,
                                        Model = model,
                                        RowNumber = rowIndex + 1,
                                        Sheet = sheet
                                    });
                            }

                        notifyProgress((double)current / total);
                        }
                    }

                loadData(waresDictionary.Values.ToList(), notifyProgress);
                }
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

                docRow.NameDecl = row.Row.GetString(nameDeclIndex);
                docRow.NameInvoice = row.Row.GetString(nameInvoiceIndex);

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

                docRow.CodeInternal.Id = getId(row.Row.GetLong(customsCodeIndex), customsCodes, "УКТЗЕД", false);

                if (docRow.CodeInternal.Id == 0)
                    {
                    addWarning("Не найден УКТЗЕД!", row);
                    docRow.RemoveFromTable();
                    continue;
                    }

                docRow.Price = row.Row.GetDouble(priceIndex);
                docRow.NetOfOne = row.Row.GetDouble(netIndex);
                docRow.GrossOfOne = row.Row.GetDouble(grossIndex);
                var count = row.Row.GetDouble(countIndex);
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

                try
                    {
                    docRow.Country.Id = getId(row.Row.GetLong(countryIndex), countries, "Страна");
                    docRow.MeasureUnit.Id = getId(row.Row.GetLong(unitMeasureIndex), unitMeasures, "Един. изм.");

                    var producer = row.Row.GetString(producerIndex);
                    if (string.IsNullOrEmpty(producer))
                        {
                        addWarning("Не заполненно производитель!", row);
                        docRow.RemoveFromTable();
                        continue;
                        }

                    docRow.Producer.Item = getItem<IManufacturer>(producer, producers);
                    docRow.TradeMark.Item = getItem<ITradeMark>(row.Row.GetString(tradeMarkIndex), tradeMarks);
                    }
                catch (Exception exp)
                    {
                    string.Format("Ошибка в строке № {0}, на странице \"{1}\".\r\n{2}", row.RowNumber, row.Sheet.Name, exp.Message).WarningBox();
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
                    var docType = getId(row.Row.GetLong(currentColumnOffset), approvalsTypes, "Типы док-в", false);
                    if (docType == 0)
                        {
                        break;
                        }

                    var documentNumber = row.Row.GetString(currentColumnOffset + approvalNumberIndex);
                    var date = row.Row.GetDate(currentColumnOffset + approvalDateIndex);
                    if (date.IsEmpty())
                        {
                        string.Format("Ошибка получения даты в строке № {1}; страница {0}", row.Sheet.Name, row.RowNumber).WarningBox();
                        return;
                        }

                    var newDoc = new ApprovalDocumentInfo() { DocumentNumber = documentNumber.Trim(), StartDate = date, DocumentType = docType };
                    if (!newDoc.StartDate.Equals(DateTime.MinValue) || !string.IsNullOrEmpty(newDoc.DocumentNumber))
                        {
                        newDoc.ApprovalsDocument = getApprovalsDocument(newDoc);
                        approvalDocuments.Add(newDoc);
                        }

                    currentColumnOffset += 3;
                    docType = getId(row.Row.GetLong(currentColumnOffset), approvalsTypes, "Типы док-в", false);
                    if (docType == 0)
                        {
                        currentColumnOffset++;
                        docType = getId(row.Row.GetLong(currentColumnOffset), approvalsTypes, "Типы док-в", false);
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
            if (!writeToDatabase(notifyProgress, false, out errorDescription))
                {
                errorDescription.WarningBox();
                }
            }

        private void addWarning(string message, ExcelRow row)
            {
            warnings.AppendLine(string.Format("{0} Страница - {1} № стр. - {2}", message.PadRight(40), row.Sheet.Name.PadRight(25), row.RowNumber));
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

        private bool writeToDatabase(Action<double> notifyProgress, bool wareFromCatalog, out string errorDescription)
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

                if (wareFromCatalog)
                    {
                    if (!ware.IsNew) continue;

                    ware.WareFromCatalog = true;
                    }

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

        private T getItem<T>(string strValue, Dictionary<string, T> cache) where T : IDatabaseObject
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
                    if (item is ICatalog)
                        {
                        ((ICatalog)item).Description = strValue;
                        newCatalogItems.Add((ICatalog)item);
                        }
                    else
                        {
                        newDocumentItems.Add((IDocument)item);
                        }
                    cache.Add(strValue, item);
                    return item;
                    }
                }

            throw new Exception(string.Format("Не удалось получить поле {0}",
                    SystemConfiguration.DBConfigurationTree[typeof(T).GetTableName()].Description));
            }

        private long getId(object value, StringCacheDictionary cache, string fieldName, bool throwException = true)
            {
            if (value == null) return 0;

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

        internal void LoadExcelFiles(List<string> files, Action<double> notifyProgress)
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

                if (!tryLoadDataFromExcelFile(fileName, notifyProgress, out errorDescription, out errorHelpData))
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

        private delegate bool TryLoadExcelDataFromExcelFileDelegate(DataSet dataSet, Action<double> notifyProgress, out string errorDescription, out string errorHelpData);

        private bool tryLoadDataFromExcelFile(string fileName, Action<double> notifyProgress, out string errorDescription, out string errorHelpData)
            {
            DataSet fileData = null;

            try
                {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                    var openXmlFileType = fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase);
                    IExcelDataReader excelReader = openXmlFileType ?
                        ExcelReaderFactory.CreateOpenXmlReader(stream) :
                        ExcelReaderFactory.CreateBinaryReader(stream);

                    fileData = excelReader.AsDataSet();
                    }
                }
            catch (Exception e)
                {
                errorDescription = "Не удалось открыть Excel файл!";
                errorHelpData = e.Message;
                return false;
                }

            var fileLoadingHandler = O.LoadingType == ElectroluxLoadingTypes.Nomenclature
              ? new TryLoadExcelDataFromExcelFileDelegate(tryLoadNewWaresFromExcelFile) :
              new TryLoadExcelDataFromExcelFileDelegate(tryLoadApprovalsFromExcelFile);

            return fileLoadingHandler(fileData, notifyProgress, out errorDescription, out errorHelpData);
            }

        private bool tryLoadNewWaresFromExcelFile(DataSet dataSet, Action<double> notifyProgress, out string errorDescription, out string errorHelpData)
            {
            if (dataSet.Tables.Count < 2)
                {
                errorDescription = "В файле должно быть минимум 2 страницы";
                errorHelpData = null;
                return false;
                }

            Dictionary<string, InternalCodesInfo> internalCodesDictionary;
            if (!loadInternalCodes(dataSet.Tables[0], out internalCodesDictionary, out errorDescription, out errorHelpData)) return false;

            return loadWares(dataSet.Tables[1], internalCodesDictionary, out errorDescription, out errorHelpData);
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
                writeToDatabase((progress) => { }, true, out errorDescription);
            return result;
            }

        private long defaultMeasureUnitId;
        private IDocumentType declarationType;
        private IDocumentType certificateType;
        private Dictionary<string, List<Approvals>> declarationsCache;
        private DateTime defaultStartDate = new DateTime(2010, 1, 1);

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
                        errorHelpData = info.Article;
                        errorDescription = string.Format("Стр. № {0}. Не найден артикул", (rowIndex + 1));
                        return false;
                        }

                    if (string.IsNullOrEmpty(info.Model))
                        {
                        errorHelpData = info.Model;
                        errorDescription = string.Format("Стр. № {0}. Не найдена модель", (rowIndex + 1));
                        return false;
                        }

                    InternalCodesInfo existsCode;

                    if (internalCodesDictionary.TryGetValue(info.Article, out existsCode))
                        {
                        if (existsCode.Model.EqualsIgnoreCase(info.Model)) continue;

                        errorHelpData = info.Article;
                        errorDescription = string.Format("Стр. № {0}. Повтор артикула", (rowIndex + 1));
                        return false;
                        }

                    if (internalCodesDictionary.ContainsKey(info.Model))
                        {
                        errorHelpData = info.Model;
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

        private bool tryLoadApprovalsFromExcelFile(DataSet dataSet, Action<double> notifyProgress, out string errorDescription, out string errorHelpData)
            {
            errorHelpData = null;

            if (declarationType.IsNew)
                {
                errorDescription = "Вид декларации не найден (справ. Типы документов), возможно есть несколько элементов с одним и тем же кодом в документе!";
                return false;
                }

            if (certificateType.IsNew)
                {
                errorDescription = "Вид сертификата не найден (справ. Типы документов), возможно есть несколько элементов с одним и тем же кодом в документе!";
                return false;
                }

            newCatalogItems.Clear();
            newDocumentItems.Clear();

            DataTable sheet = dataSet.Tables[0];
            var totalRowsCount = (double)sheet.Rows.Count;
            notifyProgress(0.0);

            for (int rowIndex = 1; rowIndex < sheet.Rows.Count; rowIndex++)
                {
                var row = sheet.Rows[rowIndex];
                var wareId = getId(row[0], nomnclatureCache, "ware", false);
                if (wareId == 0) continue;

                var declaration1Number = row[1].ToString().Trim();
                var declaration2Number = row[2].ToString().Trim();
                var certificateNumber = row[3].ToString().Trim();

                if (string.IsNullOrEmpty(declaration1Number) &&
                    string.IsNullOrEmpty(declaration2Number) &&
                    string.IsNullOrEmpty(certificateNumber))
                    {
                    continue;
                    }

                Approvals certificate;
                if (string.IsNullOrEmpty(certificateNumber))
                    {
                    certificate = A.New<Approvals>();
                    }
                else
                    {
                    certificate = getItem<Approvals>(certificateNumber, certificatesCache);
                    if (certificate.IsNew)
                        {
                        certificate.DocumentNumber = certificateNumber;
                        certificate.SetRef("DocumentType", certificateType.Id);
                        certificate.DocumentCode = certificateType.QualifierCodeName;
                        certificate.DateFrom = defaultStartDate;
                        certificate.DateTo = DateTime.Now.AddYears(2);
                        }
                    else
                        {
                        // need to update
                        newDocumentItems.Add(certificate);
                        }

                    certificate.AddWareId(wareId);
                    }

                if (!string.IsNullOrEmpty(declaration1Number)
                    && declaration1Number.Equals(declaration2Number))
                    {
                    List<Approvals> itemsList;
                    if (!declarationsCache.TryGetValue(declaration1Number, out itemsList))
                        {
                        itemsList = new List<Approvals>();
                        declarationsCache.Add(declaration1Number, itemsList);

                        var item = createNewApprovalDeclaration(certificate, declaration1Number);
                        newDocumentItems.Add(item);
                        itemsList.Add(item);
                        item.AddWareId(wareId);

                        item = createNewApprovalDeclaration(certificate, declaration1Number);
                        newDocumentItems.Add(item);
                        itemsList.Add(item);
                        item.AddWareId(wareId);
                        }
                    else
                        {
                        itemsList.ForEach(item =>
                            {
                                newDocumentItems.Add(item);
                                item.AddWareId(wareId);
                            });

                        while (itemsList.Count < 2)
                            {
                            var item = createNewApprovalDeclaration(certificate, declaration1Number);
                            newDocumentItems.Add(item);
                            itemsList.Add(item);
                            item.AddWareId(wareId);
                            }
                        }
                    }
                else
                    {
                    if (!string.IsNullOrEmpty(declaration1Number))
                        {
                        List<Approvals> itemsList;
                        Approvals item = null;
                        if (!declarationsCache.TryGetValue(declaration1Number, out itemsList))
                            {
                            itemsList = new List<Approvals>();
                            declarationsCache.Add(declaration1Number, itemsList);

                            item = createNewApprovalDeclaration(certificate, declaration1Number);
                            itemsList.Add(item);
                            }
                        else
                            {
                            item = itemsList[0];
                            }

                        item.AddWareId(wareId);
                        newDocumentItems.Add(item);
                        }

                    if (!string.IsNullOrEmpty(declaration2Number))
                        {
                        List<Approvals> itemsList;
                        Approvals item = null;
                        if (!declarationsCache.TryGetValue(declaration2Number, out itemsList))
                            {
                            itemsList = new List<Approvals>();
                            declarationsCache.Add(declaration2Number, itemsList);

                            item = createNewApprovalDeclaration(certificate, declaration2Number);
                            itemsList.Add(item);
                            }
                        else
                            {
                            item = itemsList[0];
                            }

                        item.AddWareId(wareId);
                        newDocumentItems.Add(item);
                        }
                    }
                notifyProgress((1.0 + rowIndex) / totalRowsCount);
                }

            notifyProgress(0);
            for (int i = 0; i < newDocumentItems.Count; i++)
                {
                var item = newDocumentItems[i];
                var writtenResult = item.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    errorDescription = "Не удалось записать элемент: " + item;
                    return false;
                    }
                notifyProgress((1.0 + i) / newDocumentItems.Count);
                }

            errorDescription = null;
            return true;
            }

        private Approvals createNewApprovalDeclaration(Approvals certificate, string docNumber)
            {
            var item = A.New<Approvals>();
            item.SetRef("Contractor", O.Contractor.Id);
            item.DocumentNumber = docNumber;
            item.SetRef("DocumentType", declarationType.Id);
            item.DocumentCode = declarationType.QualifierCodeName;
            item.DateFrom = defaultStartDate;
            item.DateTo = DateTime.Now.AddYears(2);
            item.BaseApproval = certificate;
            return item;
            }
        }

    }
