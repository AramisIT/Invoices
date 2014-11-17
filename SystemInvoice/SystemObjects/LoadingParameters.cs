using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.Documents;
using Aramis.Core;
using Aramis.DataBase;
using Aramis.DatabaseConnector;
using Aramis.SystemConfigurations;
using Aramis.UI;
using Aramis.UI.WinFormsDevXpress;
using ReportView;
using TableViewInterfaces;

namespace SystemInvoice.SystemObjects
    {
    public abstract class LoadingParameters
        {
        public abstract IContractor Contractor { get; }

        public abstract int ModelIndex { get; }

        public abstract int ArticleIndex { get; }

        public abstract int DateIndex { get; }

        public abstract bool ProcessExcelRow(LoadingEuroluxBehaviour.ExcelRow row,
            ILoadingWareRow docRow,
            List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> approvalDocuments);

        public StringBuilder Warnings { get; private set; }

        protected void addWarning(string message, LoadingEuroluxBehaviour.ExcelRow row, string comment = "")
            {
            Warnings.AppendLine(
                string.Format("{0} Страница - {1} № стр. - {2}; {3}",
                message.PadRight(40), row.Sheet.Name.PadRight(25), row.RowNumber, comment));
            }

        internal void Init()
            {
            Warnings = new StringBuilder();
            }

        public List<ICatalog> NewCatalogItems = new List<ICatalog>();
        public List<IDocument> NewDocumentItems = new List<IDocument>();

        public StringCacheDictionary ApprovalsCertTypes { get; set; }
        public StringCacheDictionary CustomsCodes { get; set; }
        public StringCacheDictionary ApprovalsTypes { get; set; }
        public StringCacheDictionary Countries { get; set; }
        public StringCacheDictionary UnitMeasures { get; set; }
        public Dictionary<string, IManufacturer> Producers { get; set; }
        public Dictionary<string, ITradeMark> TradeMarks { get; set; }
        public Dictionary<DateTime, Dictionary<string, List<Approvals>>> ApprovalsCache { get; set; }

        public Dictionary<string, List<Approvals>> DeclarationsCache { get; set; }
        protected StringCacheDictionary NomnclatureCache { get; set; }
        public Dictionary<string, Approvals> CertificatesCache { get; set; }
        public IDocumentType DeclarationType { get; set; }
        public IDocumentType CertificateType { get; set; }

        public DateTime DefaultStartDate = new DateTime(2010, 1, 1);


        public long GetId(object value, StringCacheDictionary cache, string fieldName, bool throwException = true)
            {
            if (value == null) return 0;

            var strValue = value.ToString().Trim();
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

        public T GetItem<T>(string strValue, Dictionary<string, T> cache) where T : IDatabaseObject
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
                    item.SetRef("Contractor", Contractor.Id);
                    if (item is ICatalog)
                        {
                        ((ICatalog)item).Description = strValue;
                        NewCatalogItems.Add((ICatalog)item);
                        }
                    else
                        {
                        NewDocumentItems.Add((IDocument)item);
                        }
                    cache.Add(strValue, item);
                    return item;
                    }
                }

            throw new Exception(string.Format("Не удалось получить поле {0}",
                    SystemConfiguration.DBConfigurationTree[typeof(T).GetTableName()].Description));
            }

        public abstract bool TryLoadApprovals(System.Data.DataSet dataSet, Action<double> notifyProgress,
            int approvalDurationYears,
            out string errorDescription, out string errorHelpData);

        internal abstract void InitForApprovals();
        }

    class ElectroluxLoadingParameters : LoadingParameters
        {
        private static IContractor _ELECTROLUX_CONTRACTOR;
        public static IContractor ELECTROLUX_CONTRACTOR
            {
            get
                {
                return _ELECTROLUX_CONTRACTOR ??
                    (_ELECTROLUX_CONTRACTOR = CatalogTable.FindByDescription(typeof(IContractor).GetTableName(), "Electrolux") as IContractor);
                }
            }

        public override IContractor Contractor
            {
            get { return ELECTROLUX_CONTRACTOR; }
            }

        public override int ModelIndex
            {
            get { return 4; }
            }

        public override int ArticleIndex
            {
            get { return 20; }
            }

        public override int DateIndex
            {
            get { return 2; }
            }

        public override bool ProcessExcelRow(LoadingEuroluxBehaviour.ExcelRow row,
            ILoadingWareRow docRow,
            List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> approvalDocuments)
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

            docRow.Model = row.Model;

            docRow.NameDecl = row.Row.GetString(nameDeclIndex);
            docRow.NameInvoice = row.Row.GetString(nameInvoiceIndex);

            if (string.IsNullOrEmpty(docRow.NameDecl)
                && string.IsNullOrEmpty(docRow.NameInvoice))
                {
                addWarning("Не заполненно ни наим. декларации ни наим. инв.!", row);
                docRow.RemoveFromTable();
                return true;
                }

            if (string.IsNullOrEmpty(docRow.NameDecl))
                {
                docRow.NameDecl = docRow.NameInvoice;
                }
            else if (string.IsNullOrEmpty(docRow.NameInvoice))
                {
                docRow.NameInvoice = docRow.NameDecl;
                }

            var internalColde = row.Row.GetString(customsCodeIndex);
            docRow.CodeInternal.Id = GetId(internalColde, CustomsCodes, "УКТЗЕД", false);

            if (docRow.CodeInternal.Id == 0)
                {
                addWarning("Не найден УКТЗЕД!", row, internalColde);
                docRow.RemoveFromTable();
                return true;
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
                docRow.Country.Id = GetId(row.Row.GetString(countryIndex), Countries, "Страна");
                docRow.MeasureUnit.Id = GetId(row.Row.GetString(unitMeasureIndex), UnitMeasures, "Един. изм.");

                var producer = row.Row.GetString(producerIndex);
                if (string.IsNullOrEmpty(producer))
                    {
                    addWarning("Не заполненно производитель!", row);
                    docRow.RemoveFromTable();
                    return true;
                    }

                docRow.Producer.Item = GetItem<IManufacturer>(producer, Producers);
                if (docRow.Producer.Item.Id == 0)
                    {
                    addWarning("Не заполненно производитель!", row);
                    docRow.RemoveFromTable();
                    return true;
                    }

                docRow.TradeMark.Item = GetItem<ITradeMark>(row.Row.GetString(tradeMarkIndex), TradeMarks);
                }
            catch (Exception exp)
                {
                string.Format("Ошибка в строке № {0}, на странице \"{1}\".\r\n{2}", row.RowNumber, row.Sheet.Name, exp.Message).WarningBox();
                return false;
                }

            const int approvalsIndexOffset = 21;

            const int approvalNumberIndex = 1;
            const int approvalDateIndex = 2;
            const int certificateIndex = 3;


            var documentIndex = 1;
            var currentColumnOffset = approvalsIndexOffset;
            while (true)
                {
                var docCode = row.Row.GetString(currentColumnOffset).Trim();
                var docType = GetId(docCode, ApprovalsTypes, "Типы док-в", false);
                if (docType == 0)
                    {
                    break;
                    }

                if (!docCode.Equals("5111"))
                    {
                    var documentNumber = row.Row.GetString(currentColumnOffset + approvalNumberIndex);
                    var date = row.Row.GetDate(currentColumnOffset + approvalDateIndex);
                    if (date.IsEmpty())
                        {
                        if (!string.Format("Ошибка получения даты в строке № {1}; страница {0}", row.Sheet.Name,
                            row.RowNumber).Ask())
                            {
                            return false;
                            }
                        break;
                        }

                    var certNumber = row.Row.GetString(currentColumnOffset + certificateIndex);
                    if (certNumber == "5112" || certNumber == "5111")
                        {
                        certNumber = null;
                        }
                    var certId = 0L;
                    if (!string.IsNullOrEmpty(certNumber))
                        {
                        certId = getCertificate(certNumber.Trim(), date);
                        }

                    var newDoc = new LoadingEuroluxBehaviour.ApprovalDocumentInfo()
                    {
                        DocumentNumber = documentNumber.Trim(),
                        StartDate = date,
                        DocumentType = docType,
                        Cert = certId
                    };
                    if (!newDoc.StartDate.Equals(DateTime.MinValue) || !string.IsNullOrEmpty(newDoc.DocumentNumber))
                        {
                        approvalDocuments.Add(newDoc);
                        }
                    }

                currentColumnOffset += 3;
                docType = GetId(row.Row.GetString(currentColumnOffset), ApprovalsTypes, "Типы док-в", false);
                if (docType == 0)
                    {
                    currentColumnOffset++;
                    docType = GetId(row.Row.GetString(currentColumnOffset), ApprovalsTypes, "Типы док-в", false);
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

            fillApprovals(approvalDocuments);

            return true;
            }

        private void fillApprovals(List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> approvalDocuments)
            {
            var tasks = new Dictionary<DateTime, Dictionary<string, List<LoadingEuroluxBehaviour.ApprovalDocumentInfo>>>();

            foreach (var doc in approvalDocuments)
                {
                Dictionary<string, List<LoadingEuroluxBehaviour.ApprovalDocumentInfo>> subDict;
                if (!tasks.TryGetValue(doc.StartDate, out subDict))
                    {
                    subDict = new Dictionary<string, List<LoadingEuroluxBehaviour.ApprovalDocumentInfo>>(new IgnoreCaseStringEqualityComparer());
                    tasks.Add(doc.StartDate, subDict);
                    }

                List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> documents;
                if (!subDict.TryGetValue(doc.DocumentNumber, out documents))
                    {
                    documents = new List<LoadingEuroluxBehaviour.ApprovalDocumentInfo>();
                    subDict.Add(doc.DocumentNumber, documents);
                    }

                documents.Add(doc);
                }

            foreach (var kvp in tasks)
                {
                var date = kvp.Key;
                foreach (var kvpNumber in kvp.Value)
                    {
                    var number = kvpNumber.Key;
                    List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> documents = kvpNumber.Value;
                    var cachedDocs = getApprovalsDocument(date, number);
                    checkExistsDocuments(documents, cachedDocs);
                    }
                }
            }

        private List<Approvals> getApprovalsDocument(DateTime dateTime, string docNumber)
            {
            Dictionary<string, List<Approvals>> subDict;
            if (!ApprovalsCache.TryGetValue(dateTime, out subDict))
                {
                subDict = new Dictionary<string, List<Approvals>>(new IgnoreCaseStringEqualityComparer());
                ApprovalsCache.Add(dateTime, subDict);
                }

            List<Approvals> docs;
            if (!subDict.TryGetValue(docNumber, out docs))
                {
                docs = new List<Approvals>();
                subDict.Add(docNumber, docs);
                }

            return docs;
            }

        private Approvals getApprovalsDocument(List<Approvals> existDocs, int docIndex, LoadingEuroluxBehaviour.ApprovalDocumentInfo requaredDoc)
            {
            if (docIndex < existDocs.Count)
                {
                return existDocs[docIndex];
                }

            var newDoc = A.New<Approvals>();
            newDoc.DateFrom = requaredDoc.StartDate.Equals(DateTime.MinValue) ? DateTime.Now : requaredDoc.StartDate;
            newDoc.DateTo = newDoc.DateFrom.AddYears(2);
            newDoc.Date = DateTime.Now;
            if (requaredDoc.Cert > 0)
                {
                newDoc.SetRef("BaseApproval", requaredDoc.Cert);
                }
            newDoc.DocumentNumber = requaredDoc.DocumentNumber;
            newDoc.Contractor = Contractor;
            newDoc.SetRef("DocumentType", requaredDoc.DocumentType);
            existDocs.Add(newDoc);
            NewDocumentItems.Add(newDoc);
            return newDoc;
            }

        private void checkExistsDocuments(List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> requaredDocuments, List<Approvals> existDocs)
            {
            for (int docIndex = 0; docIndex < requaredDocuments.Count; docIndex++)
                {
                var requaredDoc = requaredDocuments[docIndex];
                requaredDoc.ApprovalsDocument = getApprovalsDocument(existDocs, docIndex, requaredDoc);
                }
            }

        private long getCertificate(string certNumber, DateTime date)
            {
            long id;
            if (!ApprovalsCertTypes.TryGetValue(certNumber, out id))
                {
                var cert = A.New<Approvals>();
                cert.DocumentNumber = certNumber;
                cert.DocumentType = DocumentTypeHelper.GetCertificateType();
                cert.DocumentCode = cert.DocumentType.QualifierCodeName;

                cert.DateFrom = date.Equals(DateTime.MinValue) ? DateTime.Now : date;
                cert.DateTo = cert.DateFrom.AddYears(2);
                cert.Date = DateTime.Now;
                cert.Contractor = Contractor;

                cert.Write();
                id = cert.Id;
                ApprovalsCertTypes.Add(certNumber, id);
                }
            return id;
            }

        private Approvals createNewApprovalDeclaration(Approvals certificate, string docNumber)
            {
            var item = A.New<Approvals>();
            item.SetRef("Contractor", Contractor.Id);
            item.DocumentNumber = docNumber;
            item.SetRef("DocumentType", DeclarationType.Id);
            item.DocumentCode = DeclarationType.QualifierCodeName;
            item.DateFrom = DefaultStartDate;
            item.DateTo = DateTime.Now.AddYears(2);
            item.BaseApproval = certificate;
            return item;
            }

        public override bool TryLoadApprovals(System.Data.DataSet dataSet, Action<double> notifyProgress, int approvalDurationYears, out string errorDescription, out string errorHelpData)
            {
            errorHelpData = null;

            DataTable sheet = dataSet.Tables[0];
            var totalRowsCount = (double)sheet.Rows.Count;
            notifyProgress(0.0);

            for (int rowIndex = 1; rowIndex < sheet.Rows.Count; rowIndex++)
                {
                var row = sheet.Rows[rowIndex];
                var wareId = GetId(row[0], NomnclatureCache, "ware", false);
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
                    certificate = GetItem<Approvals>(certificateNumber, CertificatesCache);
                    if (certificate.IsNew)
                        {
                        certificate.DocumentNumber = certificateNumber;
                        certificate.SetRef("DocumentType", CertificateType.Id);
                        certificate.DocumentCode = CertificateType.QualifierCodeName;
                        certificate.DateFrom = DefaultStartDate;
                        certificate.DateTo = DateTime.Now.AddYears(2);
                        var writtenResult = certificate.Write();
                        if (!writtenResult.IsSuccess())
                            {
                            errorDescription = certificate.LastWrittingError;
                            errorHelpData = certificateNumber;
                            return false;
                            }
                        }
                    else
                        {
                        // need to update
                        NewDocumentItems.Add(certificate);
                        }

                    certificate.AddWareId(wareId);
                    }

                if (!string.IsNullOrEmpty(declaration1Number)
                    && declaration1Number.Equals(declaration2Number))
                    {
                    List<Approvals> itemsList;
                    if (!DeclarationsCache.TryGetValue(declaration1Number, out itemsList))
                        {
                        itemsList = new List<Approvals>();
                        DeclarationsCache.Add(declaration1Number, itemsList);

                        var item = createNewApprovalDeclaration(certificate, declaration1Number);
                        NewDocumentItems.Add(item);
                        itemsList.Add(item);
                        item.AddWareId(wareId);

                        item = createNewApprovalDeclaration(certificate, declaration1Number);
                        NewDocumentItems.Add(item);
                        itemsList.Add(item);
                        item.AddWareId(wareId);
                        }
                    else
                        {
                        itemsList.ForEach(item =>
                        {
                            NewDocumentItems.Add(item);
                            item.BaseApproval = certificate;
                            item.AddWareId(wareId);
                        });

                        while (itemsList.Count < 2)
                            {
                            var item = createNewApprovalDeclaration(certificate, declaration1Number);
                            NewDocumentItems.Add(item);
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
                        if (!DeclarationsCache.TryGetValue(declaration1Number, out itemsList))
                            {
                            itemsList = new List<Approvals>();
                            DeclarationsCache.Add(declaration1Number, itemsList);

                            item = createNewApprovalDeclaration(certificate, declaration1Number);
                            itemsList.Add(item);
                            }
                        else
                            {
                            item = itemsList[0];
                            }

                        item.BaseApproval = certificate;
                        item.AddWareId(wareId);
                        NewDocumentItems.Add(item);
                        }

                    if (!string.IsNullOrEmpty(declaration2Number))
                        {
                        List<Approvals> itemsList;
                        Approvals item = null;
                        if (!DeclarationsCache.TryGetValue(declaration2Number, out itemsList))
                            {
                            itemsList = new List<Approvals>();
                            DeclarationsCache.Add(declaration2Number, itemsList);

                            item = createNewApprovalDeclaration(certificate, declaration2Number);
                            itemsList.Add(item);
                            }
                        else
                            {
                            item = itemsList[0];
                            }

                        item.BaseApproval = certificate;
                        item.AddWareId(wareId);
                        NewDocumentItems.Add(item);
                        }
                    }
                notifyProgress((1.0 + rowIndex) / totalRowsCount);
                }

            notifyProgress(0);
            for (int i = 0; i < NewDocumentItems.Count; i++)
                {
                var item = NewDocumentItems[i];
                var writtenResult = item.Write();
                if (writtenResult != WritingResult.Success)
                    {
                    errorDescription = "Не удалось записать элемент: " + item;
                    return false;
                    }
                notifyProgress((1.0 + i) / NewDocumentItems.Count);
                }

            errorDescription = null;
            return true;
            }

        internal override void InitForApprovals()
            {
            NomnclatureCache = new CatalogCacheCreator<Nomenclature>().GetDescriptionIdCache(new { Contractor = Contractor }, "Model");
            }
        }

    class WhirlpoolLoadingParameters : LoadingParameters
        {
        private static IContractor _WHIRLPOOL_CONTRACTOR;

        private int defaultApprovalDurationYears;

        public static IContractor WHIRLPOOL_CONTRACTOR
            {
            get
                {
                return _WHIRLPOOL_CONTRACTOR ??
                    (_WHIRLPOOL_CONTRACTOR = CatalogTable.FindByDescription(typeof(IContractor).GetTableName(), "Whirlpool") as IContractor);
                }
            }

        public override IContractor Contractor
            {
            get { return WHIRLPOOL_CONTRACTOR; }
            }

        public override int ModelIndex
            {
            get { return ArticleIndex; }
            }

        public override int ArticleIndex
            {
            get { return 4; }
            }

        public override int DateIndex
            {
            get { return 2; }
            }

        public override bool ProcessExcelRow(LoadingEuroluxBehaviour.ExcelRow row, ILoadingWareRow docRow, List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> approvalDocuments)
            {
            const int customsCodeIndex = 3;
            const int producerIndex = 16;
            const int tradeMarkIndex = 15;
            const int countryIndex = 14;
            const int unitMeasureIndex = 17;

            const int netIndex = 10;
            const int grossIndex = 30;
            const int priceIndex = 8;

            const int nameInvoiceIndex = 5;
            const int nameDeclIndex = 6;

            docRow.Model = row.Model;

            docRow.NameDecl = row.Row.GetString(nameDeclIndex);
            docRow.NameInvoice = row.Row.GetString(nameInvoiceIndex);

            if (string.IsNullOrEmpty(docRow.NameDecl)
                && string.IsNullOrEmpty(docRow.NameInvoice))
                {
                addWarning("Не заполненно ни наим. декларации ни наим. инв.!", row);
                docRow.RemoveFromTable();
                return true;
                }

            if (string.IsNullOrEmpty(docRow.NameDecl))
                {
                docRow.NameDecl = docRow.NameInvoice;
                }
            else if (string.IsNullOrEmpty(docRow.NameInvoice))
                {
                docRow.NameInvoice = docRow.NameDecl;
                }

            var internalColde = row.Row.GetString(customsCodeIndex);
            docRow.CodeInternal.Id = GetId(internalColde, CustomsCodes, "УКТЗЕД", false);

            if (docRow.CodeInternal.Id == 0)
                {
                addWarning("Не найден УКТЗЕД!", row, internalColde);
                docRow.RemoveFromTable();
                return true;
                }

            docRow.Price = row.Row.GetDouble(priceIndex);
            docRow.NetOfOne = row.Row.GetDouble(netIndex);
            docRow.GrossOfOne = row.Row.GetDouble(grossIndex);

            try
                {
                docRow.Country.Id = GetId(row.Row.GetString(countryIndex), Countries, "Страна");
                docRow.MeasureUnit.Id = GetId(row.Row.GetString(unitMeasureIndex), UnitMeasures, "Един. изм.");

                var producer = row.Row.GetString(producerIndex);
                if (string.IsNullOrEmpty(producer))
                    {
                    addWarning("Не заполненно производитель!", row);
                    docRow.RemoveFromTable();
                    return true;
                    }

                docRow.Producer.Item = GetItem<IManufacturer>(producer, Producers);
                if (docRow.Producer.Item.Id == 0)
                    {
                    addWarning("Не заполненно производитель!", row);
                    docRow.RemoveFromTable();
                    return true;
                    }

                docRow.TradeMark.Item = GetItem<ITradeMark>(row.Row.GetString(tradeMarkIndex), TradeMarks);
                }
            catch (Exception exp)
                {
                string.Format("Ошибка в строке № {0}, на странице \"{1}\".\r\n{2}", row.RowNumber, row.Sheet.Name, exp.Message).WarningBox();
                return false;
                }

            return true;
            }

        public override bool TryLoadApprovals(DataSet dataSet, Action<double> notifyProgress, int approvalDurationYears, out string errorDescription, out string errorHelpData)
            {
            bool justUpdateWares = KeyBoard.CtrlAltShiftPressed() && "Только обновить необход. разрешительные?".Ask();

            errorDescription = string.Empty;
            errorHelpData = string.Empty;
            defaultApprovalDurationYears = approvalDurationYears;
            notifyProgress(0.0);

            var totalRows = 0;
            var currentRowIndex = 0;


            foreach (DataTable table in dataSet.Tables)
                {
                for (int rowIndex = 2; rowIndex < table.Rows.Count; rowIndex++) totalRows++;
                }

            foreach (DataTable table in dataSet.Tables)
                {
                for (int rowIndex = 2; rowIndex < table.Rows.Count; rowIndex++)
                    {
                    var row = table.Rows[rowIndex];

                    const int acticleIndex = 4;
                    var wareId = GetId(row[acticleIndex], NomnclatureCache, "ware", false);
                    if (wareId == 0)
                        {
                        currentRowIndex++;
                        continue;
                        }

                    if (justUpdateWares)
                        {
                        var ware = A.New<Nomenclature>(wareId);
                        ware.NotifyPropertyChanged("Code");
                        var writtenResult = ware.Write();
                        if (!writtenResult.IsSuccess())
                            {
                            string.Format("Не удалось обновить товар: {0}", ware).NotifyToUser(MessagesToUserTypes.Error);
                            return false;
                            }
                        }
                    else
                        {
                        if (!loadApprovals(wareId, row, out errorDescription))
                            {
                            errorHelpData = A.New<Nomenclature>(wareId).Article;
                            return false;
                            }
                        }
                    currentRowIndex++;
                    notifyProgress(((double)currentRowIndex) / totalRows);
                    }
                }

            return true;
            }

        private bool loadApprovals(long wareId, DataRow row, out string errorDescription)
            {
            const int offset = 7;
            const int docNumberIndex = 1;
            const int startDateIndex = 2;
            const int finishDateIndex = 3;

            var approvalsInfo = new List<KeyValuePair<ApprovalInfo, int>>();

            int currentOffset = offset;
            int blockNumber = 0;
            int maxBlockNumber = 5;
            while (blockNumber < maxBlockNumber)
                {
                blockNumber++;
                if (currentOffset >= row.Table.Columns.Count)
                    {
                    break;
                    }

                var docType = GetId(row[currentOffset], ApprovalsTypes, "Типы док-в", false);
                if (docType == 0)
                    {
                    currentOffset += 4;
                    continue;
                    }

                var approvalInfo = new ApprovalInfo(defaultApprovalDurationYears) { DocumentType = docType };
                var docnumber = row[currentOffset + docNumberIndex].ToString().Trim();

                approvalInfo.Number = docnumber;
                Trace.WriteLine(docnumber);

                approvalInfo.StartDate = getDate(row[currentOffset + startDateIndex]);

                var finishDateResultIndex = currentOffset + finishDateIndex;
                if (finishDateResultIndex < row.Table.Columns.Count)
                    {
                    var nextDocType = GetId(row[finishDateResultIndex], ApprovalsTypes, "Типы док-в", false);
                    if (nextDocType == 0)
                        {
                        approvalInfo.FinishDate = getDate(row[finishDateResultIndex]);
                        currentOffset += 4;
                        }
                    else
                        {
                        currentOffset += 3;
                        }
                    }

                if (!approvalInfo.Number.StartsWith("#") && !string.IsNullOrEmpty(approvalInfo.Number))
                    {
                    var added = false;
                    for (int itemIndex = 0; itemIndex < approvalsInfo.Count; itemIndex++)
                        {
                        var item = approvalsInfo[itemIndex];

                        if (item.Key.Equals(approvalInfo))
                            {
                            var number = item.Value + 1;
                            approvalsInfo.Remove(item);
                            approvalsInfo.Add(new KeyValuePair<ApprovalInfo, int>(approvalInfo, number));
                            added = true;
                            }
                        }
                    if (!added)
                        {
                        approvalsInfo.Add(new KeyValuePair<ApprovalInfo, int>(approvalInfo, 1));
                        }
                    }
                }

            return saveApprovalsToDatabase(wareId, approvalsInfo, out  errorDescription);
            }

        private DateTime getDate(object value)
            {
            if (value is DateTime)
                {
                return (DateTime)value;
                }

            return DateTime.MinValue;
            }

        private bool saveApprovalsToDatabase(long wareId, List<KeyValuePair<ApprovalInfo, int>> approvalsInfo, out string errorDescription)
            {
            foreach (var item in approvalsInfo)
                {
                var approval = item.Key;

                var q = DB.NewQuery(@"select Id, case when n.ItemNomenclature is null then 0 else 1 end ContainsWare
	from Approvals a
	left join SubApprovalsNomenclatures n on n.IdDoc = a.Id and n.ItemNomenclature = @Ware
	where a.Contractor = @Contractor and a.DocumentType=@DocumentType
		and a.Deleted = 0 and a.DateFrom = @StartDate and a.DateTo = @FinishDate
		and a.DocumentNumber = @Number
		
		order by ContainsWare desc");
                q.AddInputParameter("Contractor", Contractor.Id);
                q.AddInputParameter("Number", approval.Number);
                q.AddInputParameter("StartDate", approval.StartDate);
                q.AddInputParameter("FinishDate", approval.FinishDate);
                q.AddInputParameter("DocumentType", approval.DocumentType);
                q.AddInputParameter("Ware", wareId);

                var docsCount = item.Value;
                using (var qResult = q.Select())
                    {
                    while (qResult.Next())
                        {
                        var containsWare = qResult["ContainsWare"].ToBoolean();
                        if (!containsWare)
                            {
                            var doc = A.New<Approvals>(qResult["Id"]);
                            doc.AddWareId(wareId);
                            var writtenResult = doc.Write();

                            if (!writtenResult.IsSuccess())
                                {
                                errorDescription = doc.LastWrittingError;
                                return false;
                                }
                            }

                        docsCount--;
                        }
                    }

                for (int i = 0; i < docsCount; i++)
                    {
                    var newDoc = A.New<Approvals>();
                    newDoc.DateFrom = approval.StartDate;
                    newDoc.DateTo = approval.FinishDate;
                    newDoc.Date = DateTime.Now;
                    newDoc.DocumentNumber = approval.Number;
                    newDoc.Contractor = Contractor;
                    newDoc.DocumentType = A.New<IDocumentType>(approval.DocumentType);
                    newDoc.DocumentCode = newDoc.DocumentType.QualifierCodeName;

                    newDoc.AddWareId(wareId);

                    var writtenResult = newDoc.Write();

                    if (!writtenResult.IsSuccess())
                        {
                        errorDescription = newDoc.LastWrittingError;
                        return false;
                        }
                    }
                }

            errorDescription = null;
            return true;
            }

        class ApprovalInfo
            {
            private int defaultApprovalDurationYears;
            public ApprovalInfo(int defaultApprovalDurationYears)
                {
                this.defaultApprovalDurationYears = defaultApprovalDurationYears;
                }

            public string Number { get; set; }

            public DateTime StartDate { get; set; }

            private DateTime finishDate;

            public DateTime FinishDate
                {
                get
                    {
                    return finishDate.Equals(DateTime.MinValue) ?
                        StartDate.AddYears(defaultApprovalDurationYears)
                        : finishDate;
                    }

                set { finishDate = value; }
                }

            public long DocumentType { get; set; }

            public override bool Equals(object obj)
                {
                var item2 = obj as ApprovalInfo;
                if (item2.IsNull()) return false;

                return item2.DocumentType == DocumentType
                       && item2.Number.Equals(Number)
                       && item2.StartDate.Equals(StartDate)
                       && item2.FinishDate.Equals(FinishDate);
                }
            }

        internal override void InitForApprovals()
            {
            NomnclatureCache = new CatalogCacheCreator<Nomenclature>().GetDescriptionIdCache(new { Contractor = Contractor }, "Article");
            }
        }
    }
