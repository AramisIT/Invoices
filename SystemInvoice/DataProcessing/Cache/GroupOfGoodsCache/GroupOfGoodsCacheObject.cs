using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.GroupOfGoodsCache
    {
    /// <summary>
    /// Используется для кеширования групп товаров
    /// </summary>
    public class GroupOfGoodsCacheObject : CacheObject<GroupOfGoodsCacheObject>
        {
        public readonly string GroupName;

        public GroupOfGoodsCacheObject(string groupName)
            {
            this.GroupName = groupName;
            }

        protected override bool equals(GroupOfGoodsCacheObject other)
            {
            return GroupName.Equals(other.GroupName);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { GroupName };
            }
        }
    }
