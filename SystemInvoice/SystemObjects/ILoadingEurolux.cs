using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using AramisWpfComponents.Excel;
using NPOI.HSSF.Record.Formula.Functions;
using NPOI.SS.UserModel;
using TableViewInterfaces;
using Cell = AramisWpfComponents.Excel.Cell;
using Row = AramisWpfComponents.Excel.Row;

namespace SystemInvoice.SystemObjects
    {
    public interface ILoadingEurolux : IAramisModel
        {
        [DataField(Description = "Контрагент", ReadOnly = true)]
        IContractor Contractor { get; set; }

        Table<ILoadingEuroluxRow> Rows { get; }
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

    public class LoadingEuroluxBehaviour : Behaviour<ILoadingEurolux>
        {
        class ApprovalDocumentInfo
            {
            public long DocumentType { get; set; }
            public DateTime StartDate { get; set; }
            public string DocumentNumber { get; set; }

            public Approvals ApprovalsDocument { get; set; }
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

            customsCodes = new CatalogCacheCreator<CustomsCode>().GetDescriptionIdCache();
            countries = new CatalogCacheCreator<Country>().GetDescriptionIdCache(descriptionFieldName: "InternationalCode");
            unitMeasures = new CatalogCacheCreator<UnitOfMeasure>().GetDescriptionIdCache(descriptionFieldName: "ShortName");
            approvalsTypes = new CatalogCacheCreator<DocumentType>().GetDescriptionIdCache(descriptionFieldName: "QualifierCodeName");

            producers = new CatalogCacheCreator<IManufacturer>().GetDescriptionItemCache(new { Contractor = O.Contractor });
            tradeMarks = new CatalogCacheCreator<ITradeMark>().GetDescriptionItemCache(new { Contractor = O.Contractor });

            approvalsCache = getApprovalCache();
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

        internal void LoadExcelFile(string fileName, Action<double> notifyProgress)
            {
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
                for (int rowIndex = sheet.RowCount - 1; rowIndex > 0; rowIndex--)
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

            //  await Task.Factory.StartNew(() => 
            loadData(waresDictionary.Values.ToList(), notifyProgress);//);
            }

        List<ExcelRow> skippedRows = new List<ExcelRow>();

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

            newCatalogItems = new List<ICatalog>();
            newDocumentItems = new List<IDocument>();
            approvalsDocuments = new Dictionary<long, List<ApprovalDocumentInfo>>();

            for (int i = 0; i < list.Count; i++)
                {
                var row = list[i];

                var docRow = O.Rows.Add();
                docRow.Model = row.Model;
                docRow.Article = row.Article;

                docRow.NameDecl = row.Row[nameDeclIndex].Value.ToString();
                docRow.NameInvoice = row.Row[nameInvoiceIndex].Value.ToString();

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

                    //docRow.CodeInternal.Id = getId(row, customsCodeIndex, customsCodes, "УКТЗЕД");
                    docRow.Country.Id = getId(row, countryIndex, countries, "Страна");
                    docRow.MeasureUnit.Id = getId(row, unitMeasureIndex, unitMeasures, "Един. изм.");

                    var producer = row.Row[producerIndex].Value.ToString().TrimEnd();
                    if (string.IsNullOrEmpty(producer))
                        {
                        skippedRows.Add(row);
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
                    var docType = getId(row, currentColumnOffset, approvalsTypes, "Типы док-в", false);
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
                    docType = getId(row, currentColumnOffset, approvalsTypes, "Типы док-в", false);
                    if (docType == 0)
                        {
                        currentColumnOffset++;
                        docType = getId(row, currentColumnOffset, approvalsTypes, "Типы док-в", false);
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

            writeToDatabase(notifyProgress);
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

        private void writeToDatabase(Action<double> notifyProgress)
            {
            foreach (var item in newCatalogItems)
                {
                var writtenResult = item.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    ("Не удалось записать элемент: " + item.Description).WarningBox();
                    return;
                    }
                }

            foreach (var item in newDocumentItems)
                {
                var writtenResult = item.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    ("Не удалось записать элемент: " + item.ToString()).WarningBox();
                    return;
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

                var writtenResult = ware.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    ("Не удалось записать товар: " + ware.Description).WarningBox();
                    return;
                    }

                notifyProgress((double)itemIndex / (double)rowsCount);
                }

            "Данные загружены в базу!".NotifyToUser();
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

        private List<ICatalog> newCatalogItems;
        private List<IDocument> newDocumentItems;
        private Dictionary<long, List<ApprovalDocumentInfo>> approvalsDocuments;

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

        private long getId(ExcelRow row, int fieldIndex, StringCacheDictionary cache, string fieldName, bool throwException = true)
            {
            var strValue = row.Row[fieldIndex].Value.ToString();
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
        }

    }
