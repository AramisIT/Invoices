using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.DataProcessing.Cache.SubGroupOfGoodsCache
    {
    /// <summary>
    /// Используется для хранения информации о подгруппах товара
    /// </summary>
    public class SubGroupOfGoodsCacheObjectsStore : CacheObjectsStore<SubGroupOfGoodsCacheObject>
        {
        /// <summary>
        /// Источник информации о торговой марке/контрагенте используемых при построении кеша
        /// </summary>
        private ITradeMarkContractorSource tradeMarkContractorSource = null;

        public SubGroupOfGoodsCacheObjectsStore( ITradeMarkContractorSource tradeMarkContractorSource )
            {
            this.tradeMarkContractorSource = tradeMarkContractorSource;
            }

        protected override string SelectQuery
            {
            get
                {
                return @"select Ltrim(RTRIM(GroupCode)) as subGroupCode,Contractor as contractorId,TradeMark as tradeMarkId,
Manufacturer as manufacturerId,Ltrim(Rtrim(Description)) as name,Id,GroupOfGoods as groupId from SubGroupOfGoods where MarkForDeleting = 0;";
                }
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from SubGroupOfGoods;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from SubGroupOfGoods where MarkForDeleting = 0;"; }
            }

        protected override SubGroupOfGoodsCacheObject createNew( System.Data.DataRow row )
            {
            string groupCodeStr = row.TrySafeGetColumnValue<string>( "subGroupCode", "" );
            long contractorId = row.TrySafeGetColumnValue<long>( "contractorId", -1 );
            long tradeMarkId = row.TrySafeGetColumnValue<long>( "tradeMarkId", -1 );
            long manufacturerId = row.TrySafeGetColumnValue<long>( "manufacturerId", -1 );
            long groupId = row.TrySafeGetColumnValue<long>( "groupId", -1 );
            string name = row.TrySafeGetColumnValue<string>( "name", "" );
            return new SubGroupOfGoodsCacheObject( groupId, groupCodeStr, tradeMarkId, contractorId, manufacturerId, name );
            }

        public SubGroupOfGoodsCacheObject CreateNew( long groupID, string groupName, string groupCode, long tradeMarkId, long manufacturerId )
            {
            if (tradeMarkContractorSource == null || tradeMarkContractorSource.Contractor.Id == 0)
                {
                return null;
                }
            return new SubGroupOfGoodsCacheObject( groupID, groupCode, tradeMarkId, tradeMarkContractorSource.Contractor.Id, manufacturerId, groupName );
            }

        }
    }
