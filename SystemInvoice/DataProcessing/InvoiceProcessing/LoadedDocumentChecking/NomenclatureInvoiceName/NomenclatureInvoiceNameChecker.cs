using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureInvoiceName
    {
    /// <summary>
    /// Проверяет соответствие наименования инвойса значению сохраненному в базе для номенклатуры
    /// </summary>
    public class NomenclatureInvoiceNameChecker : NomenclatureChecker
        {
        private readonly string nomenclatureInvoiceName;
        public NomenclatureInvoiceNameChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            nomenclatureInvoiceName = ProcessingConsts.ColumnNames.NOMENCLATURE_INVOICE_COLUM_NAME;
            }

        protected override bool CheckThatEquals(NomenclatureCacheObject nomenclatureCacheObject, string expectededValue)
            {
            return nomenclatureCacheObject.NameInvoice.Equals(expectededValue);
            }

        protected override string ColumnToCheck
            {
            get { return nomenclatureInvoiceName; }
            }

        protected override CellError CreateError(string expectedValue, NomenclatureCacheObject nomenclatureCacheObject)
            {
            if (nomenclatureCacheObject == null)
                {
                if (string.IsNullOrEmpty(expectedValue))
                    {
                    return new NomenclatureInvoiceError();
                    }
                return null;
                }
            return new NomenclatureInvoiceError(expectedValue, nomenclatureCacheObject.NameInvoice, nomenclatureInvoiceName);
            }
        }
    }
