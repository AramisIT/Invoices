using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Country
    {
    /// <summary>
    /// Проверяет короткое наименование страны с сохраненным в номенклатуре значением
    /// </summary>
    public class CountryChecker : NomenclatureChecker
        {
        public CountryChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override bool CheckThatEquals(NomenclatureCacheObject nomenclatureCacheObject, string expectededValue)
            {
            string dbValue = dbCache.GetNomenclatureCountryShortName(nomenclatureCacheObject);
            if (dbValue != null && expectededValue != null)
                {
                return dbValue.Equals(expectededValue);
                }
            return false;
            }

        protected override bool CheckExpectedValue(string expectedValue, ExcelMapper mapper)
            {
            return dbCache.CountryCahceObjectsStore.GetIdForCountryShortName(expectedValue) != 0;
            }

        protected override string ColumnToCheck
            {
            get { return ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME; }
            }

        protected override CellError CreateError(string expectedValue, NomenclatureCacheObject nomenclatureCacheObject)
            {
            if (nomenclatureCacheObject == null)
                {
                return new CountryCheckError();
                }
            else
                {
                return new CountryCheckError(expectedValue, dbCache.GetNomenclatureCountryShortName(nomenclatureCacheObject), ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME);
                }
            }
        }
    }
