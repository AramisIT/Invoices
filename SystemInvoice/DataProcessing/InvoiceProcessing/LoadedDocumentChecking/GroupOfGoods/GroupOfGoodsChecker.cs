using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.GroupOfGoods
    {
    /// <summary>
    /// Проверяет соответствие группы товара значению сохраненному в базе
    /// </summary>
    public class GroupOfGoodsChecker : NomenclatureChecker
        {
        public GroupOfGoodsChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override bool CheckThatEquals(Cache.NomenclaturesCache.NomenclatureCacheObject nomenclatureCacheObject, string expectededValue)
            {
            string inDB = dbCache.GetNomenclatureGroupName(nomenclatureCacheObject);
            if (inDB != null && expectededValue != null)
                {
                return inDB.Equals(expectededValue);
                }
            return false;
            }

        protected override string ColumnToCheck
            {
            get { return ProcessingConsts.ColumnNames.GROUP_OF_GOODS_COLUMN_NAME; }
            }

        protected override bool CheckExpectedValue(string expectedValue, ExcelMapper mapper)
            {
            if (mapper.ContainsKey(ProcessingConsts.ColumnNames.GROUP_OF_GOODS_COLUMN_NAME))
                {
                return dbCache.GroupOfGoodsStore.GetGroupOfGoodsId(expectedValue) != 0;
                }
            return true;
            }

        protected override CellError CreateError(string expectedValue, Cache.NomenclaturesCache.NomenclatureCacheObject nomenclatureCacheObject)
            {
            if (nomenclatureCacheObject == null)
                {
                return null;
                }
            return new SubGroupOfGoods.SubGroupOfGoodsError(expectedValue, dbCache.GetNomenclatureGroupName(nomenclatureCacheObject), ProcessingConsts.ColumnNames.GROUP_OF_GOODS_COLUMN_NAME);
            }
        }
    }
