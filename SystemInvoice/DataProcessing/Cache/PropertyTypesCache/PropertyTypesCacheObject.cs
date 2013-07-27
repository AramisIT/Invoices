using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Используется для кеширования информации о свойствах товара
    /// </summary>
    public class PropertyTypesCacheObject : CacheObject<PropertyTypesCacheObject>, IPropertyTypeFullSearch
        {
        public long NomenclatureId { get; private set; }
        public long SubGroupOfGoodsId { get; private set; }
        public long TypeOfPropertyId { get; private set; }
        public string PropertyEnValue { get; private set; }
        public readonly string PropertyCode;
        public readonly string PropertyUkrValue;
        public readonly string InsoleLength;

        public long GroupOfGoodsId { get; private set; }
        public long TradeMarkId { get; private set; }
        public long ContractorId { get; private set; }

        public PropertyTypesCacheObject(long nomenclatureId, long SubGroupOfGoodsId, long typeOfPropertyId,
                                        string propertyUkrValue, string propertyEnValue, string propertyCode,
                                        long groupOfGoodsId, long tradeMarkId, long contractorId,string insoleLength)
            {
            this.NomenclatureId = nomenclatureId;
            this.SubGroupOfGoodsId = SubGroupOfGoodsId;
            this.TypeOfPropertyId = typeOfPropertyId;
            this.PropertyUkrValue = propertyUkrValue;
            this.PropertyEnValue = propertyEnValue;
            this.PropertyCode = propertyCode;

            this.GroupOfGoodsId = groupOfGoodsId;
            this.TradeMarkId = tradeMarkId;
            this.ContractorId = contractorId;
            this.InsoleLength = insoleLength;
            }

        protected override bool equals(PropertyTypesCacheObject other)
            {
            return NomenclatureId == other.NomenclatureId && SubGroupOfGoodsId == other.SubGroupOfGoodsId &&
                   TypeOfPropertyId == other.TypeOfPropertyId &&
                   PropertyEnValue.ToUpper().Equals(other.PropertyEnValue.ToUpper())
                   && PropertyCode.Equals(other.PropertyCode);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[]
                       {NomenclatureId, SubGroupOfGoodsId, TypeOfPropertyId, PropertyEnValue.ToUpper(), PropertyCode};
            }



        private bool isSizeProperty;

        public bool IsSizeProperty
            {
            get { return isSizeProperty; }
            }

        private bool isGenderProperty;

        public bool IsGenderProperty
            {
            get { return isGenderProperty; }
            }

        private string[] getSizeCompareFields()
            {
            return new string[] {PropertyEnValue};
            }

        private object[] getGenderCompareFields()
            {
            return new object[] {SubGroupOfGoodsId, NomenclatureId};
            }

        private bool compareSize(PropertyTypesCacheObject other)
            {
            return PropertyEnValue.Equals(other);
            }

        private bool compareGender(PropertyTypesCacheObject other)
            {
            return other.SubGroupOfGoodsId.Equals(SubGroupOfGoodsId) && other.NomenclatureId.Equals(NomenclatureId);
            }

        void IPropertyTypeFullSearch.SetSearchOptions(long propertyTypeId, string propertyValue, long nomenclatureId,
                                                      long subGroupOfGoodsId, long groupOfGoodsId, long tradeMarkId,
                                                      long contractorId)
            {
            this.NomenclatureId = nomenclatureId;
            this.SubGroupOfGoodsId = subGroupOfGoodsId;
            this.TypeOfPropertyId = propertyTypeId;
            this.PropertyEnValue = propertyValue;
            this.GroupOfGoodsId = groupOfGoodsId;
            this.TradeMarkId = tradeMarkId;
            this.ContractorId = contractorId;
            }
        }
    }
