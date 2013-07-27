using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Индекс используемый для получения перевода значения на основании значения - ключа и подгруппы
    /// </summary>
    public class TranslationIndex : IEqualityComparer<PropertyTypesCacheObject>
        {
        public bool Equals( PropertyTypesCacheObject x, PropertyTypesCacheObject y )
            {
            return x.PropertyEnValue == y.PropertyEnValue && x.SubGroupOfGoodsId == y.SubGroupOfGoodsId && x.TypeOfPropertyId == y.TypeOfPropertyId;
            }

        public int GetHashCode( PropertyTypesCacheObject obj )
            {
            return obj.PropertyEnValue.GetHashCode() ^ obj.SubGroupOfGoodsId.GetHashCode() ^ obj.TypeOfPropertyId.GetHashCode();
            }
        }
    }
