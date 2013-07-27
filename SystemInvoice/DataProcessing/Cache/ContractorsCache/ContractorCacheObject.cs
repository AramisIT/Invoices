using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.ContractorsCache
    {
    /// <summary>
    /// Используется для кеширования контрагентов, что необходимо для быстрого поиска  торговых марок, номенклатуры, производителя и других справочников,
    ///  в поиске которых используется имя контрагента 
    /// </summary>
    public class ContractorCacheObject : CacheObject<ContractorCacheObject>
        {
        public readonly string ContractorName;

        public bool UseComodityPrices { get; private set; }

        public ContractorCacheObject(string contractorName, bool useComodityPrices)
            {
            this.ContractorName = contractorName;
            this.UseComodityPrices = useComodityPrices;
            }

        protected override bool equals(ContractorCacheObject other)
            {
            return other.ContractorName.Equals(ContractorName);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { ContractorName };
            }
        }
    }
