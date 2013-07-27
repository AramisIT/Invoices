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
    /// Осуществляет перевод наименований декларации и инвойса после загрузки документа в табличную часть
    /// </summary>
    public class NamesTranslationHandler
        {
        private Invoice invoice = null;
        private SystemInvoiceDBCache dbCache = null;
        private TranslateContentHandler translateContentHandler = null;
        /// <summary>
        /// Обработчик загружаемого документа, который используется для осуществления перевода уже после загрузки
        /// </summary>
        private TranslationHandler translationHandler = null;

        public NamesTranslationHandler(Invoice invoice, SystemInvoiceDBCache invoiceDBCache)
            {
            this.invoice = invoice;
            this.dbCache = invoiceDBCache;
            translationHandler = new TranslationHandler(invoiceDBCache);
            translateContentHandler = new TranslateContentHandler(invoiceDBCache);
            }

        /// <summary>
        /// Осуществляет перевод наименований декларации и инвойса если есть соответствующие настройки в формате загрузки
        /// </summary>
        public void SetNamesTranslation(DataTable table)
            {
            bool haveUpdateDeclarationName = this.haveToUpdateNomenclatureDeclaration();
            bool haveUpdateInvoiceName = this.haveToUpdateNomenclatureInvoice();
            bool haveUpdateContentTranslation = this.haveUpdateContentTranslation();
            if (haveUpdateDeclarationName || haveUpdateInvoiceName)
                {
                foreach (DataRow row in table.Rows)
                    {
                    refreshRowNames(row, haveUpdateDeclarationName, haveUpdateInvoiceName);
                    if (haveUpdateContentTranslation)
                        {
                        this.refreshContentTranslation(row);
                        }
                    }
                }
            }

        private void refreshContentTranslation(DataRow row)
            {
            string contentOld = row.TrySafeGetColumnValue(InvoiceColumnNames.Content.ToString(), string.Empty);
            if (!string.IsNullOrEmpty(contentOld))
                {
                string translatedContent = translateContentHandler.ProcessRow(contentOld).ToString().Trim();
                if (!translatedContent.Equals(contentOld))
                    {
                    row[InvoiceColumnNames.Content.ToString()] = translatedContent;
                    }
                }
            }

        private bool haveUpdateContentTranslation()
            {
            return this.invoice.Contractor.Description.Trim().ToLower().StartsWith("bns");
            }

        private void refreshRowNames(DataRow row, bool haveUpdateDeclarationName, bool haveUpdateInvoiceName)
            {
            if (haveUpdateDeclarationName)
                {
                string declarationName = row.TrySafeGetColumnValue(InvoiceColumnNames.NomenclatureDeclaration.ToString(), string.Empty);
                if (!string.IsNullOrEmpty(declarationName))
                    {
                    string translatedDeclaration = translationHandler.ProcessRow(declarationName).ToString();
                    if (!translatedDeclaration.Equals(declarationName))
                        {
                        row[InvoiceColumnNames.NomenclatureDeclaration.ToString()] = translatedDeclaration;
                        }
                    }
                }
            if (haveUpdateInvoiceName)
                {
                string invoiceName = row.TrySafeGetColumnValue(InvoiceColumnNames.NomenclatureInvoice.ToString(), string.Empty);
                if (!string.IsNullOrEmpty(invoiceName))
                    {
                    string translatedInvoice = translationHandler.ProcessRow(invoiceName).ToString();
                    if (!translatedInvoice.Equals(invoiceName))
                        {
                        row[InvoiceColumnNames.NomenclatureInvoice.ToString()] = translatedInvoice;
                        }
                    }
                }
            }

        private bool haveToUpdateNomenclatureInvoice()
            {
            try
                {
                DataTable columnsMapping = this.invoice.ExcelLoadingFormat.ColumnsMappings;
                foreach (DataRow row in columnsMapping.Rows)
                    {
                    int columnName = (int)row[this.invoice.ExcelLoadingFormat.ColumnName];
                    if (columnName == (int)InvoiceColumnNames.NomenclatureInvoice)
                        {
                        string columnDescription = (string)row[this.invoice.ExcelLoadingFormat.ColumnNumberInExcel];
                        if (columnDescription.Trim().ToLower().StartsWith("перевод["))
                            {
                            return true;
                            }
                        }
                    }
                return false;
                }
            catch
                {
                return false;
                }
            }

        private bool haveToUpdateNomenclatureDeclaration()
            {
            try
                {
                DataTable columnsMapping = this.invoice.ExcelLoadingFormat.ColumnsMappings;
                foreach (DataRow row in columnsMapping.Rows)
                    {
                    int columnName = (int)row[this.invoice.ExcelLoadingFormat.ColumnName];
                    if (columnName == (int)InvoiceColumnNames.NomenclatureDeclaration)
                        {
                        string columnDescription = (string)row[this.invoice.ExcelLoadingFormat.ColumnNumberInExcel];
                        if (columnDescription.Trim().ToLower().StartsWith("перевод["))
                            {
                            return true;
                            }
                        }
                    }
                return false;
                }
            catch
                {
                return false;
                }
            }
        }
    }
