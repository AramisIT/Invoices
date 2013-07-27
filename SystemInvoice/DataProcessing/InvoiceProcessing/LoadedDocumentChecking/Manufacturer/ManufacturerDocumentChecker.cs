using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Manufacturer
    {
    /// <summary>
    /// Проверяет соответствие производителя значению сохраненному в базе для номенклатуры
    /// </summary>
    public class ManufacturerDocumentChecker : NomenclatureChecker
        {
        public ManufacturerDocumentChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override bool CheckThatEquals(Cache.NomenclaturesCache.NomenclatureCacheObject nomenclatureCacheObject, string expectededValue)
            {
            string manufacturerName = dbCache.GetNomenclatureManufacturer(nomenclatureCacheObject);
            return manufacturerName.Equals(expectededValue);
            }

        protected override bool CheckExpectedValue(string expectedValue, ExcelMapper mapper)
            {
            return dbCache.ManufacturerCacheObjectsStore.GetManufcaturerId(expectedValue) > 0;
            }

        protected override string ColumnToCheck
            {
            get { return ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME; }
            }

        protected override CellError CreateError(string expectedValue, Cache.NomenclaturesCache.NomenclatureCacheObject nomenclatureCacheObject)
            {
            if (nomenclatureCacheObject == null)
                {
                return new ManufacturerCheckError();
                }
            return new ManufacturerCheckError(expectedValue, dbCache.GetNomenclatureManufacturer(nomenclatureCacheObject), ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME);
            }
        }
    }
