using System;
using System.Collections.Generic;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.DataProcessing.Cache.ApprovalsCache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.RDChecking;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.ApprovalsModification
    {
    /// <summary>
    /// Ищет разрешительные документы в системе и добавляет их в строки таблицы инвойса
    /// </summary>
    public class ApprovalsSearcher
        {
        // private const int APPROVALS_FOR_ROW_COUNT = 3;
        private const string foundedApprovalColumnNamePrefix = "FoundedApprovals";
        ApprovalsCacheObjectsStore approvalsCacheObjectStore = null;
        private List<string> RDDocTypeCodeColumnNames = new List<string>();
        private List<string> RDDateFromColumnNames = new List<string>();
        private List<string> RDDateToColumnNames = new List<string>();
        private List<string> RDBaseNumberToColumnNames = new List<string>();
        private List<string> RDDocNumberColumnNames = new List<string>();
        private SystemInvoiceDBCache dbCache = null;
        private Invoice invoice;
        private string dateFormatStr;

        public ApprovalsSearcher(SystemInvoiceDBCache dbCache, Invoice invoice)
            {
            this.invoice = invoice;
            approvalsCacheObjectStore = dbCache.ApprovalsCacheObjectsStore;
            this.dbCache = dbCache;
            initColumnNames();
            }

        private void initColumnNames()
            {
            RDChecker.InitColumnsNames(RDDocTypeCodeColumnNames, RDDateFromColumnNames, RDDateToColumnNames, RDDocNumberColumnNames, RDBaseNumberToColumnNames);
            }

        public void FindApprovals(DataTable tableToProcess)
            {
            HashSet<DateTime> invoiceDates = getInvoiceDates(tableToProcess);
            bool isInExistedTran = TransactionManager.TransactionManagerInstance.IsInTransaction();
            try
                {
                if (!isInExistedTran)
                    {
                    if (!TransactionManager.TransactionManagerInstance.BeginBusinessTransaction())
                        {
                        return;
                        };
                    }
                approvalsCacheObjectStore.Refresh(0, dbCache.TradeMarkContractorSource.Contractor.Id, invoiceDates);
                }
            finally
                {
                if (!isInExistedTran)
                    {
                    TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                    }
                }

            dateFormatStr = invoice.ExcelLoadingFormat.DateFormatStr;
            if (string.IsNullOrEmpty(dateFormatStr))
                {
                dateFormatStr = "yyyy.MM.dd";
                }

            foreach (DataRow row in tableToProcess.Rows)
                {
                this.setApprovals(row);
                }
            }

        /// <summary>
        /// Возвращает уникальные даты инвойса которые встречаются в таблице
        /// </summary>
        private HashSet<DateTime> getInvoiceDates(DataTable tableToProcess)
            {
            HashSet<DateTime> dates = new HashSet<DateTime>();
            foreach (DataRow row in tableToProcess.Rows)
                {
                var dt = getInvoiceDate(row);
                dates.Add(dt);
                }
            return dates;
            }

        private DateTime getInvoiceDate(DataRow row)
            {
            string invoiceDateStr = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME, string.Empty);
            DateTime dt;
            if (!DateTime.TryParse(invoiceDateStr, out dt))
                {
                dt = DateTime.MinValue;
                }
            return dt;
            }

        private void setApprovals(DataRow row)
            {
            string article = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.Article.ToString(), "");
            string tradeMarkName = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.ItemTradeMark.ToString(), "");
            long nomenlatureId = dbCache.GetNomenclatureId(article, tradeMarkName);
            DateTime invoiceDate = getInvoiceDate(row);
            if (nomenlatureId != 0)
                {
                this.fillApprovals(row, invoiceDate, nomenlatureId);
                }
            }

        /// <summary>
        /// Ищет разрешительные в системе и записывает в табличную часть инвойса для строки
        /// </summary>
        private void fillApprovals(DataRow dataRow, DateTime invoiceDate, long nomenlatureId)
            {
            if (nomenlatureId != 0 && invoiceDate != DateTime.MinValue)
                {
                var foundedApprovals = approvalsCacheObjectStore.GetApprovals(nomenlatureId, invoiceDate);
                int i = 0;
                for (; i < ProcessingConsts.CHECKING_APPROVALS_COUNT && i < foundedApprovals.Count; i++)
                    {
                    string foundedApprovalColumnName = this.getFoundedApprovalColumnName(i);
                    ApprovalsCacheObject foundedApproval = foundedApprovals[i];
                    if (foundedApproval == null)
                        {
                        continue;
                        }
                    string currentDocTypeColumnName = RDDocTypeCodeColumnNames[i];
                    string currentDateFromColumnName = RDDateFromColumnNames[i];
                    string currentDateToColumnName = RDDateToColumnNames[i];
                    string currentDocNumberColumnName = RDDocNumberColumnNames[i];
                    dataRow[currentDocTypeColumnName] = foundedApproval.DocumentCodeName;
                    dataRow[currentDateFromColumnName] = foundedApproval.DateFrom.ToString(dateFormatStr);
                    dataRow[currentDateToColumnName] = foundedApproval.DateTo == DateTime.MinValue ? "" : foundedApproval.DateTo.ToString(dateFormatStr);
                    dataRow[currentDocNumberColumnName] = foundedApproval.DocumentNumber;
                    dataRow[foundedApprovalColumnName] = foundedApproval.ApprovalsId;
                    dataRow[RDBaseNumberToColumnNames[i]] = foundedApproval.DocumentBaseNumber;
                    }
                for (; i < ProcessingConsts.CHECKING_APPROVALS_COUNT; i++)
                    {
                    string currentDocTypeColumnName = RDDocTypeCodeColumnNames[i];
                    string currentDateFromColumnName = RDDateFromColumnNames[i];
                    string currentDateToColumnName = RDDateToColumnNames[i];
                    string currentDocNumberColumnName = RDDocNumberColumnNames[i];
                    if (!string.IsNullOrEmpty(dataRow[currentDocTypeColumnName].ToString().Trim()))
                        {
                        dataRow[currentDocTypeColumnName] = " ";
                        }
                    if (!string.IsNullOrEmpty(dataRow[currentDateFromColumnName].ToString().Trim()))
                        {
                        dataRow[currentDateFromColumnName] = " ";
                        }
                    if (!string.IsNullOrEmpty(dataRow[currentDateToColumnName].ToString().Trim()))
                        {
                        dataRow[currentDateToColumnName] = " ";
                        }
                    if (!string.IsNullOrEmpty(dataRow[currentDocNumberColumnName].ToString().Trim()))
                        {
                        dataRow[currentDocNumberColumnName] = " ";
                        }
                    }
                }
            }

        private string getFoundedApprovalColumnName(int i)
            {
            return string.Concat(foundedApprovalColumnNamePrefix, i + 1);
            }
        }
    }
