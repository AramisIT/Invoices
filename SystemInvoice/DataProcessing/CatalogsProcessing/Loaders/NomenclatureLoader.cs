using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using Aramis.Core;
using AramisWpfComponents.Excel;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Загружает справочиник номенклатуры
    /// </summary>
    public class NomenclatureLoader : FromExcelToDataBaseObjectsLoaderBase<Nomenclature>
        {
        private ITradeMarkContractorSource tradeMarkContractorSource = null;
        //1 - based indexes
        private const int descriptionAndNameInvoiceDisplayColumnIndex = 13;
        private const int declarationNameDisplayColumnIndex = 12;
        private const int articleDisplayColumnIndex = 5;
        private const int priceDisplayColumnIndex = 6;
        private const int customCodeExternDisplayColumnIndex = 10;
        private const int nameOriginalDisplayColumnIndex = 21;
        //Zero - based indexes
        private const int countryNameColumnIndex = 6;
        private const int customsCodeInternalColumnIndex = 10;
        private const int manufacturerColumnIndex = 8;
        private const int trademarkColumnIndex = 7;
        private const int unitOfMeasureColumnIndex = 16;
        private const int netWeightColumnIndex = 13;

        private long currentContractorId = 0;

        private HashSet<NomenclatureCacheObject> createdObjects = new HashSet<NomenclatureCacheObject>();

        public NomenclatureLoader(ITradeMarkContractorSource tradeMarkContractorSource, SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            this.tradeMarkContractorSource = tradeMarkContractorSource;
            }

        protected override bool CheckItemBegoreCreate(Nomenclature itemToCheck)
            {
            var newObject = new NomenclatureCacheObject(itemToCheck.Article, itemToCheck.TradeMark.Id,
                                                        itemToCheck.Contractor.Id, 0);
            if (createdObjects.Contains(newObject))
                {
                return false;
                }

            var cachedObject = cachedData.NomenclatureCacheObjectsStore.GetCachedObject(newObject);
            if (cachedObject == null)
                {
                createdObjects.Add(newObject);
                return true;
                }

            double cachedWeightFrom = cachedObject.NetWeightFrom;
            double cachedTo = cachedObject.NetWeightTo;
            string cachedBarCode = cachedObject.BarCode;
            string cachedNameOriginal = cachedObject.NameOriginal.Trim();

            if ((cachedWeightFrom == 0 && itemToCheck.NetWeightFrom != 0) || (cachedTo == 0 && itemToCheck.NetWeightTo != 0) || (!cachedNameOriginal.Equals(itemToCheck.NameOriginal)))
                {
                long id = cachedData.NomenclatureCacheObjectsStore.GetCachedObjectId(cachedObject);
                double newNetWeightFrom = cachedWeightFrom == 0 ? itemToCheck.NetWeightFrom : cachedWeightFrom;
                double newNetWeightTo = cachedTo == 0 ? itemToCheck.NetWeightTo : cachedTo;
                string nameOriginal = string.IsNullOrEmpty(cachedNameOriginal) ? itemToCheck.NameOriginal : cachedNameOriginal;
                itemToCheck.Id = id;
                itemToCheck.Read();
                itemToCheck.NetWeightFrom = newNetWeightFrom;
                itemToCheck.NetWeightTo = newNetWeightTo;
                itemToCheck.NameOriginal = nameOriginal;
                createdObjects.Add(newObject);
                return true;
                }
            return false;
            }

        protected override void InitializeMapping(Excel.ExcelMapper mapper)
            {
            base.AddPropertyMapping("Description", descriptionAndNameInvoiceDisplayColumnIndex);
            base.AddPropertyMapping("NameDecl", declarationNameDisplayColumnIndex);
            base.AddPropertyMapping("Article", articleDisplayColumnIndex);
            base.AddPropertyMapping("Price", priceDisplayColumnIndex);
            base.AddCustomMapping("Country", selectCountry);
            base.AddCustomMapping("CustomsCodeInternal", selectCustomsCode);
            base.AddPropertyMapping("CustomsCodeExtern", customCodeExternDisplayColumnIndex);
            base.AddCustomMapping("Contractor", selectContractor);
            base.AddCustomMapping("Manufacturer", selectManufacturer);
            base.AddCustomMapping("TradeMark", selectTradeMark);
            base.AddCustomMapping("UnitOfMeasure", selectUnitOfMeasure);
            base.AddCustomMapping("NetWeightFrom", selectNetWeightFrom);
            base.AddCustomMapping("NetWeightTo", selectNetWeightTo);
            base.AddPropertyMapping("NameOriginal", nameOriginalDisplayColumnIndex);
            }

        private Country selectCountry(Row row)
            {
            string countryName = row[countryNameColumnIndex].Value.ToString().Trim();
            long countryId = cachedData.CountryCahceObjectsStore.GetIdForCountryShortName(countryName);
            Country country = new Country();
            country.Id = countryId;
            return country;
            }

        private CustomsCode selectCustomsCode(Row row)
            {
            string customsCodeInternal = row[customsCodeInternalColumnIndex].Value.ToString().Trim();
            long customCodeId = cachedData.CustomsCodesCacheStore.GetCustomsCodeIdForCodeName(customsCodeInternal);
            CustomsCode customsCode = new CustomsCode();
            customsCode.Id = customCodeId;

            if (customCodeId == 0)
                {
                string custId = "aaa";
                }
            return customsCode;
            }

        private IContractor selectContractor(Row row)
            {
            IContractor contractor = A.New<IContractor>();
            contractor.Id = currentContractorId;
            return contractor;
            }

        private IManufacturer selectManufacturer(Row row)
            {
            string manufacturerName = row[manufacturerColumnIndex].Value.ToString().Trim();
            long manufacturerId = cachedData.ManufacturerCacheObjectsStore.GetManufcaturerId(manufacturerName, currentContractorId);

            IManufacturer manufacturer = A.New<IManufacturer>();
            manufacturer.Contractor = selectContractor(row);
            manufacturer.Id = manufacturerId;
            //создаем нового производителя если такого еще нету
            if (manufacturerId == 0 && !string.IsNullOrEmpty(manufacturerName) && currentContractorId > 0)
                {
                manufacturer.Description = manufacturerName;
                if (manufacturer.Write() == Aramis.Core.WritingResult.Success)
                    {
                    this.cachedData.ManufacturerCacheObjectsStore.Refresh();
                    }
                }
            return manufacturer;
            }

        private ITradeMark selectTradeMark(Row row)
            {
            string tradeMarkName = row[trademarkColumnIndex].Value.ToString().Trim();
            long tradeMarkId = cachedData.TradeMarkCacheObjectsStore.GetTradeMarkId(tradeMarkName, currentContractorId);

            ITradeMark tradeMark = A.New<ITradeMark>();
            tradeMark.Contractor = selectContractor(row);
            tradeMark.Id = tradeMarkId;
            //создаем новую торговую марку если такой еще нету
            if (tradeMarkId == 0 && !string.IsNullOrEmpty(tradeMarkName) && currentContractorId > 0)
                {
                tradeMark.Description = tradeMarkName;
                if (tradeMark.Write() == Aramis.Core.WritingResult.Success)
                    {
                    this.cachedData.TradeMarkCacheObjectsStore.Refresh();
                    }
                }
            return tradeMark;
            }

        private UnitOfMeasure selectUnitOfMeasure(Row row)
            {
            string unitOfMeasureShortName = row[unitOfMeasureColumnIndex].Value.ToString().Trim();
            long unitOfMeasureId = cachedData.UnitOfMeasureCacheObjectsStore.GetUnitOfMeasureIdForShortName(unitOfMeasureShortName);
            UnitOfMeasure unitOfMeasure = new UnitOfMeasure();
            unitOfMeasure.Id = unitOfMeasureId;
            return unitOfMeasure;
            }

        private object selectNetWeightFrom(Row row)
            {
            string netWeightStr = row[netWeightColumnIndex].Value.ToString().Trim();
            netWeightStr = netWeightStr.Replace(".", ",");
            double netWeight = 0;
            if (double.TryParse(netWeightStr, out netWeight))
                {
                return netWeight;
                }
            return 0;
            }

        private object selectNetWeightTo(Row row)
            {
            string netWeightStr = row[netWeightColumnIndex].Value.ToString().Trim();
            netWeightStr = netWeightStr.Replace(".", ",");
            double netWeight = 0;
            if (double.TryParse(netWeightStr, out netWeight))
                {
                return netWeight;
                }
            return 0;
            }

        protected override bool OnLoadBegin()
            {
            if (this.trySelectCurrentContractor())
                {
                this.createdObjects.Clear();
                // this.cachedData.tr
                base.cachedData.RefreshCache();
                cachedData.RefreshCache();
                }
            else
                {
                return false;
                }
            return true;
            }

        protected override void OnLoadComplete()
            {
            this.tradeMarkContractorSource.Contractor = A.New<IContractor>();
            this.tradeMarkContractorSource.TradeMark = A.New<ITradeMark>();
            base.OnLoadComplete();
            string countStr = string.Format("createdNumbers: {0}", createdObjects.Count);
            Console.WriteLine(countStr);
            }

        protected override int StartRowIndex
            {
            get
                {
                return 4;
                }
            }

        private bool trySelectCurrentContractor()
            {
            currentContractorId = Aramis.UI.UserInterface.Current.SelectItemFromList((A.New<IContractor>()).GUID, 0);
            this.tradeMarkContractorSource.Contractor = A.New<IContractor>();
            this.tradeMarkContractorSource.Contractor.Id = currentContractorId;
            this.tradeMarkContractorSource.TradeMark = A.New<ITradeMark>();
            return currentContractorId > 0;
            }
        }
    }
