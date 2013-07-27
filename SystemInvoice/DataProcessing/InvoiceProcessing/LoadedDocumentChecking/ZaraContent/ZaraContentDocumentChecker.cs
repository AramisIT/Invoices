using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.PropertyTypesCache;
using SystemInvoice.Excel;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.ZaraContent
    {
    /// <summary>
    /// Проверяет состав зары - соответствие колонок код, наименование английское, наименование русское/украинское в справочнике виды свойств. Проверака происходит для тех колонок которые указаны в формате загрузки.
    /// </summary>
    public class ZaraContentDocumentChecker : LoadedDocumentCheckerBase
        {
        public ZaraContentDocumentChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        private bool checkZaraColumns(System.Data.DataRow rowToChek, string currentZaraCodeColumnName, string currentZaraContentEnColumnName, string currentZaraContentUkrName)
            {
            string currentCode = rowToChek.TrySafeGetColumnValue<string>(currentZaraCodeColumnName, "").Trim();
            if (currentCode.Equals("000"))
                {
                currentCode = "";
                }
            string currentUkrName = rowToChek.TrySafeGetColumnValue<string>(currentZaraContentUkrName, "").Trim();
            string currentEnName = rowToChek.TrySafeGetColumnValue<string>(currentZaraContentEnColumnName, "").Trim();
            if (string.IsNullOrEmpty(currentCode) && string.IsNullOrEmpty(currentUkrName) && string.IsNullOrEmpty(currentEnName))//при отсутствующих значениях не проверяем
                {
                return true;
                }
            if (string.IsNullOrEmpty(currentCode) || string.IsNullOrEmpty(currentUkrName) || string.IsNullOrEmpty(currentEnName))
                {
                return false;
                }
            if (!this.checkContent(currentCode, currentUkrName, currentEnName))
                {
                return false;
                }
            return true;
            }

        private bool checkContent(string currentCode, string currentUkrName, string currentEnName)
            {
            long nomenclatureID = 0;
            long SubGroupOfGoodsId = 0;
            long typeOfPropertyID = dbCache.PropertyOfGoodsCacheObjectsStore.GetCachedObjectId("Состав");
            PropertyTypesCacheObject propTypeCacheObject =
                new PropertyTypesCacheObject(nomenclatureID, SubGroupOfGoodsId, typeOfPropertyID, currentUkrName,
                                             currentEnName, currentCode, 0, 0, 0,string.Empty);
            long foundedId = dbCache.PropertyTypesCacheObjectsStore.GetCachedObjectId(propTypeCacheObject);
            if (foundedId == 0)
                {
                return false;
                }
            string ukrValue = dbCache.PropertyTypesCacheObjectsStore.GetCachedObject(foundedId).PropertyUkrValue;
            return ukrValue.Equals(currentUkrName);
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            if (!isDocumentCurrentlyLoaded || rowToCheck == null || mapper == null)
                {
                return;
                }
            RowColumnsErrors errors = new RowColumnsErrors();
            string zaraContentCodeTemplate = "ZaraContent{0}Code";
            string zaraContentEnNameTemplate = "ZaraContent{0}EnName";
            string zaraContentUkNameTemplate = "ZaraContent{0}UkrName";
            for (int i = 0; i <= 15; i++)
                {
                string currentZaraCodeColumnName = string.Format(zaraContentCodeTemplate, i);
                string currentZaraContentEnColumnName = string.Format(zaraContentEnNameTemplate, i);
                string currentZaraContentUkrName = string.Format(zaraContentUkNameTemplate, i);
                if (!checkZaraColumns(rowToCheck, currentZaraCodeColumnName, currentZaraContentEnColumnName, currentZaraContentUkrName))
                    {
                    AddError(currentZaraCodeColumnName, new ZaraContentError());
                    AddError(currentZaraContentEnColumnName, new ZaraContentError());
                    AddError(currentZaraContentUkrName, new ZaraContentError());
                    }
                }
            }
        }
    }
