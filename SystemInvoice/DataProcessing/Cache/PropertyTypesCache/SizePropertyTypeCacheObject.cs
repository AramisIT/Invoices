using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Используется для кеширования размеров
    /// </summary>
    public class SizePropertyTypeCacheObject : PropertyTypesCacheObject, ISizeSearch
        {
        public long SubGroupOfGoodsId { get; private set; }
        public string SizeEn { get; private set; }
        public string SizeUk { get; private set; }
        public string InsoleLength { get; private set; }

        public SizePropertyTypeCacheObject(long groupId, string sizeEn, string sizeUk, long typeOfPropertyId, string insoleLength)
            : base(0, groupId, typeOfPropertyId, sizeUk, sizeEn, string.Empty, 0, 0, 0,string.Empty)
            {
            this.SizeEn = sizeEn;
            this.SizeUk = sizeUk;
            this.SubGroupOfGoodsId = groupId;
            this.InsoleLength = insoleLength;
            }

        protected override bool equals(PropertyTypesCacheObject other)
            {
            SizePropertyTypeCacheObject otherSize = other as SizePropertyTypeCacheObject;
            if (otherSize != null)
                {
                return SizeUk.Equals(otherSize.SizeUk) && this.SubGroupOfGoodsId.Equals(otherSize.SubGroupOfGoodsId);
                }
            return false;
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] { SubGroupOfGoodsId, SizeUk };
            }

        protected override int calcHash()
            {
            return SubGroupOfGoodsId.GetHashCode() ^ SizeUk.GetHashCode();
            }

        #region Реализация ISizeSearch

        void ISizeSearch.SetSearchOptions(long SubGroupOfGoodsId, string ukSize)
            {
            this.SubGroupOfGoodsId = SubGroupOfGoodsId;
            // this.SizeEn = enSize;
            this.SizeUk = ukSize;
            refreshHash();
            }

        #endregion

        }
    }
