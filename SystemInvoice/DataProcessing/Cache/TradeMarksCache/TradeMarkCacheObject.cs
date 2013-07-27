using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.TradeMarksCache
    {
    /// <summary>
    /// Используется для кеширования торговых марок
    /// </summary>
    public class TradeMarkCacheObject : CacheObject<TradeMarkCacheObject>, ITradeMarkSearch
        {
        public string TradeMarkName { get; private set; }
        public long ContractorId { get; private set; }

        public TradeMarkCacheObject(string tradeMarkName, long contractorID)
            {
            this.TradeMarkName = tradeMarkName;
            this.ContractorId = contractorID;
            }

        protected override bool equals(TradeMarkCacheObject other)
            {
            return ContractorId == other.ContractorId && TradeMarkName.Equals(other.TradeMarkName);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] { TradeMarkName, ContractorId };
            }

        protected override int calcHash()
            {
            return TradeMarkName.GetHashCode() ^ ContractorId.GetHashCode();
            }

        #region Реализация ITradeMarkSearch

        void ITradeMarkSearch.SetSearchOptions(string tradeMarkName, long contractorId)
            {
            if (string.IsNullOrEmpty(tradeMarkName))
                {
                return;
                }
            this.TradeMarkName = tradeMarkName;
            this.ContractorId = contractorId;
            refreshHash();
            }

        #endregion

        }
    }
