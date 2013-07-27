using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.SubGroupOfGoods
    {
    /// <summary>
    /// Проверяет соответствие подгруппы товара значению сохраненному в базе для номенклатуры
    /// </summary>
    public class SubGroupOfGoodsChecker : LoadedDocumentCheckerBase
        {

        public SubGroupOfGoodsChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            //С подгруппой товара связанно 2 колонки - имя подгруппы и код подгруппы, соответственно мы проверяем значения в обеих колонках, со значениями в базе
            string subGroupOfGoodsCode = ProcessingConsts.ColumnNames.GROUP_CODE_COLUMN_NAME;
            string subGroupOfGoodsName = ProcessingConsts.ColumnNames.SUB_GROUP_OF_GOODS_COLUMN_NAME;
            if (rowToCheck == null)
                {
                return;
                }
            RowColumnsErrors rowErrors = new RowColumnsErrors();
            string groupName = Helpers.InvoiceDataRetrieveHelper.GetRowGroupName(rowToCheck);
            string subGroupName = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupName(rowToCheck);
            string subGroupCode = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupCode(rowToCheck);
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            NomenclatureCacheObject cacheObject = null;
            if (nomenclatureId != 0)
                {
                cacheObject = dbCache.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureId);
                if (cacheObject != null)
                    {
                    string nomenclatureSubGroupName = dbCache.GetNomenclatureSubGroup(cacheObject);
                    string nomenclatureSubGropCode = dbCache.GetNomenclatureSubGroupCode(cacheObject);
                    if (!subGroupName.Equals(nomenclatureSubGroupName))
                        {
                        AddError(subGroupOfGoodsName, new SubGroupOfGoodsError(subGroupName, nomenclatureSubGroupName, subGroupOfGoodsName));
                        }
                    if (!subGroupCode.Equals(nomenclatureSubGropCode))
                        {
                        AddError(subGroupOfGoodsCode, new SubGroupOfGoodsError(subGroupCode, nomenclatureSubGropCode, subGroupOfGoodsCode));
                        }
                    }
                }
            else
                {
                if (!string.IsNullOrEmpty(subGroupName) && !string.IsNullOrEmpty(subGroupCode)
                    && dbCache.GetSubGroupId(groupName, subGroupName, subGroupCode) == 0)
                    {
                    AddError(subGroupOfGoodsName, new SubGroupOfGoodsError());
                    AddError(subGroupOfGoodsCode, new SubGroupOfGoodsError());
                    }
                }
            }
        }
    }
