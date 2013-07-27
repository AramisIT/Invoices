using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.DataProcessing.Cache.ManufacturersCache
    {
    /// <summary>
    /// Хранилище кэшированных производителей используется к примеру для поиска производителя по айдишнику контрагента и имени производителя
    /// </summary>
    public class ManufacturerCacheObjectsStore : CacheObjectsStore<ManufacturerCacheObject>
        {
        /// <summary>
        /// Объект - запрос для поиска таможенного кода, при поиске мы просто меняем поле объекта, это позволяет нам не создавать каждый раз для поиска нового производителея новый объект,
        /// что экономит память и увеличивает скорость
        /// </summary>
        ManufacturerCacheObject searchObject = new ManufacturerCacheObject(string.Empty, 0);
        /// <summary>
        /// Интерфейс с помощью которого мы меняем поля объекта поиска, делаем это для того что бы нельзя напрямую изменить эти поля в кешированных объектах и тем 
        /// самым нарушить целостность данных
        /// </summary>
        IManufacturerSearch searchState = null;

        /// <summary>
        /// Экземпляр реализации интерфейса который позволяет получить доступ к текущему контрагенту/торговой марке, которые использовались при построении кеша
        /// </summary>
        private ITradeMarkContractorSource tradeMarkContractorSource = null;

        /// <summary>
        /// Создает новый экземпляр хранилища производителей
        /// </summary>
        /// <param name="tradeMarkContractorSource">Объект содержащий информацию о текущей торговой марке, текущем контрагенте</param>
        public ManufacturerCacheObjectsStore(ITradeMarkContractorSource tradeMarkContractorSource)
            {
            this.tradeMarkContractorSource = tradeMarkContractorSource;
            searchState = searchObject;
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from Manufacturer;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from Manufacturer where MarkForDeleting = 0;"; }
            }

        protected override string SelectQuery
            {
            get { return "select Id,LTRIM(RTRIM(Description)) as name,Contractor as contractorID from Manufacturer where MarkForDeleting = 0;"; }
            }

        protected override ManufacturerCacheObject createNew(System.Data.DataRow row)
            {
            string name = row.TryGetColumnValue<string>("name", "").Trim();
            long contractorID = row.TryGetColumnValue<long>("contractorID", 0);
            return new ManufacturerCacheObject(name, contractorID);
            }

        /// <summary>
        /// Возвращает ID производителя по имени и текущему контрагенту
        /// </summary>
        /// <param name="manufactuerName">Имя производителя</param>
        public long GetManufcaturerId(string manufactuerName)
            {
            if (setSearchedManufacturer(manufactuerName.Trim()))
                {
                return base.GetCachedObjectId(searchObject);
                }
            return 0;
            }

        /// <summary>
        /// Возвращает кешированные данные производителя по имени и текущему контрагенту
        /// </summary>
        /// <param name="manufactuerName">Имя производителя</param>
        public ManufacturerCacheObject GetManugacturerForName(string manufactuerName)
            {
            if (setSearchedManufacturer(manufactuerName.Trim()))
                {
                return base.GetCachedObject(searchObject);
                }
            return null;
            }

        /// <summary>
        /// Устанавливает поля в объекте поиска, контрагент берется текущий
        /// </summary>
        /// <param name="manufactuerName">Имя производителя</param>
        /// <returns>Было ли значение установлено успешно</returns>
        private bool setSearchedManufacturer(string manufactuerName)
            {
            if (tradeMarkContractorSource == null)
                {
                return false;
                }
            long contractorId = tradeMarkContractorSource.Contractor.Id;
            return setSearchedManufacturer(manufactuerName, contractorId);
            }

        /// <summary>
        /// Устанавливает поля в объекте поиска
        /// </summary>
        /// <param name="manufactuerName">Имя производителя</param>
        /// <param name="contractorId">Ай-ди контрагента</param>
        /// <returns>Было ли значение установлено успешно</returns>
        private bool setSearchedManufacturer(string manufactuerName, long contractorId)
            {
            if (string.IsNullOrEmpty(manufactuerName.Trim()) || contractorId == 0)
                {
                return false;
                }
            searchState.SetSearchOptions(manufactuerName.Trim(), contractorId);
            return true;
            }

        /// <summary>
        /// Создает новый экземпляр кешированных данных производителя
        /// </summary>
        /// <param name="manufacturerName">Имя производителя</param>
        /// <returns>Объект кеша</returns>
        public ManufacturerCacheObject CreateManufacturer(string manufacturerName)
            {
            if (tradeMarkContractorSource == null)
                {
                return null;
                }
            ManufacturerCacheObject manufacturer = new ManufacturerCacheObject(manufacturerName.Trim(), tradeMarkContractorSource.Contractor.Id);
            return manufacturer;
            }

        /// <summary>
        /// Возвращает ID производителя 
        /// </summary>
        /// <param name="manufacturerName">Имя производителя</param>
        /// <param name="currentContractorId">Id контрагента</param>
        /// <returns>Id производителя</returns>
        internal long GetManufcaturerId(string manufacturerName, long currentContractorId)
            {
            if (setSearchedManufacturer(manufacturerName.Trim(), currentContractorId))
                {
                return base.GetCachedObjectId(searchObject);
                }
            return 0;
            }
        }
    }
