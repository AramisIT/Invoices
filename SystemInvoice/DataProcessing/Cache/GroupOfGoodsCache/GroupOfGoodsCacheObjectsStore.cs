using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.GroupOfGoodsCache
    {
    /// <summary>
    /// Хранилище кэшированных групп товаров используется к примеру для поиска айдишника группы товара по имени
    /// </summary>
    public class GroupOfGoodsCacheObjectsStore : CacheObjectsStore<GroupOfGoodsCacheObject>
        {
        protected override string SelectQuery
            {
            get { return @"select Id,LTRIM(RTRIM(Description)) as subGroupName from GroupOfGoods where MarkForDeleting = 0;"; }
            }
        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from GroupOfGoods;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from GroupOfGoods where MarkForDeleting = 0;"; }
            }

        protected override GroupOfGoodsCacheObject createNew( System.Data.DataRow row )
            {
            string groupName = row.TrySafeGetColumnValue<string>( "subGroupName", string.Empty );
            GroupOfGoodsCacheObject newObject = new GroupOfGoodsCacheObject( groupName );
            return newObject;
            }

        /// <summary>
        /// Возвращает айдишник группы товара по ее имени
        /// </summary>
        /// <param name="groupOfGoodsName">имя группы</param>
        /// <returns>Id группы</returns>
        public long GetGroupOfGoodsId( string groupOfGoodsName )
            {
            if (string.IsNullOrEmpty( groupOfGoodsName.Trim() ))
                {
                return 0;
                }
            GroupOfGoodsCacheObject searchObject = new GroupOfGoodsCacheObject( groupOfGoodsName.Trim() );
            return GetCachedObjectId( searchObject );
            }

        }
    }
