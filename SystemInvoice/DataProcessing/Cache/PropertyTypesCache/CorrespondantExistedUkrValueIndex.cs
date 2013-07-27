using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Индекс для быстрой проверки элемента на соответствие какому - либо другому елементу
    /// </summary>
    public class CorrespondantExistedUkrValueIndex : IEqualityComparer<PropertyTypesCacheObject>
        {
        public bool Equals(PropertyTypesCacheObject x, PropertyTypesCacheObject y)
            {
            return x.PropertyUkrValue == y.PropertyUkrValue && x.TypeOfPropertyId == y.TypeOfPropertyId;
            }

        public int GetHashCode(PropertyTypesCacheObject obj)
            {
            return obj.PropertyUkrValue.GetHashCode() ^ obj.TypeOfPropertyId.GetHashCode();
            }
        }
    }
