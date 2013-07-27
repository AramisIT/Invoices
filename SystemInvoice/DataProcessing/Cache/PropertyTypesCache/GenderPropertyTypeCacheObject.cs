using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Используется для кеширования информации о соответствии пола к группе товара
    /// </summary>
    public class GenderPropertyTypeCacheObject : PropertyTypesCacheObject, IGenderSearch
        {
        public long SubGroupOfGoodsId { get; private set; }
        public readonly string GenderUk;

        public GenderPropertyTypeCacheObject(long groupId, string genderUk, string genderOriginal, long propertyTypeId)
            : base(0, groupId, propertyTypeId, genderUk, genderOriginal, string.Empty,0,0,0,string.Empty)
            {
            this.SubGroupOfGoodsId = groupId;
            this.GenderUk = genderUk;
            }

        protected override bool equals(PropertyTypesCacheObject other)
            {
            GenderPropertyTypeCacheObject otherGender = other as GenderPropertyTypeCacheObject;
            if (otherGender != null)
                {
                return SubGroupOfGoodsId.Equals(otherGender.SubGroupOfGoodsId);
                }
            return false;
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] { SubGroupOfGoodsId };
            }
        protected override int calcHash()
            {
            return SubGroupOfGoodsId.GetHashCode();
            }

        #region Реализация IGenderSearch

        void IGenderSearch.SetSearchOptions(long SubGroupOfGoodsId)
            {
            this.SubGroupOfGoodsId = SubGroupOfGoodsId;
            refreshHash();
            }

        #endregion
        }
    }
