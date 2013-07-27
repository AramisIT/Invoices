using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Индекс для поиска соответствия элементов по всей группе полей - Номенклатура,Подгруппа,Группа,Торговая Марка,Контрагент,Значение
    /// </summary>
    class FullCorrespondanceIndex : IEqualityComparer<PropertyTypesCacheObject>
        {
        public bool Equals(PropertyTypesCacheObject x, PropertyTypesCacheObject y)
            {
            return x.TypeOfPropertyId.Equals(y.TypeOfPropertyId) && x.PropertyEnValue.Equals(y.PropertyEnValue)
                   && x.NomenclatureId.Equals(y.NomenclatureId) && x.SubGroupOfGoodsId.Equals(y.SubGroupOfGoodsId)
                   && x.GroupOfGoodsId.Equals(y.GroupOfGoodsId) && x.TradeMarkId.Equals(y.TradeMarkId)
                   && x.ContractorId.Equals(y.ContractorId);
            }

        public int GetHashCode(PropertyTypesCacheObject obj)
            {
            return obj.TypeOfPropertyId.GetHashCode() ^ obj.PropertyEnValue.GetHashCode() ^
                   obj.NomenclatureId.GetHashCode() ^ obj.SubGroupOfGoodsId.GetHashCode() ^
                   obj.GroupOfGoodsId.GetHashCode() ^ obj.TradeMarkId.GetHashCode() ^
                   obj.ContractorId.GetHashCode();
            }
        }
    }
