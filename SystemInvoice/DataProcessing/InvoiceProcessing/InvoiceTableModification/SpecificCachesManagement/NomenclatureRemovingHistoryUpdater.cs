using System;
using System.Collections.Generic;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.SpecificCachesManagement
    {
    /// <summary>
    /// Обновляет кэш удаленной из РД номенклатуры, с тем, что бы он был актуальным для текущего производителя/контрагента в инвойсе и для тех дат инвойса, которые присутствуют
    /// в табличной части.
    /// </summary>
    public class NomenclatureRemovingHistoryUpdater
        {
        private Invoice invoice = null;
        private SystemInvoiceDBCache cache = null;
        public NomenclatureRemovingHistoryUpdater(SystemInvoiceDBCache cache, Invoice invoice)
            {
            this.cache = cache;
            this.invoice = invoice;
            }

        public void RefreshRequiredNomenclatureCache()
            {
            //обновление осуществляем в транзакции
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
                HashSet<DateTime> dates = getInvoiceDates(this.invoice.Goods);
                this.cache.NomenclatureRemovingHistoryCacheObjectsStore.Refresh(invoice.TradeMark.Id, invoice.Contractor.Id, dates);
                }
            finally
                {
                if (!isInExistedTran)
                    {
                    TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                    }
                }
            }

        /// <summary>
        /// Получаем даты инвойса в текущем документе
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
            string invoiceDateStr = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME,
                                                                      string.Empty);
            DateTime dt;
            if (!DateTime.TryParse(invoiceDateStr, out dt))
                {
                dt = DateTime.MinValue;
                }
            return dt;
            }
        }
    }
