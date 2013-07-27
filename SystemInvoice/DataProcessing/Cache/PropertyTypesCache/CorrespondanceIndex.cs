using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Индекс для быстрого поиска нахождения соответствия множества элементов одному элементу элементу с одинаковыми видами типов свойств. Элемент по которому находится соответствие 
    /// используется при поиске множества.
    /// </summary>
    public class CorrespondanceIndex : IEqualityComparer<PropertyTypesCacheObject>
        {
        public bool Equals(PropertyTypesCacheObject x, PropertyTypesCacheObject y)
            {
            return x.PropertyEnValue.ToUpper() == y.PropertyEnValue.ToUpper() && x.TypeOfPropertyId == y.TypeOfPropertyId;
            }

        public int GetHashCode(PropertyTypesCacheObject obj)
            {
            return obj.PropertyEnValue.ToUpper().GetHashCode() ^ obj.TypeOfPropertyId.GetHashCode();
            }
        }
    }
