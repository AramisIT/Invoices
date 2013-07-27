using System;
using System.Collections.Generic;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.DataProcessing.Cache.ApprovalsCache;
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
        private List<string> RDDocNumberColumnNames = new List<string>();
        private SystemInvoiceDBCache dbCache = null;

        public ApprovalsSearcher(SystemInvoiceDBCache dbCache)
            {
            approvalsCacheObjectStore = dbCache.ApprovalsCacheObjectsStore;
            this.dbCache = dbCache;
            initColumnNames();
            }

        private void initColumnNames()
            {
            RDDocTypeCodeColumnNames.Clear();
            RDDateFromColumnNames.Clear();
            RDDateToColumnNames.Clear();
            RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode1.ToString());
            RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode2.ToString());
            RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode3.ToString());
            RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode4.ToString());
            RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode5.ToString());
            RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate1.ToString());
            RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate2.ToString());
            RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate3.ToString());
            RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate4.ToString());
            RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate5.ToString());
            RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate1.ToString());
            RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate2.ToString());
            RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate3.ToString());
            RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate4.ToString());
            RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate5.ToString());
            RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber1.ToString());
            RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber2.ToString());
            RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber3.ToString());
            RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber4.ToString());
            RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber5.ToString());
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
                    dataRow[currentDateFromColumnName] = foundedApproval.DateFrom.ToString("yyyy.MM.dd");
                    dataRow[currentDateToColumnName] = foundedApproval.DateTo == DateTime.MinValue ? "" : foundedApproval.DateTo.ToString("yyyy.MM.dd");
                    dataRow[currentDocNumberColumnName] = foundedApproval.DocumentNumber;
                    dataRow[foundedApprovalColumnName] = foundedApproval.ApprovalsId;
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
