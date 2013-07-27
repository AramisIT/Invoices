using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.SubGroupOfGoodsCache
    {
    /// <summary>
    /// Используется для кеширования подгрупп товара
    /// </summary>
    public class SubGroupOfGoodsCacheObject : CacheObject<SubGroupOfGoodsCacheObject>
        {

        public readonly long GroupId;
        public readonly string Code;
        public readonly string Name;

        public readonly long TradeMark;
        public readonly long Contractor;
        public readonly long Manufacturer;

        public SubGroupOfGoodsCacheObject(long groupId, string code, long tradeMark, long contractor, long manufacturer, string name)
            {
            this.GroupId = groupId;
            this.Code = code;
            this.TradeMark = tradeMark;
            this.Contractor = contractor;
            this.Manufacturer = manufacturer;
            this.Name = name;
            }

        public SubGroupOfGoodsCacheObject( long groupId, string code, string name )
            : this( groupId, code, 0, 0, 0, name )
            {
            }

        protected override bool equals( SubGroupOfGoodsCacheObject other )
            {
            return Code.Equals( other.Code ) && Name.Equals( other.Name ) && GroupId.Equals( other.GroupId );
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] { Code, Name, GroupId };
            }
        }
    }
