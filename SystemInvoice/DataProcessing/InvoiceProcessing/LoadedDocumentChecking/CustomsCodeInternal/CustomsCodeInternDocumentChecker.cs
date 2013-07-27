using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.CustomsCodeInternal
    {
    /// <summary>
    /// Проверяет соответствие внутреннего таможенного кода, сохраненному в номенклатуре
    /// </summary>
    public class CustomsCodeInternDocumentChecker : NomenclatureChecker
        {
        public CustomsCodeInternDocumentChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override bool CheckThatEquals(Cache.NomenclaturesCache.NomenclatureCacheObject nomenclatureCacheObject, string expectededValue)
            {
            string dbValue = dbCache.GetNomenclatureCustomsCodeIntern(nomenclatureCacheObject);
            if (dbValue != null && expectededValue != null)
                {
                return dbValue.Equals(expectededValue);
                }
            return false;
            }

        protected override bool CheckExpectedValue(string expectedValue, ExcelMapper mapper)
            {
            return dbCache.CustomsCodesCacheStore.GetCustomsCodeIdForCodeName(expectedValue) != 0;
            }

        protected override string ColumnToCheck
            {
            get { return ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME; }
            }

        protected override CellError CreateError(string expectedValue, Cache.NomenclaturesCache.NomenclatureCacheObject nomenclatureCacheObject)
            {
            if (nomenclatureCacheObject == null)
                {
                return new CustomsCodeInternCheckError();
                }
            else
                {
                return new CustomsCodeInternCheckError(expectedValue, dbCache.GetNomenclatureCustomsCodeIntern(nomenclatureCacheObject), ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME);
                }
            }
        }
    }
