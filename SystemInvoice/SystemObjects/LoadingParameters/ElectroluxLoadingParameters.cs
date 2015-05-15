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
                        var error = string.Format("\r\nОшибка получения даты в строке № {1}; страница {0}", row.Sheet.Name, row.RowNumber);
                        Warnings.AppendLine(error);
                        //if (!error.Ask())
                        //    {
                        //    return false;
                        //    }
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
    }
