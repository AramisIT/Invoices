using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache.ManufacturersCache;
using SystemInvoice.DataProcessing.Cache.TradeMarksCache;
using SystemInvoice.DataProcessing.Cache.CustomCodesCache;
using SystemInvoice.DataProcessing.Cache.UnitOfMeasureCache;
using SystemInvoice.DataProcessing.Cache.CountryCache;
using Aramis.Core;

namespace SystemInvoice.DataProcessing.Cache.NomenclaturesCache
    {
    /// <summary>
    /// Создает новые экземпляры номенклатуры
    /// </summary>
    public class NomenclatureObjectsCreator : DbObjectCreator<Nomenclature, NomenclatureCacheObject>
        {
        NomenclatureCacheObjectsStore nomenclatureStore = null;
        ManufacturerCacheObjectsStore manufacturersStore = null;
        TradeMarkCacheObjectsStore tradeMarksStore = null;

        UnitOfMeasureCacheObjectsStore unitsOfMeasuresStore = null;
        CountryCahceObjectsStore countriesStore = null;
        /// <summary>
        /// Используется для проверки того что мы добавляем каждую номенклатуру в список на создание только один раз
        /// </summary>
        NomenclatureFastSearchSet createdNomenclatures = new NomenclatureFastSearchSet();

        public CustomsCodesCacheObjectsStore CustomsCodesStore { get; private set; }

        public NomenclatureObjectsCreator(NomenclatureCacheObjectsStore nomenclatureStore,
            ManufacturerCacheObjectsStore manufacturersStore,
            TradeMarkCacheObjectsStore tradeMarksStore,
            CustomsCodesCacheObjectsStore customsCodesStore,
            UnitOfMeasureCacheObjectsStore unitsOfMeasuresStore,
            CountryCahceObjectsStore countriesStore)
            : base(nomenclatureStore)
            {
            this.nomenclatureStore = nomenclatureStore;
            this.tradeMarksStore = tradeMarksStore;
            this.manufacturersStore = manufacturersStore;
            this.CustomsCodesStore = customsCodesStore;
            this.unitsOfMeasuresStore = unitsOfMeasuresStore;
            this.countriesStore = countriesStore;
            }

        protected override Nomenclature createDBObject(NomenclatureCacheObject cacheObject)
            {
            if (!cacheObject.IsValidForCreation())
                {
                int k = 0;//просто тест для того что бы понять в отладчике почему не создается номенклатура
                }
            //вспомогательные объекты
            IContractor contractor = A.New<IContractor>();
            contractor.Id = cacheObject.ContractorId;
            IManufacturer manufacturerN = A.New<IManufacturer>();
            manufacturerN.Id = cacheObject.ManufacturerId;
            manufacturerN.Contractor = contractor;
            ITradeMark tmark = A.New<ITradeMark>();
            tmark.Contractor = contractor;
            tmark.Id = cacheObject.TradeMarkId;
            CustomsCode code = new CustomsCode();
            code.Id = cacheObject.CustomsCodeId;
            Country country = new Country();
            country.Id = cacheObject.CountryId;
            UnitOfMeasure unitOfMeasure = new UnitOfMeasure();
            unitOfMeasure.Id = cacheObject.UnitOfMeasureId;
            SubGroupOfGoods subGroupOfGoods = new SubGroupOfGoods();
            subGroupOfGoods.Id = cacheObject.SubGroupId;
            //создание номенклатуры
            Nomenclature nomenclature = new Nomenclature();
            nomenclature.Contractor = contractor;
            nomenclature.TradeMark = tmark;
            nomenclature.Manufacturer = manufacturerN;
            nomenclature.CustomsCodeInternal = code;
            nomenclature.Country = country;
            nomenclature.UnitOfMeasure = unitOfMeasure;
            nomenclature.Article = cacheObject.Article;
            nomenclature.NameOriginal = cacheObject.NameOriginal;
            nomenclature.NameInvoice = cacheObject.NameInvoice;
            nomenclature.NameDecl = cacheObject.NameDecl;
            nomenclature.CustomsCodeExtern = cacheObject.CustomsCodeExtern;
            nomenclature.BarCode = cacheObject.BarCode;
            nomenclature.Price = double.IsNaN(cacheObject.Price) ? 0 : cacheObject.Price;
            nomenclature.NetWeightFrom = double.IsNaN(cacheObject.NetWeightFrom) ? 0 : cacheObject.NetWeightFrom;
            nomenclature.NetWeightTo = double.IsNaN(cacheObject.NetWeightTo) ? 0 : cacheObject.NetWeightTo;
            nomenclature.GrossWeightFrom = double.IsNaN(cacheObject.GrossWeight) ? 0 : cacheObject.GrossWeight;
            nomenclature.GrossWeightTo = double.IsNaN(cacheObject.GrossWeight) ? 0 : cacheObject.GrossWeight;
            nomenclature.SubGroupOfGoods = subGroupOfGoods;
            return nomenclature;
            }

        protected override void deleteObject(Nomenclature objectToDelete)
            {
            long id = objectToDelete.Id;
            if (id == 0)
                {
                return;
                }
            string query = string.Format("delete from Nomenclature where Id = {0}", id);
            ExceuteQuery(query);
            }

        /// <summary>
        /// Проверяет можно ли добавить номенклатуру - существовала ли она ранее
        /// </summary>
        public bool CanAddNewNomenclature(string article, string trademark)
            {
            long trademarkId = tradeMarksStore.GetTradeMarkIdOrCurrent(trademark);
            if (this.nomenclatureStore.SelectNomenclatureIfExists(article, trademarkId) == 0)
                {
                if (createdNomenclatures.AddIfNotContains(article, trademarkId))
                    {
                    return true;
                    }
                }
            return false;
            }
        /// <summary>
        /// Добавляет информацию необходимую для создания новой номенклатуры
        /// </summary>
        /// <returns>Была ли информация добавлена и будет ли номенклатура создана при вызове метода Create</returns>
        public bool AddNomenclature(string article, string trademark, string manufacturer, long CustomsCodeId, string invoiceName,
            string countryShortName, string unitOfMeasureName, string customsCodeExtern, string barCode, double netWeightFrom, double netWeightTo, double grossWright, double price,
            string nameOriginal, string nameDecl, long groupId)//, string groupNamestring subGroupName,string subGroupCode)
            {
            try
                {
                //получаем айдишники связанных с номенклатурой справочников
                long TradeMarkId = tradeMarksStore.GetTradeMarkIdOrCurrent(trademark);
                long ContractorId = tradeMarksStore.CurrentContractor;
                long ManufacturerId = manufacturersStore.GetManufcaturerId(manufacturer);

                long CountryId = countriesStore.GetIdForCountryShortName(countryShortName);
                long UnitOfMeasureId = unitsOfMeasuresStore.GetCachedObjectId(new UnitOfMeasureCacheObject(unitOfMeasureName, string.Empty));
                //значения непосредственно присваиваемые номенклатуре
                string Article = article;
                string InvoiceName = invoiceName;
                string CustomsCodeExtern = customsCodeExtern;
                string BarCode = barCode;
                double NetWeightFrom = netWeightFrom;
                double NetWeightTo = netWeightTo;
                double GrossWeight = grossWright;
                double Price = price;
                string NameOriginal = nameOriginal;
                string NameDecl = nameDecl;
                //создаем объект - кеш, на основании которого потом мы создаем номенклатуру, и пытаемся его добавить в список для создания
                NomenclatureCacheObject cacheObject = new NomenclatureCacheObject(Article, TradeMarkId, ContractorId, ManufacturerId, CustomsCodeId,
                    InvoiceName, CountryId, UnitOfMeasureId, CustomsCodeExtern, BarCode, NetWeightFrom, NetWeightTo, GrossWeight, Price, NameOriginal, NameDecl, groupId, 0, string.Empty);
                return base.TryAddToCreationList(cacheObject);
                }
            catch (Exception e)
                {
                return false;
                }
            }

        public void BeginCreation()
            {
            createdNomenclatures.Clear();
            Refresh();
            }


        protected override string failToCreateMessage(int failCount)
            {
            return string.Format(@"{0} елемента(ов) справочника ""Номенклатура"" не удалось сохранить, так как не все обязательные поля в файле с новыми позициями заполнены.
Вернитесь, пожалуйста, в файл с новыми позициями, заполните все поля и загрузите ещё раз.", failCount);
            }
        }
    }
