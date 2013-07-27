using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Excel;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.TradeMark
    {
    /// <summary>
    /// Проверяет соответствие цены значению сохраненному в базе для номенклатуры
    /// </summary>
    public class TradeMarkDocumentChecker : LoadedDocumentCheckerBase
        {
        public TradeMarkDocumentChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            string tradeMarkColumnName = ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME;
            if (rowToCheck == null || !mapper.ContainsKey(tradeMarkColumnName))
                {
                return;
                }
            string tradeMark = Helpers.InvoiceDataRetrieveHelper.GetRowTradeMark(rowToCheck);
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            if (nomenclatureId > 0)
                {
                var nomenclatureCached = dbCache.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureId);
                if (nomenclatureCached != null)
                    {
                    long inNomenclatureTradeMarkID = nomenclatureCached.TradeMarkId;
                    string cachedTradeMark =
                        dbCache.TradeMarkCacheObjectsStore.GetCachedObject(inNomenclatureTradeMarkID).TradeMarkName;
                    if (!cachedTradeMark.Equals(tradeMark))
                        {
                        AddError(tradeMarkColumnName, new TradeMarkCheckError(tradeMark, cachedTradeMark, tradeMarkColumnName));
                        }
                    return;
                    }
                }
            if (dbCache.TradeMarkCacheObjectsStore.GetTradeMarkId(tradeMark) == 0)
                {
                AddError(tradeMarkColumnName, new TradeMarkCheckError());
                }
            }
        }
    }
