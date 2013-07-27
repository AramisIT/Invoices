using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureOriginalName
    {
    /// <summary>
    /// Проверяет соответствие исходного наименования значению сохраненному в базе для номенклатуры
    /// </summary>
    public class NomenclatureOriginalNameChecker : NomenclatureChecker
        {
        private readonly string nomenclatureOriginalName;
        public NomenclatureOriginalNameChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            nomenclatureOriginalName = ProcessingConsts.ColumnNames.NOMENCLATURE_ORIGINAL_COLUMN_NAME;
            }

        protected override bool CheckThatEquals(NomenclatureCacheObject nomenclatureCacheObject, string expectededValue)
            {
            return nomenclatureCacheObject.NameOriginal.Equals(expectededValue);
            }

        protected override bool CheckExpectedValue(string expectedValue, ExcelMapper mapper)
            {
            return true;
            }

        protected override bool CheckThatHaveToProcess(ExcelMapper mapper)
            {
            return mapper.ContainsKey(nomenclatureOriginalName);
            }

        protected override string ColumnToCheck
            {
            get { return nomenclatureOriginalName; }
            }

        protected override CellError CreateError(string expectedValue, NomenclatureCacheObject nomenclatureCacheObject)
            {
            if (nomenclatureCacheObject == null)
                {
                return null;
                }
            return new NomenclatureOriginalError(expectedValue, nomenclatureCacheObject.NameOriginal, nomenclatureOriginalName);
            }
        }
    }
