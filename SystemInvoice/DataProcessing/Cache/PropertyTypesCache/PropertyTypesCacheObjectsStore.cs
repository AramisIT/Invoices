using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Используется для хранения информации о видах свойств
    /// </summary>
    public class PropertyTypesCacheObjectsStore : CacheObjectsStore<PropertyTypesCacheObject>
        {
        /// <summary>
        /// Интерфейс с помощью которого осуществляется установка значений для поиска перевода по всем полям
        /// </summary>
        private readonly IPropertyTypeFullSearch propertyTypeFullSearchState = null;
        /// <summary>
        /// Объект  - запрос поиска с помощью которого осуществляется поиск перевода по всем полям
        /// </summary>
        private readonly PropertyTypesCacheObject propertyTypesSearchObject = null;
        /// <summary>
        /// Используется для получения айдишников типов свойств
        /// </summary>
        private readonly IPredefinedPropertyOfGoods predefinedPropertyOfGoods;
        /// <summary>
        /// Интерфейс с помощью которого осуществляется установка значений для поиска размеров в украинской системе
        /// </summary>
        private ISizeSearch sizeSearchState = null;
        /// <summary>
        /// Интерфейс с помощью которого осуществляется установка значений для поиска пола
        /// </summary>
        private IGenderSearch genderSearchState = null;
        /// <summary>
        /// Объект  - запрос поиска с помощью которого осуществляется поиск размера
        /// </summary>
        private SizePropertyTypeCacheObject sizeSearchObject = null;
        /// <summary>
        /// Используется как null - object.
        /// </summary>
        private readonly string[] emptyCollection = new string[0];
        /// <summary>
        /// Объект - запрос для поиска пола
        /// </summary>
        private GenderPropertyTypeCacheObject genderSearchObject = new GenderPropertyTypeCacheObject(0, string.Empty, string.Empty, 0);
        /// <summary>
        /// Индекс для поиска переводов
        /// </summary>
        private readonly TranslationIndex translationIndex = new TranslationIndex();
        /// <summary>
        /// Индекс для поиска соответствий
        /// </summary>
        private readonly CorrespondanceIndex correspondanceIndex = new CorrespondanceIndex();
        /// <summary>
        /// Индекс для проверки того является ли элемент соответствием для какого-либо другого элемента
        /// </summary>
        private readonly CorrespondantExistedUkrValueIndex correspondanceIndexReversed = new CorrespondantExistedUkrValueIndex();
        /// <summary>
        /// Индекс для поиска элементов по множеству полей - сначала поиск осуществляется по всем полям (Номенклатура, Подгруппа, Группа,Торговая марка,
        /// Контрагент,Значение,Вид свойства), если соответствия не найдено, затем поиск осуществляется по тем же элементам за исключением номенклатуры,
        /// затем за исключением Номенклатуры и Подгруппы и т.д. вплоть до поиска по виду свойства и английскому наименованию
        /// </summary>
        private readonly  FullCorrespondanceIndex fullCorrespondanceIndex = new FullCorrespondanceIndex();


        public PropertyTypesCacheObjectsStore(IPredefinedPropertyOfGoods predefinedPropertyOfGoods)
            {
            this.predefinedPropertyOfGoods = predefinedPropertyOfGoods;
            this.genderSearchState = genderSearchObject;
            this.sizeSearchObject = new SizePropertyTypeCacheObject(0, string.Empty, string.Empty, predefinedPropertyOfGoods.SizePropertyTypeID, string.Empty);
            this.propertyTypesSearchObject = new PropertyTypesCacheObject(0, 0, 0, string.Empty, string.Empty, string.Empty,
                                                                       0, 0, 0,string.Empty);
            this.propertyTypeFullSearchState = propertyTypesSearchObject;
            this.sizeSearchState = sizeSearchObject;
            }

        public override void Refresh()
            {
            base.Refresh();
            predefinedPropertyOfGoods.Refresh();
            }

        protected override string SelectQuery
            {
            get
                {
                return @"
select p.Id as Id,LTRIM(RTRIM(p.Description)) as propName,p.PropertyOfGoods as propertyId,p.SubGroupOfGoods as SubGroupOfGoodsId
,p.Nomenclature as nomenclatureID,LTRIM(RTRIM(p.CodeOfProperty)) as propertyCode,
p.Value as value,p.UkrainianValue as ukrValue,p.InsoleLength as insoleLength,
p.GroupOfGoodsRef as groupOfGoodId,p.TradeMark as tradeMarkId,p.Contractor as contractorId  from PropertyType as p where MarkForDeleting = 0
order by Id desc;";
                }
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from PropertyType;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from PropertyType as p where MarkForDeleting = 0;"; }
            }

        protected override void InitializeIndexes(List<IEqualityComparer<PropertyTypesCacheObject>> indexes)
            {
            indexes.Add(translationIndex);
            indexes.Add(correspondanceIndexReversed);
            indexes.Add(fullCorrespondanceIndex);
            }

        protected override void InitializeOneToManyIndexes(List<IEqualityComparer<PropertyTypesCacheObject>> indexes)
            {
            indexes.Add(correspondanceIndex);
            }

        protected override PropertyTypesCacheObject createNew(System.Data.DataRow row)
            {
            long nomenclatureId = row.TrySafeGetColumnValue<long>("nomenclatureID", 0);
            long SubGroupOfGoodsId = row.TrySafeGetColumnValue<long>("SubGroupOfGoodsId", 0);
            long typeOfPropertyId = row.TrySafeGetColumnValue<long>("propertyId", 0);
            long groupOfGoodsId = row.TrySafeGetColumnValue<long>("groupOfGoodId", 0);
            long tradeMarkId = row.TrySafeGetColumnValue<long>("tradeMarkId", 0);
            long contractorId = row.TrySafeGetColumnValue<long>("contractorId", 0);
            string propertyUkrValue = row.TrySafeGetColumnValue<string>("ukrValue", string.Empty).Trim();
            string propertyEnValue = row.TrySafeGetColumnValue<string>("value", "").Trim();
            string propertyCodeValue = row.TrySafeGetColumnValue<string>("propertyCode", string.Empty).Trim();
            string insoleLength = row.TrySafeGetColumnValue<string>("insoleLength", string.Empty).Trim();
            if (typeOfPropertyId == 0)//if (string.IsNullOrEmpty( propertyUkrValue ) || typeOfPropertyId == 0)
                {
                return null;
                }
            //if (predefinedPropertyOfGoods.SizePropertyTypeID != 0 && typeOfPropertyId.Equals(predefinedPropertyOfGoods.SizePropertyTypeID))
            //    {
            //    return new SizePropertyTypeCacheObject(SubGroupOfGoodsId, propertyEnValue, propertyUkrValue, predefinedPropertyOfGoods.SizePropertyTypeID, insoleLength);
            //    }
            if (predefinedPropertyOfGoods.GenderPropertyTypeID != 0 && typeOfPropertyId.Equals(predefinedPropertyOfGoods.GenderPropertyTypeID))
                {
                return new GenderPropertyTypeCacheObject(SubGroupOfGoodsId, propertyUkrValue, propertyEnValue, predefinedPropertyOfGoods.GenderPropertyTypeID);
                }
            //if (((long)row["Id"]) == 692)
            //    {
            //    int c = 0;
            //    }
            return new PropertyTypesCacheObject(nomenclatureId, SubGroupOfGoodsId, typeOfPropertyId, propertyUkrValue,
                                                propertyEnValue, propertyCodeValue, groupOfGoodsId, tradeMarkId, contractorId,insoleLength);
            }
        
        /// <summary>
        /// Возвращает размер в украинской системе
        /// </summary>
        /// <param name="SubGroupOfGoodsId">Подгруппа</param>
        /// <param name="enSize">Размер в не-украинской системе</param>
        /// <returns>размер</returns>
        public string GetSizeTranslation(long SubGroupOfGoodsId, string enSize)
            {
            return getPropertyTranslation(predefinedPropertyOfGoods.SizePropertyTypeID, SubGroupOfGoodsId, enSize);
            }

        /// <summary>
        /// Получает перевод пола!!! Уже реально может и не используется, нужно проверить есть ли ссылки на вызов методов использующих этот механизм
        /// в каком-либо формате загрузки
        /// </summary>
        public string GetGenderTranslation(long SubGroupOfGoodsId, string originalGender)
            {
            return getPropertyTranslation(predefinedPropertyOfGoods.GenderPropertyTypeID, SubGroupOfGoodsId, originalGender);
            }

        /// <summary>
        /// Возвращает соответствие свойству в какой - либо системе, свойство в украинской системе
        /// </summary>
        /// <param name="propertyType">Вид свойства</param>
        /// <param name="SubGroupOfGoodsId">Подгруппа товара</param>
        /// <param name="originalPropertyValue">Свойство для которого нужно найти соответствующее свойство в украинской системе</param>
        private string getPropertyTranslation(long propertyType, long SubGroupOfGoodsId, string originalPropertyValue)
            {
            long originalObjId = getOriginalObjectId(originalPropertyValue, SubGroupOfGoodsId, propertyType);
            if (originalObjId == 0)
                {
                originalObjId = getOriginalObjectId(originalPropertyValue, 0, propertyType);
                }
            if (originalObjId > 0)
                {
                PropertyTypesCacheObject objectTranslation = GetCachedObject(translationIndex, originalObjId) as PropertyTypesCacheObject;
                if (objectTranslation != null)
                    {
                    return objectTranslation.PropertyUkrValue;
                    }
                }
            return originalPropertyValue;
            }

        /// <summary>
        /// Проверяет входит ли данный размер в список кешированных размеров
        /// </summary>
        /// <param name="SubGroupOfGoodsId">Подгруппа</param>
        /// <param name="enSize">Размер англ.</param>
        public bool ContainsSize(long SubGroupOfGoodsId, string enSize)
            {
            if (setSizeSearchObject(SubGroupOfGoodsId, enSize))
                {
                return GetCachedObject(sizeSearchObject) != null;
                }
            return false;
            }

        /// <summary>
        /// Возвращает пол для определенной подгруппы товара
        /// </summary>
        public string GetGender(long SubGroupOfGoodsId)
            {
            if (setGenderSearchObject(SubGroupOfGoodsId))
                {
                GenderPropertyTypeCacheObject genderCached = GetCachedObject(genderSearchObject) as GenderPropertyTypeCacheObject;
                if (genderCached != null)
                    {
                    return genderCached.GenderUk;
                    }
                }
            return string.Empty;
            }

        /// <summary>
        /// Проверяет задан ли пол для данной подгруппы
        /// </summary>
        public bool ContainsGender(long SubGroupOfGoodsId)
            {
            if (setGenderSearchObject(SubGroupOfGoodsId))
                {
                return GetCachedObject(genderSearchObject) != null;
                }
            return false;
            }

        /// <summary>
        /// Возвращает длинну стельки для подгруппы/размера укр.
        /// </summary>
        public string GetInsoleLength(long subGroupOfGoodsId, string ukrSize)
            {
            if (setSizeSearchObject(subGroupOfGoodsId, ukrSize))
                {
                SizePropertyTypeCacheObject sizeCached = GetCachedObject(sizeSearchObject) as SizePropertyTypeCacheObject;
                if (sizeCached != null)
                    {
                    return sizeCached.InsoleLength;
                    }
                else
                    {
                    if (setSizeSearchObject(0, ukrSize))
                        {
                        sizeCached = GetCachedObject(sizeSearchObject) as SizePropertyTypeCacheObject;
                        if (sizeCached != null)
                            {
                            return sizeCached.InsoleLength;
                            }
                        }
                    }
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает значение в украинской системе
        /// </summary>
        /// <param name="nomenclatureID">Номенклатура</param>
        /// <param name="propertyValue">Значение свойства в другой системе</param>
        /// <param name="SubGroupOfGoodsId">Подгруппа</param>
        /// <param name="propertyTypeID">Вид свойства</param>
        public string GetPropertyUKValue(long nomenclatureID, string propertyValue, long SubGroupOfGoodsId, long propertyTypeID)
            {
            string contentGet = "";
            PropertyTypesCacheObject propTypeCacheObject = new PropertyTypesCacheObject(nomenclatureID,
                                                                                        SubGroupOfGoodsId,
                                                                                        propertyTypeID, string.Empty,
                                                                                        propertyValue.Trim(),
                                                                                        string.Empty
                                                                                        , 0, 0, 0,string.Empty);
            PropertyTypesCacheObject cachedObject = GetCachedObject(propTypeCacheObject);
            //   long id = GetCachedObjectId(cachedObject);//222
            //var cachedHL = GetCachedObject(222);
            //if (cachedHL != null)
            //    {
            //    bool equals = cachedHL.Equals(propTypeCacheObject);
            //    }
            if (cachedObject != null)
                {
                contentGet = cachedObject.PropertyUkrValue;
                }
            return contentGet;
            }
        
        /// <summary>
        /// Возвращает значение в украинской системе
        /// </summary>
        public string GetPropertyUKValue(long propertyTypeId, string propertyValue, long nomenclatureId, long subGroupOfGoodsId, long groupOfGoodsId, long tradeMarkId, long contractorId)
            {
            PropertyTypesCacheObject foundedObject = searchForAppropriateFullSearchObject(propertyTypeId, propertyValue,
                                                                                          nomenclatureId,
                                                                                          subGroupOfGoodsId,
                                                                                          groupOfGoodsId, tradeMarkId,
                                                                                          contractorId);
            if(foundedObject!=null)
                {
                return foundedObject.PropertyUkrValue;
                }
            return string.Empty;
            }

        private PropertyTypesCacheObject searchForAppropriateFullSearchObject(long propertyTypeId, string propertyValue, long nomenclatureId, long subGroupOfGoodsId, long groupOfGoodsId, long tradeMarkId, long contractorId)
            {
            PropertyTypesCacheObject foundedObject = null;
            if (
                (foundedObject =
                 this.getCurrentFullSearchObject(propertyTypeId, propertyValue, nomenclatureId, subGroupOfGoodsId,
                                                 groupOfGoodsId, tradeMarkId, contractorId)) != null)
                {
                return foundedObject;
                }
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, subGroupOfGoodsId,
                                                                 groupOfGoodsId, tradeMarkId, contractorId)) != null)
                {
                return foundedObject;
                }
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, 0,
                                                                 groupOfGoodsId, tradeMarkId, contractorId)) != null)
                {
                return foundedObject;
                }

            //  Иногда может отсутствувать торговая марка с контрагентом но быть группа/подгруппа/номенклатура
            //без торговой марки
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, nomenclatureId, subGroupOfGoodsId, groupOfGoodsId, 0, contractorId)) != null)
                {
                return foundedObject;
                }
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, subGroupOfGoodsId,
                                                                 groupOfGoodsId, 0, contractorId)) != null)
                {
                return foundedObject;
                }
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, 0,
                                                                 groupOfGoodsId, 0, contractorId)) != null)
                {
                return foundedObject;
                }
            //без торговой марки и контрагента
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, nomenclatureId, subGroupOfGoodsId, groupOfGoodsId, 0, 0)) != null)
                {
                return foundedObject;
                }
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, subGroupOfGoodsId,
                                                                 groupOfGoodsId, 0, 0)) != null)
                {
                return foundedObject;
                }
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, 0,
                                                                 groupOfGoodsId, 0, 0)) != null)
                {
                return foundedObject;
                }


            if (
                (foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, 0, 0, tradeMarkId, contractorId)) != null)
                {
                return foundedObject;
                }
            if (
                (foundedObject =
                 this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, 0, 0, 0, contractorId)) != null)
                {
                return foundedObject;
                }
            if ((foundedObject = this.getCurrentFullSearchObject(propertyTypeId, propertyValue, 0, 0, 0, 0, 0)) != null)
                {
                return foundedObject;
                }
            return null;
            }

        private PropertyTypesCacheObject getCurrentFullSearchObject(long propertyTypeId, string propertyValue, long nomenclatureId, long subGroupOfGoodsId, long groupOfGoodsId, long tradeMarkId, long contractorId)
            {
            propertyTypeFullSearchState.SetSearchOptions(propertyTypeId, propertyValue, nomenclatureId, subGroupOfGoodsId,
                                                    groupOfGoodsId, tradeMarkId, contractorId);
            var id = GetCachedObjectId(fullCorrespondanceIndex, propertyTypesSearchObject);

            if (id > 0)
                {
                return GetCachedObject(fullCorrespondanceIndex, id);
                }
            return null;
            }



        /// <summary>
        /// Проверяет наличие свойства в кэше
        /// </summary>
        public bool ContainsProperty(long nomenclatureId, string propertyValue, long SubGroupOfGoodsId, long propertyTypeID, string codeOfProperty)
            {
            PropertyTypesCacheObject propTypeCacheObject =
                new PropertyTypesCacheObject(nomenclatureId, SubGroupOfGoodsId, propertyTypeID, string.Empty, propertyValue.Trim(), codeOfProperty.Trim(),0,0,0,string.Empty);
            return GetCachedObject(propTypeCacheObject) != null;
            }

        /// <summary>
        /// Устанавливает значения для поиска размера
        /// </summary>
        private bool setSizeSearchObject(long SubGroupOfGoodsId, string ukSize)
            {
            if (string.IsNullOrEmpty(ukSize.Trim()))
                {
                return false;
                }
            sizeSearchState.SetSearchOptions(SubGroupOfGoodsId, ukSize);
            return true;
            }

        /// <summary>
        /// Устанавливает значения для поиска пола
        /// </summary>
        private bool setGenderSearchObject(long SubGroupOfGoodsId)
            {
            if (SubGroupOfGoodsId == 0)
                {
                return false;
                }
            genderSearchState.SetSearchOptions(SubGroupOfGoodsId);
            return true;
            }

        /// <summary>
        /// Поиск кешированного объекта
        /// </summary>
        /// <param name="enSize">Значение</param>
        /// <param name="subGroupId">Подгруппа</param>
        /// <param name="propertyType">Id вида свойств</param>
        /// <returns>Id кешированного объекта, 0 - если объект не найден </returns>
        private long getOriginalObjectId(string enSize, long subGroupId, long propertyType)
            {
            PropertyTypesCacheObject searchObj = new PropertyTypesCacheObject(0, subGroupId, propertyType, string.Empty, enSize, string.Empty,0,0,0,string.Empty);
            return GetCachedObjectId(translationIndex, searchObj);
            }

        /// <summary>
        /// Находит соответствующие значения элементов для ключевого - элемента (ключевой элемент хранится в базе там же где и свойство в английской системе) 
        /// </summary>
        public IEnumerable<string> GetCorrespondingElements(string elementName)
            {
            //if (elementName.Contains("ОКУ"))
            //    {
            //    string s = "";
            //    }
            PropertyTypesCacheObject searchObject = new PropertyTypesCacheObject(0, 0,
                                                                                 predefinedPropertyOfGoods.
                                                                                     CorrespondenceTypeId, string.Empty,
                                                                                 elementName, string.Empty, 0, 0, 0,string.Empty);
            IEnumerable<long> founded = GetCachedObjectIds(correspondanceIndex, searchObject);
            if (founded == null)
                {
                return emptyCollection;
                }
            List<string> items = new List<string>();
            foreach (long id in founded)
                {
                PropertyTypesCacheObject foundedItem = base.GetFromOneToManyCachedObject(correspondanceIndex, id);
                if (foundedItem != null)
                    {
                    items.Add(foundedItem.PropertyUkrValue);
                    }
                }
            return items;
            }

        /// <summary>
        /// Проверяет является ли элемент соответствием для какого либа другого элемента
        /// </summary>
        public bool ContainsInCorrespondingValues(string checkingWord)
            {
            PropertyTypesCacheObject searchObject =
                new PropertyTypesCacheObject(0, 0, predefinedPropertyOfGoods.CorrespondenceTypeId, checkingWord, string.Empty, string.Empty,0,0,0,string.Empty);
            return GetCachedObjectId(correspondanceIndexReversed, searchObject) > 0;
            }

        /// <summary>
        /// Получает значение свойства "наименование" для подгруппы
        /// </summary>
        /// <param name="subGroupId">Id подгруппы</param>
        public string GetName(long subGroupId)
            {
            if (subGroupId > 0)
                {
                PropertyTypesCacheObject propTypeCacheObject = new PropertyTypesCacheObject(0, subGroupId,
                                                                                            predefinedPropertyOfGoods.NamePropertyTypeId,
                                                                                            string.Empty, string.Empty,
                                                                                            string.Empty, 0, 0, 0,string.Empty);
                PropertyTypesCacheObject cachedObject = GetCachedObject(propTypeCacheObject);
                if (cachedObject != null)
                    {
                    return cachedObject.PropertyUkrValue;
                    }
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает размер в украинской системе
        /// </summary>
        public string GetSizeTranslation(string sizeEn, long groupId, long subGroupId, long contractorId, long trademarkId, long nomenclatureId)
            {
            long sizePropertyId = predefinedPropertyOfGoods.SizePropertyTypeID;

            PropertyTypesCacheObject foundedObject = searchForAppropriateFullSearchObject(sizePropertyId, sizeEn,
                                                                                          nomenclatureId,
                                                                                          subGroupId,
                                                                                          groupId, trademarkId,
                                                                                          contractorId);
            if (foundedObject != null)
                {
                return foundedObject.PropertyUkrValue;
                }
            return string.Empty;
            }
        
        /// <summary>
        /// Возвращает длинну стельки
        /// </summary>
        public string GetInsoleLength(string sizeEn, long groupId, long subGroupId, long contractorId, long trademarkId, long nomenclatureId)
            {
            long sizePropertyId = predefinedPropertyOfGoods.SizePropertyTypeID;

            PropertyTypesCacheObject foundedObject = searchForAppropriateFullSearchObject(sizePropertyId, sizeEn,
                                                                                          nomenclatureId,
                                                                                          subGroupId,
                                                                                          groupId, trademarkId,
                                                                                          contractorId);
            if (foundedObject != null)
                {
                if (!string.IsNullOrEmpty(foundedObject.InsoleLength))
                    {
                    int l = 0;
                    }
                return foundedObject.InsoleLength;
                }
            return string.Empty;
            }
        }
    }
