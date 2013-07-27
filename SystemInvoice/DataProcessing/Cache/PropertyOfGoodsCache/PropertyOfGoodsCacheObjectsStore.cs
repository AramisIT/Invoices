using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyOfGoodsCache
    {
    /// <summary>
    /// Используется для хранения информации о видах свойств товара
    /// </summary>
    public class PropertyOfGoodsCacheObjectsStore : CacheObjectsStore<PropertyOfGoodsCacheObject>, IPredefinedPropertyOfGoods
        {
        private long genderPropertyTypeId;
        private long sizePropertyTypeID;
        private long correspondenceTypeId;
        private long namePropertyTypeId;

        public override void Refresh()
            {
            base.Refresh();
            genderPropertyTypeId = GetCachedObjectId("Пол");
            sizePropertyTypeID = GetCachedObjectId("Размер");
            correspondenceTypeId = GetCachedObjectId("Соответствие");
            namePropertyTypeId = GetCachedObjectId("Наименование");
            }

        protected override string SelectQuery
            {
            get { return "select LTRIM(RTRIM(Description)) as name,Id from PropertyOfGoods where MarkForDeleting = 0;"; }
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from PropertyOfGoods;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from PropertyOfGoods where MarkForDeleting = 0;"; }
            }

        protected override PropertyOfGoodsCacheObject createNew(System.Data.DataRow row)
            {
            string name = row.TryGetColumnValue<string>("name", "");
            return new PropertyOfGoodsCacheObject(name);
            }

        /// <summary>
        /// Возвращает Id свойства товара по наименованию свойства (Пол, Размер, и т.д.)
        /// </summary>
        /// <param name="propertyName">Наименование свойства</param>
        /// <returns>Id свойства</returns>
        public long GetCachedObjectId(string propertyName)
            {
            PropertyOfGoodsCacheObject searched = new PropertyOfGoodsCacheObject(propertyName.Trim());
            return GetCachedObjectId(searched);
            }

        #region Реализация IPredefinedPropertyOfGoods

        public long GenderPropertyTypeID
            {
            get { return genderPropertyTypeId; }
            }

        public long SizePropertyTypeID
            {
            get { return sizePropertyTypeID; }
            }


        public long CorrespondenceTypeId
            {
            get { return correspondenceTypeId; }
            }

        public long NamePropertyTypeId
            {
            get { return namePropertyTypeId; }
            } 

        #endregion
        }
    }
