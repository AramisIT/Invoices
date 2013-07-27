using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.CustomDataProcessing
    {
    /// <summary>
    /// Осуществляет подтягивания кода единицы товара, по сокращенному наименованию
    /// </summary>
    class UnitOfMeasureCodeRetreiveHandler
        {
        private Invoice invoice = null;
        private SystemInvoiceDBCache dbCache = null;
        private GetUnitOfMeasureCodeFromNameHandler getUnitOfMeasureHandler = null;

        public UnitOfMeasureCodeRetreiveHandler(Invoice invoice, SystemInvoiceDBCache invoiceDBCache)
            {
            this.invoice = invoice;
            this.dbCache = invoiceDBCache;
            getUnitOfMeasureHandler = new GetUnitOfMeasureCodeFromNameHandler(invoiceDBCache);
            }


        /// <summary>
        /// Осуществляет установку кода свойства на основании имени свойства
        /// </summary>
        public void SetUnitOfMeasures(DataTable table)
            {
            bool haveUpdateUnitOfMeasureCode = this.haveUpdateUnitOfMeasureCode();
            if (haveUpdateUnitOfMeasureCode)
                {
                foreach (DataRow row in table.Rows)
                    {
                    this.refreshUnitOfMeasureCode(row);
                    }
                }
            }

        private void refreshUnitOfMeasureCode(DataRow row)
            {
            string unitOfMeasureName = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.UNIT_OF_MEASURE_COLUMN_NAME, string.Empty);
            if (!string.IsNullOrEmpty(unitOfMeasureName))
                {
                string unitOfMeasureCode = getUnitOfMeasureHandler.ProcessRow(unitOfMeasureName).ToString();
                if (!string.IsNullOrEmpty(unitOfMeasureCode))
                    {
                    string oldValue = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.UNIT_OF_MEASURE_CODE_COLUMN_NAME, string.Empty);
                    if (!oldValue.Equals(unitOfMeasureCode))
                        {
                        row[ProcessingConsts.ColumnNames.UNIT_OF_MEASURE_CODE_COLUMN_NAME] = unitOfMeasureCode;
                        }
                    }
                }
            }

        private bool haveUpdateUnitOfMeasureCode()
            {
            try
                {
                bool unitOfMeasureAccepted = false;
                bool unitOfMeasureCodeAccepted = false;
                DataTable columnsMapping = this.invoice.ExcelLoadingFormat.ColumnsMappings;
                foreach (DataRow row in columnsMapping.Rows)
                    {
                    if ((int)InvoiceColumnNames.UnitOfMeasure
                        == (int)row[this.invoice.ExcelLoadingFormat.ColumnName])
                        {
                        unitOfMeasureAccepted = true;
                        }
                    if ((int)InvoiceColumnNames.UnitOfMeasureCode
                        == (int)row[this.invoice.ExcelLoadingFormat.ColumnName])
                        {
                        unitOfMeasureCodeAccepted = true;
                        }
                    }
                return unitOfMeasureAccepted && unitOfMeasureCodeAccepted;
                }
            catch
                {
                return false;
                }
            }
        }
    }
