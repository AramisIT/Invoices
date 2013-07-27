using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.PropertyOfGoodsCache
    {
    /// <summary>
    /// Используется для кеширования типов свойств товара
    /// </summary>
    public class PropertyOfGoodsCacheObject : CacheObject<PropertyOfGoodsCacheObject>
        {
        public readonly string PropertyName;

        public PropertyOfGoodsCacheObject(string propertyName)
            {
            this.PropertyName = propertyName;
            }

        protected override bool equals(PropertyOfGoodsCacheObject other)
            {
            return PropertyName.Equals(other.PropertyName);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { PropertyName };
            }
        }
    }
