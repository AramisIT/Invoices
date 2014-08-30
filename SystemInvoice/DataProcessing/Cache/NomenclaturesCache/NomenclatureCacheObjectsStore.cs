using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.DataProcessing.Cache.NomenclaturesCache
    {
    /// <summary>
    /// Использутеся для хранения информации о номенклатуре и связанную с ней информацию
    /// </summary>
    public class NomenclatureCacheObjectsStore : CacheObjectsStore<NomenclatureCacheObject>
        {
        /// <summary>
        /// Объект - запрос для поиска номенклатуры
        /// </summary>
        NomenclatureCacheObject searchObject = new NomenclatureCacheObject(string.Empty, 0, 0, 0);
        /// <summary>
        /// Интерфейс с помощью которого мы меняем поля объекта поиска.
        /// </summary>
        INomenclatureSearch searchState = null;

        /// <summary>
        /// Экземпляр реализации интерфейса который позволяет получить доступ к текущему контрагенту/торговой марке, которые использовались при построении кеша
        /// </summary>
        private ITradeMarkContractorSource tradeMarkContractorSource = null;
        /// <summary>
        /// Индекс для поиска произвольной кешированной номенклатуры соответствующей выбранному таможенному коду
        /// </summary>
        private ByCustomsCodeSearchIndex byCustomsCodeSearchIndex = new ByCustomsCodeSearchIndex();

        public NomenclatureCacheObjectsStore(ITradeMarkContractorSource tradeMarkContractorSource)
            {
            this.tradeMarkContractorSource = tradeMarkContractorSource;
            searchState = searchObject;
            }

        protected override string LatModifiedDateQuery
            {
            get
                {
                long tradeMarkId = tradeMarkContractorSource == null ? 0 : tradeMarkContractorSource.TradeMark.Id;
                long contractorId = tradeMarkContractorSource == null ? 0 : tradeMarkContractorSource.Contractor.Id;
                string resultQuery = string.Format(@"with preparedForMax as
(
	select (SELECT Max(v) 
	   FROM (VALUES (nom.LastModified), (tradeMark.LastModified), (contractor.LastModified),
	   (customsCodes.LastModified), (manufacturer.LastModified), (country.LastModified),
	   (subGroups.LastModified), (groupOfGoods.LastModified), (unitOfMeasure.LastModified)) AS value(v)) as [MaxDate] from Nomenclature as nom
	left outer join TradeMark as tradeMark on tradeMark.Id = nom.TradeMark and tradeMark.MarkForDeleting = 0
	left outer join Contractor as contractor on contractor.Id = nom.Contractor and contractor.MarkForDeleting = 0
	left outer join CustomsCode as customsCodes on customsCodes.Id = nom.CustomsCodeInternal and customsCodes.MarkForDeleting = 0
	left outer join Manufacturer as manufacturer on manufacturer.Id = nom.Manufacturer
	left outer join Country as country on country.Id = nom.Country
	left outer join SubGroupOfGoods as subGroups on subGroups.Id = nom.SubGroupOfGoods
	left outer join GroupOfGoods as groupOfGoods on groupOfGoods.Id = subGroups.GroupOfGoods
	left outer join UnitOfMeasure as unitOfMeasure on unitOfMeasure.Id = nom.UnitOfMeasure
	left outer join SubNomenclatureSetContents as cont on cont.IdDoc = nom.Id
	where  nom.MarkForDeleting = 0 and (nom.TradeMark = {0} or {0} = 0) and (nom.Contractor = {1} or {1} = 0)
)
select MAX(MaxDate) from preparedForMax;", tradeMarkId, contractorId);
                return resultQuery;
                }
            }

        protected override string LastProcessedCountQuery
            {
            get
                {
                long tradeMarkId = tradeMarkContractorSource == null ? 0 : tradeMarkContractorSource.TradeMark.Id;
                long contractorId = tradeMarkContractorSource == null ? 0 : tradeMarkContractorSource.Contractor.Id;
                string resultQuery = string.Format(@"select Count(*) from Nomenclature as nom
left outer join TradeMark as tradeMark on tradeMark.Id = nom.TradeMark and tradeMark.MarkForDeleting = 0
left outer join Contractor as contractor on contractor.Id = nom.Contractor and contractor.MarkForDeleting = 0
left outer join CustomsCode as customsCodes on customsCodes.Id = nom.CustomsCodeInternal and customsCodes.MarkForDeleting = 0
left outer join Manufacturer as manufacturer on manufacturer.Id = nom.Manufacturer
left outer join Country as country on country.Id = nom.Country
left outer join SubGroupOfGoods as subGroups on subGroups.Id = nom.SubGroupOfGoods
left outer join GroupOfGoods as groupOfGoods on groupOfGoods.Id = subGroups.GroupOfGoods
left outer join UnitOfMeasure as unitOfMeasure on unitOfMeasure.Id = nom.UnitOfMeasure
left outer join SubNomenclatureSetContents as cont on cont.IdDoc = nom.Id
where nom.MarkForDeleting = 0 and (nom.TradeMark = {0} or {0} = 0) and (nom.Contractor = {1} or {1} = 0);", tradeMarkId, contractorId);
                return resultQuery;
                }
            }

        protected override string SelectQuery
            {
            get
                {
                long tradeMarkId = tradeMarkContractorSource == null ? 0 : tradeMarkContractorSource.TradeMark.Id;
                long contractorId = tradeMarkContractorSource == null ? 0 : tradeMarkContractorSource.Contractor.Id;
                if (contractorId == 0)
                    {
                    return string.Empty;
                    }
                return string.Format(@"select LTRIM(RTRIM(nom.Article)) as article,
LTRIM(RTRIM(nom.CustomsCodeExtern)) as customsCodeExtern,
LTRIM(RTRIM(nom.NameOriginal)) as originalName,LTRIM(RTRIM(nom.NameDecl)) as declName,LTRIM(RTRIM(nom.Description)) as invName,
nom.TradeMark as tradeMarkId,nom.Contractor as contractorId,
nom.Manufacturer as manufacturerId,nom.Id as Id,nom.BarCode as barCode,
nom.CustomsCodeInternal as customsCodesId,
nom.SubGroupOfGoods as SubGroupOfGoodsId,
nom.UnitOfMeasure as unitOfMeasureId,
nom.Country as countryId,
cast(cast(nom.Price as numeric(14,2)) as float) as price,
cast(cast(nom.NetWeightFrom as numeric(14,4)) as float) as netWeightFrom,
cast(cast(nom.NetWeightTo as numeric(14,4)) as float) as netWeightTo,
case when cont.IdDoc IS null then 0 else 1 end as hasContent,
rtrim(nom.Model) model
from Nomenclature as nom
left outer join SubNomenclatureSetContents as cont on cont.IdDoc = nom.Id
where nom.MarkForDeleting = 0 and (nom.TradeMark = {0} or {0} = 0) and (nom.Contractor = {1} or {1} = 0);", tradeMarkId, contractorId);
                }
            }

        //public override void Refresh()
        //    {
        //    DateTime dataTime = DateTime.Now;
        //    base.Refresh();
        //    double duration = (DateTime.Now - dataTime).TotalMilliseconds;
        //    }

        protected override NomenclatureCacheObject createNew(System.Data.DataRow row)
            {
            string article = row.TryGetColumnValue<string>("article", null);
            long tradeMarkId = row.TryGetColumnValue<long>("tradeMarkId", -1);
            long contractorId = row.TryGetColumnValue<long>("contractorId", -1);
            string invoiceName = row.TrySafeGetColumnValue<string>("invName", string.Empty);
            long customsCodeId = row.TrySafeGetColumnValue<long>("customsCodesId", -1);
            long manufacturerId = row.TrySafeGetColumnValue<long>("manufacturerId", -1);
            double netWeightFrom = row.TrySafeGetColumnValue<double>("netWeightFrom", 0);
            double netWeightTo = row.TrySafeGetColumnValue<double>("netWeightTo", 0);
            double grossWeight = 0;
            double price = row.TrySafeGetColumnValue<double>("price", 0);
            long countryId = row.TrySafeGetColumnValue<long>("countryId", -1);
            long unitOfMeasureId = row.TrySafeGetColumnValue<long>("unitOfMeasureId", -1);
            string customsCodeExtern = row.TrySafeGetColumnValue<string>("customsCodeExtern", "");
            string barCode = row.TrySafeGetColumnValue<string>("barCode", "");
            string nameOriginal = row.TrySafeGetColumnValue<string>("originalName", "");
            string nameDecl = row.TrySafeGetColumnValue<string>("declName", "");
            long subGroupOfGoodsId = row.TrySafeGetColumnValue<long>("SubGroupOfGoodsId", 0);
            int hasContent = row.TryGetColumnValue<int>("hasContent", 0);
            string model = row.TrySafeGetColumnValue<string>("model", string.Empty);
            if (article == null || tradeMarkId == -1 || contractorId == -1)
                {
                return null;
                }
            var objectCreated = new NomenclatureCacheObject(article, tradeMarkId, contractorId, manufacturerId, customsCodeId, invoiceName, countryId, unitOfMeasureId, customsCodeExtern, barCode,
                netWeightFrom, netWeightTo, grossWeight, price, nameOriginal, nameDecl, subGroupOfGoodsId, hasContent, model);
            return objectCreated;
            }

        protected override void InitializeIndexes(List<IEqualityComparer<NomenclatureCacheObject>> indexes)
            {
            base.InitializeIndexes(indexes);//создаем индекс для поиска номенклатуры по таможенному коду
            indexes.Add(byCustomsCodeSearchIndex);
            }

        /// <summary>
        /// Возвращает ID номенклатуры для заданного артикула и торговой марки используемой при построении кэша, торговая марка не может быть не выбранной
        /// поскольку методы этой группы испольуются при загрузке РД из файла где нету ТМ.
        /// </summary>
        /// <param name="article">Артикул</param>
        /// <returns>Id номенклатуры или 0 - если номенклатура не найдена</returns>
        public long SelectNomenclatureIfExists(string article)
            {
            if (setSearchedNomenclature(article))
                {
                return GetCachedObjectId(searchObject);
                }
            return 0;
            }

        /// <summary>
        /// Возвращает ID номенклатуры для заданного артикула и торговой марки
        /// </summary>
        /// <param name="article">Артикул</param>
        /// <returns>Id номенклатуры или 0 - если номенклатура не найдена</returns>
        public long SelectNomenclatureIfExists(string article, long tradeMarkId)
            {
            if (setSearchedNomenclature(article, tradeMarkId))
                {
                return GetCachedObjectId(searchObject);
                }
            return 0;
            }

        /// <summary>
        /// Возвращает кешированный объект для заданного артикула и ТМ используемой при построении кэша, торговая марка не может быть не выбранной
        /// поскольку методы этой группы испольуются при загрузке РД из файла где нету ТМ.
        /// </summary>
        /// <param name="article">Артикул</param>
        /// <returns>Кешированный объект, возвращает null  если объект не найден</returns>
        public NomenclatureCacheObject GetCachedNomenclatureForArticle(string article)
            {
            if (setSearchedNomenclature(article))
                {
                return GetCachedObject(searchObject);
                }
            return null;
            }

        /// <summary>
        /// Устанавливает артикул в объекте поиска. Установка не будет осуществлена если в источнике для выбора ТМ она не установлена 
        /// </summary>
        /// <param name="article">Артикул</param>
        /// <returns>Был ли изменен объект для поиска номенклатуры</returns>
        private bool setSearchedNomenclature(string article)
            {
            long contractorId = tradeMarkContractorSource.Contractor.Id;
            long tradeMarkId = tradeMarkContractorSource.TradeMark.Id;
            if (tradeMarkId == 0 || contractorId == 0)
                {
                return false;
                }
            searchState.SetSearchOptions(article, tradeMarkId, contractorId);
            return true;
            }

        /// <summary>
        /// Устанавливает артикул и торговую марку в объекте поиска номенклатуры
        /// </summary>
        /// <param name="article">Артикул</param>
        /// <param name="tradeMarkId">Торговая марка</param>
        private bool setSearchedNomenclature(string article, long tradeMarkId)
            {
            long contractorId = tradeMarkContractorSource.Contractor.Id;
            if (contractorId == 0)
                {
                return false;
                }
            searchState.SetSearchOptions(article, tradeMarkId, contractorId);
            return true;
            }

        /// <summary>
        /// Возвращает диапазон весов нетто для номенклатуры соответствующей оперделенному таможенному коду
        /// </summary>
        public Tuple<double, double> GetNetWeightRangeForCustomsCode(long customsCodeId)
            {
            if (customsCodeId == 0)
                {
                return null;
                }
            NomenclatureCacheObject byCustomsCodesearchObject = new NomenclatureCacheObject(string.Empty, 0, 0, 0, customsCodeId, string.Empty, 0);
            long cachedId = GetCachedObjectId(byCustomsCodeSearchIndex, byCustomsCodesearchObject);
            if (cachedId == 0)
                {
                return null;
                }
            NomenclatureCacheObject cached = GetCachedObject(byCustomsCodeSearchIndex, cachedId);
            return getNomenclatureNetWeightsRange(cached);
            }

        /// <summary>
        /// Возвращает диапазон весов нетто для номенклатуры
        /// </summary>
        public Tuple<double, double> GetNetWeightRangeForNomenclature(long nomenclatureId)
            {
            if (nomenclatureId == 0)
                {
                return null;
                }
            NomenclatureCacheObject cached = GetCachedObject(nomenclatureId);
            return getNomenclatureNetWeightsRange(cached);
            }

        /// <summary>
        /// Выбирает диапазон допустимого веса нетто для кешированного объекта номенклатуры
        /// </summary>
        /// <param name="cached">Кешированный объект</param>
        /// <returns>Диапазон</returns>
        private Tuple<double, double> getNomenclatureNetWeightsRange(NomenclatureCacheObject cached)
            {
            if (cached == null)
                {
                return null;
                }
            double netWeightFrom = cached.NetWeightFrom;
            double netWeightTo = cached.NetWeightTo;
            return new Tuple<double, double>(netWeightFrom, netWeightTo);
            }

        /// <summary>
        /// Проверяет существует ли номенклатура в кеше
        /// </summary>
        public bool ContainsNomenclature(string article, long tradeMarkId, long contractorId)
            {
            NomenclatureCacheObject nomenclatureObject = new NomenclatureCacheObject(article, tradeMarkId, contractorId);
            return GetCachedObject(nomenclatureObject) != null;
            }
        }
    }
