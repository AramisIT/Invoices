using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache.MaterialsMappingCache;
using SystemInvoice.DataProcessing.Cache.NomenclatureRemovingFromApprovalsHistoryCache;
using Aramis.Core;
using System.Threading.Tasks;
using SystemInvoice.PropsSyncronization;
using Aramis.DatabaseConnector;
using SystemInvoice.DataProcessing.Cache.ContractorsCache;
using SystemInvoice.DataProcessing.Cache.CustomCodesCache;
using SystemInvoice.DataProcessing.Cache.SubGroupOfGoodsCache;
using SystemInvoice.DataProcessing.Cache.ManufacturersCache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.DataProcessing.Cache.PropertyOfGoodsCache;
using SystemInvoice.DataProcessing.Cache.PropertyTypesCache;
using SystemInvoice.DataProcessing.Cache.TradeMarksCache;
using SystemInvoice.DataProcessing.Cache.UnitOfMeasureCache;
using SystemInvoice.DataProcessing.Cache.CountryCache;
using SystemInvoice.DataProcessing.Cache.GroupOfGoodsCache;
using SystemInvoice.DataProcessing.Cache.DocumentTypesCache;
using SystemInvoice.DataProcessing.Cache.ApprovalsCache;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.Cache
    {
    /// <summary>
    /// Класс - фасад, для доступа к подсистеме кеширования и создания объектов БД
    /// </summary>
    public class SystemInvoiceDBCache
        {
        #region Статические кеши - синглетоны, которые хранят данные независимо от контрагента/торговой марки обрабатываемых системой, постоянно висят в памяти но не требуют пересоздания при обработке нового документа

        private static CustomsCodesCacheObjectsStore customsCodesSingletonInstance = null;
        private static ContractorCacheObjectsStore contractorCacheObjectsStore = null;
        private static PropertyOfGoodsCacheObjectsStore propertyOfGoodsCacheObjectsStore = null;
        private static PropertyTypesCacheObjectsStore propertyTypesCacheObjectsStore = null;
        private static CountryCahceObjectsStore countryCahceObjectsStore = null;
        private static DocumentTypesCacheObjectsStore documentTypesCacheObjectStore = null;
        private static MaterialsMappingCacheObjectsStore materialsMappingCacheObjectsStore = null;

        static SystemInvoiceDBCache()
            {
            customsCodesSingletonInstance = new CustomsCodesCacheObjectsStore();
            contractorCacheObjectsStore = new ContractorCacheObjectsStore();
            propertyOfGoodsCacheObjectsStore = new PropertyOfGoodsCacheObjectsStore();
            propertyTypesCacheObjectsStore = new PropertyTypesCacheObjectsStore(propertyOfGoodsCacheObjectsStore);
            countryCahceObjectsStore = new CountryCahceObjectsStore();
            documentTypesCacheObjectStore = new DocumentTypesCacheObjectsStore();
            materialsMappingCacheObjectsStore = new MaterialsMappingCacheObjectsStore();
            }

        #endregion

        #region Кеши уровня екземпляра класа
        /// <summary>
        /// Возвращает кешированное хранилище подгрупп товаров
        /// </summary>
        public readonly SubGroupOfGoodsCacheObjectsStore SubGroupOfGoodsCacheObjectsStore;
        /// <summary>
        /// Возвращает кешированное хранилище производителей
        /// </summary>
        public readonly ManufacturerCacheObjectsStore ManufacturerCacheObjectsStore;
        /// <summary>
        /// Возвращает кешированное хранилище номенклатуры
        /// </summary>
        public readonly NomenclatureCacheObjectsStore NomenclatureCacheObjectsStore;
        /// <summary>
        /// Возвращает кешированное хранилище торговых марок
        /// </summary>
        public readonly TradeMarkCacheObjectsStore TradeMarkCacheObjectsStore;
        /// <summary>
        /// Возвращает кешированное хранилище единиц измерения
        /// </summary>
        public readonly UnitOfMeasureCacheObjectsStore UnitOfMeasureCacheObjectsStore;
        /// <summary>
        /// Возвращает кешированное хранилище груп товаров
        /// </summary>
        public readonly GroupOfGoodsCacheObjectsStore GroupOfGoodsStore;
        /// <summary>
        /// Возвращает кешированное хранилище РД. Для получения Разрешительных необходимо в начале обновить кеш необходимым диапазоном дат, а так же указать контрагента, и при необходимости торговую марку
        /// </summary>
        public readonly ApprovalsCacheObjectsStore ApprovalsCacheObjectsStore;
        /// <summary>
        /// Возвращает кешированное хранилище записей об удаленных из РД номенклатурах. Для получения данных необходимо в начале обновить кеш необходимым диапазоном дат, а так же указать контрагента, и при необходимости торговую марку
        /// </summary>
        public readonly NomenclatureRemovingHistoryCacheObjectsStore NomenclatureRemovingHistoryCacheObjectsStore;

        #endregion

        #region Класы - создатели элементов документов/справочников
        public readonly TradeMarkObjectsCreator TradeMarksCreator;
        public readonly ManufacturersObjectsCreator ManufacturersCreator;
        public readonly NomenclatureObjectsCreator NomenclatureCreator;
        public readonly SubGroupOfGoodsObjectsCreator SubGroupOfGoodsObjectsCreator;
        public readonly GroupOfGoodsObjectsCreator GroupOfGoodsCreator;
        public readonly ApprovalsObjectCreator ApprovalsObjectCreator;
        #endregion

        private ITradeMarkContractorSource tradeMarkContractorSource = null;

        /// <summary>
        /// Возвращает используемые при построении кеша торговую марку/контрагента
        /// </summary>
        public ITradeMarkContractorSource TradeMarkContractorSource
            {
            get
                {
                return tradeMarkContractorSource;
                }
            }

        /// <summary>
        /// Возвращает кешированное хранилище контрагентов
        /// </summary>
        public ContractorCacheObjectsStore ContractorCacheObjectsStore
            {
            get { return contractorCacheObjectsStore; }
            }

        /// <summary>
        /// Возвращает кешированное хранилище таможенных кодов
        /// </summary>
        public CustomsCodesCacheObjectsStore CustomsCodesCacheStore
            {
            get { return customsCodesSingletonInstance; }
            }

        /// <summary>
        /// Возвращает кешированное хранилище видов свойств товаров (Состав, размер,...)
        /// </summary>
        public PropertyOfGoodsCacheObjectsStore PropertyOfGoodsCacheObjectsStore
            {
            get { return propertyOfGoodsCacheObjectsStore; }
            }

        /// <summary>
        /// Возвращает кешированное хранилище свойств товаров (Вообще говоря нужно было бы изначально поменять название местами с PropertyOfGoodsCacheObjectsStore)
        /// </summary>
        public PropertyTypesCacheObjectsStore PropertyTypesCacheObjectsStore
            {
            get
                {
                return propertyTypesCacheObjectsStore;
                }
            }

        /// <summary>
        /// Возвращает кешированное хранилище стран
        /// </summary>
        public CountryCahceObjectsStore CountryCahceObjectsStore
            {
            get
                {
                return countryCahceObjectsStore;
                }
            }

        /// <summary>
        /// Возвращает кешированное хранилище типов документов
        /// </summary>
        public DocumentTypesCacheObjectsStore DocumentTypesCacheObjectsStore
            {
            get
                {
                return documentTypesCacheObjectStore;
                }
            }

        /// <summary>
        /// Возвращает екземпляр класа позволяющий получить доступ к айдишникам определенных свойств товаров
        /// </summary>
        private static IPredefinedPropertyOfGoods PredefinedPropertyOfGoods
            {
            get
                {
                return propertyOfGoodsCacheObjectsStore;
                }
            }

        /// <summary>
        /// Возвращает кешированное хранилище типов материалов
        /// </summary>
        public MaterialsMappingCacheObjectsStore MaterialsMappingCacheObjectsStore
            {
            get { return materialsMappingCacheObjectsStore; }
            }

        public SystemInvoiceDBCache(ITradeMarkContractorSource tradeMarkContractorSource)
            {
            if (tradeMarkContractorSource is Documents.Invoice)
                {
                this.tradeMarkContractorSource = new InvoiceTrademarkContractorSource((Documents.Invoice)tradeMarkContractorSource);
                }
            else
                {
                this.tradeMarkContractorSource = tradeMarkContractorSource;
                }
            this.SubGroupOfGoodsCacheObjectsStore = new SubGroupOfGoodsCacheObjectsStore(tradeMarkContractorSource);
            this.ManufacturerCacheObjectsStore = new ManufacturerCacheObjectsStore(tradeMarkContractorSource);
            this.NomenclatureCacheObjectsStore = new NomenclatureCacheObjectsStore(tradeMarkContractorSource);
            this.GroupOfGoodsStore = new GroupOfGoodsCacheObjectsStore();
            this.TradeMarkCacheObjectsStore = new TradeMarkCacheObjectsStore(tradeMarkContractorSource);
            this.UnitOfMeasureCacheObjectsStore = new UnitOfMeasureCacheObjectsStore();
            this.TradeMarksCreator = new TradeMarkObjectsCreator(this.TradeMarkCacheObjectsStore);
            this.ManufacturersCreator = new ManufacturersObjectsCreator(this.ManufacturerCacheObjectsStore);
            this.NomenclatureCreator = new NomenclatureObjectsCreator(this.NomenclatureCacheObjectsStore, this.ManufacturerCacheObjectsStore,
                this.TradeMarkCacheObjectsStore, this.CustomsCodesCacheStore, this.UnitOfMeasureCacheObjectsStore, this.CountryCahceObjectsStore);
            this.SubGroupOfGoodsObjectsCreator = new SubGroupOfGoodsObjectsCreator(this.ManufacturerCacheObjectsStore,
                this.TradeMarkCacheObjectsStore, SubGroupOfGoodsCacheObjectsStore, this.GroupOfGoodsStore);
            this.GroupOfGoodsCreator = new GroupOfGoodsObjectsCreator(this.GroupOfGoodsStore);
            this.ApprovalsCacheObjectsStore = new ApprovalsCacheObjectsStore();
            this.ApprovalsObjectCreator = new ApprovalsObjectCreator(ApprovalsCacheObjectsStore);
            this.NomenclatureRemovingHistoryCacheObjectsStore = new NomenclatureRemovingHistoryCacheObjectsStore();
            this.initializeCaches();
            }

        private void initializeCaches()
            {
            bool isInExistedTran = TransactionManager.TransactionManagerInstance.IsInTransaction();
            try
                {
                if (!isInExistedTran)
                    {
                    if (!TransactionManager.TransactionManagerInstance.BeginBusinessTransaction())
                        {
                        return;
                        };
                    }
                propertyOfGoodsCacheObjectsStore.Refresh();
                }
            finally
                {
                if (!isInExistedTran)
                    {
                    TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                    }
                }
            }

        public SystemInvoiceDBCache()
            : this(null)
            {
            }

        private Task refreshTask = null;

        /// <summary>
        /// Вызывается при завершении асинхронного обновления кеша
        /// </summary>
        public event Action OnCacheRefreshingComplete = null;

        /// <summary>
        /// Асинхронно обновляет кеш
        /// </summary>
        public void RefreshCacheAsync()
            {
            if (refreshTask == null || refreshTask.Status != TaskStatus.Running)
                {
                refreshTask = new Task(refreshCache);
                refreshTask.ContinueWith((task) =>
                    {
                        if (OnCacheRefreshingComplete != null)
                            {
                            OnCacheRefreshingComplete();
                            }
                    });
                try
                    {
                    refreshTask.Start();
                    }
                catch { }
                }
            }

        /// <summary>
        /// Выполняет обновление кешей документов справочников. Не обновляются кешированные разрешительные и история об удаленной номенклатуре, поскольку они задействованы
        /// только в некоторых процесах, где они и обновляются
        /// </summary>
        public void RefreshCache()
            {
            if (refreshTask != null && refreshTask.Status == TaskStatus.Running)//если выполняется асинхронное обновление - ждем пока оно завершится
                {
                try
                    {
                    refreshTask.Wait();
                    }
                catch { }
                }
            refreshCache();
            }

        private void refreshCache()
            {
            bool isInExistedTran = TransactionManager.TransactionManagerInstance.IsInTransaction();
            try
                {
                if (!isInExistedTran)
                    {
                    if (!TransactionManager.TransactionManagerInstance.BeginBusinessTransaction())
                        {
                        return;
                        };
                    }
                ContractorCacheObjectsStore.Refresh();
                NomenclatureCacheObjectsStore.Refresh();
                CustomsCodesCacheStore.Refresh();
                UnitOfMeasureCacheObjectsStore.Refresh();
                TradeMarkCacheObjectsStore.Refresh();
                ManufacturerCacheObjectsStore.Refresh();
                SubGroupOfGoodsCacheObjectsStore.Refresh();
                PropertyOfGoodsCacheObjectsStore.Refresh();
                PropertyTypesCacheObjectsStore.Refresh();
                CountryCahceObjectsStore.Refresh();
                GroupOfGoodsStore.Refresh();
                DocumentTypesCacheObjectsStore.Refresh();
                MaterialsMappingCacheObjectsStore.Refresh();
                }
            finally
                {
                if (!isInExistedTran)
                    {
                    TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                    }
                }
            }

        /// <summary>
        /// Получает длинну стельки для обуви
        /// </summary>
        /// <param name="groupName">Имя группы товара</param>
        /// <param name="subGroupName">Имя подгруппы товара</param>
        /// <param name="gropCode">Код группы</param>
        /// <param name="article">Артикул</param>
        /// <param name="tradeMark">Торговая марка</param>
        /// <param name="sizeEn">Размер в исходной системе</param>
        /// <returns>Размер стельки</returns>
        public string GetInsoleLength(string groupName, string subGroupName, string gropCode, string article, string tradeMark, string sizeEn)
            {

            long groupId = this.GroupOfGoodsStore.GetGroupOfGoodsId(groupName);
            long subGroupId = GetSubGroupId(groupName, subGroupName, gropCode);
            long contractorId = this.tradeMarkContractorSource.Contractor.Id;
            long trademarkId = this.TradeMarkCacheObjectsStore.GetTradeMarkId(tradeMark);
            long nomenclatureId = this.GetNomenclatureId(article, tradeMark);


            //long groupOfPropertyId = GetSubGroupId(groupName, subGroupName, gropCode);
            //long nomenclatureId = GetNomenclatureId(article, tradeMark);
            //long typeOfProperty = PropertyOfGoodsCacheObjectsStore.GetCachedObjectId("Размер");
            //if (typeOfProperty < 0)
            //    {
            //    return string.Empty;
            //    }
            string insoleLength = PropertyTypesCacheObjectsStore.GetInsoleLength(sizeEn, groupId, subGroupId, contractorId, trademarkId, nomenclatureId);
            return insoleLength;
            }

        /// <summary>
        /// Возвращет ID номенклатуры.
        /// </summary>
        public long GetNomenclatureId(string article, string tradeMarkName)
            {
            string tradeMarkClearName = string.IsNullOrEmpty(tradeMarkName) ? tradeMarkContractorSource.TradeMark.Description.Trim() : tradeMarkName;
            long tradeMarkId = TradeMarkCacheObjectsStore.GetCachedObjectId(new TradeMarkCacheObject(tradeMarkClearName, tradeMarkContractorSource.Contractor.Id));
            if (tradeMarkId == 0)
                {
                return 0;
                }
            NomenclatureCacheObject nomenclature = new NomenclatureCacheObject(article, tradeMarkId, tradeMarkContractorSource.Contractor.Id, 0);
            return NomenclatureCacheObjectsStore.GetCachedObjectId(nomenclature);
            }

        /// <summary>
        /// Возвращает кешированный екземпляр номенклатуры
        /// </summary>
        public NomenclatureCacheObject GetCachedNomenclature(string article, string tradeMark)
            {
            long contractorId = tradeMarkContractorSource.Contractor.Id;
            long tradeMarkId = TradeMarkCacheObjectsStore.GetTradeMarkIdOrCurrent(tradeMark);
            if (contractorId == 0 || tradeMarkId == 0)
                {
                return null;
                }
            NomenclatureCacheObject searched = new NomenclatureCacheObject(article, tradeMarkId, contractorId);
            return NomenclatureCacheObjectsStore.GetCachedObject(searched);
            }

        /// <summary>
        /// Возвращает айди подгруппы товара
        /// </summary>
        /// <param name="groupName">Имя группы</param>
        /// <param name="subGroupName">Имя подгруппы</param>
        /// <param name="subGroupCode">Код подгруппы</param>
        public long GetSubGroupId(string groupName, string subGroupName, string subGroupCode)
            {
            long groupId = this.GroupOfGoodsStore.GetGroupOfGoodsId(groupName);
            SubGroupOfGoodsCacheObject searched = new SubGroupOfGoodsCacheObject(groupId, subGroupCode, 0, 0, 0, subGroupName);
            return this.SubGroupOfGoodsCacheObjectsStore.GetCachedObjectId(searched);
            }

        /// <summary>
        /// Возвращает размер в украинской системе
        /// </summary>
        /// <param name="groupName">Имя группы</param>
        /// <param name="subGroupName">Имя подгруппы</param>
        /// <param name="subGroupCode">Код подгруппы</param>
        /// <param name="sizeEn"> </param>
        /// <param name="tradeMark"> </param>
        /// <param name="article"> </param>
        public string GetTranslatedSize(string groupName, string subGroupName, string subGroupCode, string sizeEn,string tradeMark, string article)
            {
            long groupId = this.GroupOfGoodsStore.GetGroupOfGoodsId(groupName);
            long subGroupId = GetSubGroupId(groupName, subGroupName, subGroupCode);
            long contractorId = this.tradeMarkContractorSource.Contractor.Id;
            long trademarkId = this.TradeMarkCacheObjectsStore.GetTradeMarkId(tradeMark);
            long nomenclatureId = this.GetNomenclatureId(article, tradeMark);
            return PropertyTypesCacheObjectsStore.GetSizeTranslation(sizeEn,groupId,subGroupId,contractorId,trademarkId,nomenclatureId);//GetSizeTranslation(subGroupId, sizeEn);
            }

        /// <summary>
        /// Получает пол для товара
        /// </summary>
        /// <param name="groupName">Имя группы</param>
        /// <param name="subGroupName">Имя подгруппы</param>
        /// <param name="subGroupCode">Код подгруппы</param>
        public string GetGender(string groupName, string subGroupName, string subGroupCode)
            {
            long groupId = GetSubGroupId(groupName, subGroupName, subGroupCode);
            if (groupId > 0)
                {
                return PropertyTypesCacheObjectsStore.GetGender(groupId);
                }
            return string.Empty;
            }

        /// <summary>
        /// Получает наименование товара на основании наименования его группы/подгруппы
        /// </summary>
        /// <param name="groupName">Имя группы</param>
        /// <param name="subGroupName">Имя подгруппы</param>
        /// <param name="subGroupCode">Код подгруппы</param>
        public string GetName(string groupName, string subGroupName, string subGroupCode)
            {
            long subGroupId = GetSubGroupId(groupName, subGroupName, subGroupCode);
            if (subGroupId > 0)
                {
                return PropertyTypesCacheObjectsStore.GetName(subGroupId);
                }
            return string.Empty;
            }

        #region Получение дополнительной информации о номенклатуре
        /// <summary>
        /// Возвращает наименование производителя для кешированной номенклатуры
        /// </summary>
        /// <param name="nomenclatureCacheObject">Объект кеша номенклатуры полученный из хранилища кешированной номенклатуры</param>
        /// <returns>Имя производителя</returns>
        public string GetNomenclatureManufacturer(NomenclatureCacheObject nomenclatureCacheObject)
            {
            ManufacturerCacheObject manufacturer = ManufacturerCacheObjectsStore.GetCachedObject(nomenclatureCacheObject.ManufacturerId);
            if (manufacturer != null)
                {
                return manufacturer.ManufacturerName;
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает наименование группы для кешированной номенклатуры
        /// </summary>
        /// <param name="nomenclatureCacheObject">Объект кеша номенклатуры полученный из хранилища кешированной номенклатуры</param>
        /// <returns>Имя группы</returns>
        public string GetNomenclatureGroupName(NomenclatureCacheObject nomenclatureCacheObject)
            {
            SubGroupOfGoodsCacheObject subGroup = SubGroupOfGoodsCacheObjectsStore.GetCachedObject(nomenclatureCacheObject.SubGroupId);
            if (subGroup == null)
                {
                return string.Empty;
                }
            GroupOfGoodsCacheObject group = this.GroupOfGoodsStore.GetCachedObject(subGroup.GroupId);
            if (group != null)
                {
                return group.GroupName;
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает внутренний таможенный код для кешированной номенклатуры
        /// </summary>
        /// <param name="nomenclatureCacheObject">Объект кеша номенклатуры полученный из хранилища кешированной номенклатуры</param>
        /// <returns>Цифровое значение внутреннего таможенного кода</returns>
        public string GetNomenclatureCustomsCodeIntern(NomenclatureCacheObject nomenclatureCacheObject)
            {
            CustomsCodesCacheObject customsCode = CustomsCodesCacheStore.GetCachedObject(nomenclatureCacheObject.CustomsCodeId);
            if (customsCode != null)
                {
                return customsCode.Code;
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает короткое наименование страны для кешированной номенклатуры
        /// </summary>
        /// <param name="nomenclatureCacheObject">Объект кеша номенклатуры полученный из хранилища кешированной номенклатуры</param>
        /// <returns>Короткое наименование страны</returns>
        public string GetNomenclatureCountryShortName(NomenclatureCacheObject nomenclatureCacheObject)
            {
            CountryCacheObject countryCacheObject = CountryCahceObjectsStore.GetCachedObject(nomenclatureCacheObject.CountryId);
            if (countryCacheObject != null)
                {
                return countryCacheObject.CountryShortName;
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает короткое наименование единицы измерения для кешированной номенклатуры
        /// </summary>
        /// <param name="nomenclatureCacheObject">Объект кеша номенклатуры полученный из хранилища кешированной номенклатуры</param>
        /// <returns>Короткое наименование единицы измерения</returns>
        public string GetNomenclatureUnitOfMeasureCode(NomenclatureCacheObject cachedNomenclature)
            {
            UnitOfMeasureCacheObject unitOfMeasureCacheObject = UnitOfMeasureCacheObjectsStore.GetCachedObject(cachedNomenclature.UnitOfMeasureId);
            if (unitOfMeasureCacheObject != null)
                {
                return unitOfMeasureCacheObject.Name;
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает наименование подгруппы для кешированной номенклатуры
        /// </summary>
        /// <param name="nomenclatureCacheObject">Объект кеша номенклатуры полученный из хранилища кешированной номенклатуры</param>
        /// <returns>Наименование подгруппы</returns>
        public string GetNomenclatureSubGroup(NomenclatureCacheObject cacheObject)
            {
            SubGroupOfGoodsCacheObject subGroup = SubGroupOfGoodsCacheObjectsStore.GetCachedObject(cacheObject.SubGroupId);
            if (subGroup != null)
                {
                return subGroup.Name;
                }
            return string.Empty;
            }

        /// <summary>
        /// Возвращает код подгруппы для кешированной номенклатуры
        /// </summary>
        /// <param name="nomenclatureCacheObject">Объект кеша номенклатуры полученный из хранилища кешированной номенклатуры</param>
        /// <returns>Код подгруппы</returns>
        public string GetNomenclatureSubGroupCode(NomenclatureCacheObject cacheObject)
            {
            SubGroupOfGoodsCacheObject subGroup = SubGroupOfGoodsCacheObjectsStore.GetCachedObject(cacheObject.SubGroupId);
            if (subGroup != null)
                {
                return subGroup.Code;
                }
            return string.Empty;
            }
        #endregion
        }
    }
