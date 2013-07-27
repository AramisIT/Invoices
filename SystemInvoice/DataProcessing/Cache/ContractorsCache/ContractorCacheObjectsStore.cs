using System;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.ContractorsCache
    {
    /// <summary>
    /// Хранилище кешированных данных контрагентов
    /// </summary>
    public class ContractorCacheObjectsStore : CacheObjectsStore<ContractorCacheObject>
        {
        protected override string SelectQuery
            {
            get { return "select Id,LTRIM(RTRIM(Description)) as Description,UseComodityPrices from Contractor where MarkForDeleting = 0;"; }
            }

        protected override ContractorCacheObject createNew(DataRow row)
            {
            string name = row.TryGetColumnValue<string>("Description", "").Trim();
            bool useComodityPrices = row.TrySafeGetColumnValue<bool>("UseComodityPrices", false);
            ContractorCacheObject contractorCacheObject = new ContractorCacheObject(name, useComodityPrices);
            return contractorCacheObject;
            }


        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from Contractor;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from Contractor where MarkForDeleting = 0;"; }
            }
        }
    }
