using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using Aramis.DatabaseConnector;

namespace SystemInvoice.DataProcessing.Cache
    {
    /// <summary>
    /// Базовый клас репозитория кешированных объектов. Обеспечивает доступ к объектам хранящим результаты выполнения запросов к БД, а так же выполнения быстрого поиска объектов
    /// с нужными данными, используя индексы (реализации IEqualityComparer<T>). Каждый репозиторий имеет индекс по умолчанию (используются методы GetHashCode и Equals класа object)
    /// Индексы сравнивают объекты только по некоторым полям, поля которые не сравниваются, используются для хранения необходимой информации, которую можна получить используя другой
    /// объект для поиска в котором установлены значения полей/свойств по которым нужно искать информацию. Так же для каждого репозитория присутствует индекс по ID объекта (может представлять
    /// айдишник объекта в БД например)
    /// </summary>
    /// <typeparam name="T">Тип кешированного обхекта</typeparam>
    public abstract class CacheObjectsStore<T> where T : CacheObject<T>
        {
        /// <summary>
        /// Поле которое хранит дату последнего изменения загружаемых объектов из БД, используется для определения актуальности кеша
        /// </summary>
        private DateTime lastModifiedDate = DateTime.MinValue;
        /// <summary>
        /// Поле которое хранит количество обрабатываемых объектов из БД, используется для определения актуальности кеша
        /// </summary>
        protected long lastCachedCount = 0;

        #region Кэши по умолчанию
        /// <summary>
        /// Словарь - кеш по умолчанию, позволяет искать Id объекта
        /// </summary>
        private Dictionary<T, long> cacheObjects = new Dictionary<T, long>();
        /// <summary>
        /// Словарь - кеш по умолчанию, позволяет искать объект по Id
        /// </summary>
        private Dictionary<long, T> objectsForIds = new Dictionary<long, T>();
        /// <summary>
        /// Словарь - кеш по умолчанию, позволяет искать объект по другому объекту (в объекте - ключе могут быть заполнены только поля/свойстав для поиска, в объекте - значении - все поля из БД)
        /// </summary>
        private Dictionary<T, T> selfRefsDictionary = new Dictionary<T, T>();

        #endregion

        #region Кастомные кэши, генерируемые на основании индексов

        //Кеши с соответствием один к одному, делают то же самое что и кеши по умолчанию, но сравнение происходит с использованием объекта - индекса,
        //Сам объект - индекс также используется как ключь для доступа к нужному кешу, поэтому при доступе к кэшу необходимо использовать ТОТ ЖЕ ЕКЗЕМПЛЯР ОБЪЕКТА ЧТО И ПРИ СОЗДАНИИ КЕША
        //Если из БД выбирается несколько объектов которые соответствуют заданному индексу, выбирается произвольно один из них
        private List<IEqualityComparer<T>> oneToOneIndexes = new List<IEqualityComparer<T>>();
        private Dictionary<IEqualityComparer<T>, Dictionary<T, long>> indexedCaches = new Dictionary<IEqualityComparer<T>, Dictionary<T, long>>();
        private Dictionary<IEqualityComparer<T>, Dictionary<long, T>> indexedCachesReversed = new Dictionary<IEqualityComparer<T>, Dictionary<long, T>>();
        //Кеши с соответствием один ко многим (когда нам нужно выбирать из базы несколько записей с одинаковыми значениями по некоторым полям), механизм аналогичный кешам один к одному
        private List<IEqualityComparer<T>> oneToManyIndexes = new List<IEqualityComparer<T>>();
        private Dictionary<IEqualityComparer<T>, Dictionary<T, IEnumerable<long>>> oneToManyindexedCaches = new Dictionary<IEqualityComparer<T>, Dictionary<T, IEnumerable<long>>>();
        private Dictionary<IEqualityComparer<T>, Dictionary<long, T>> oneToManyindexedCachesReversed = new Dictionary<IEqualityComparer<T>, Dictionary<long, T>>();

        #endregion

        /// <summary>
        /// Инициализирует индекси для кеширования в соотношении один к одному
        /// </summary>
        /// <param name="indexes">Набор индексов</param>
        protected virtual void InitializeIndexes(List<IEqualityComparer<T>> indexes)
            {
            }

        /// <summary>
        /// Инициализирует индекси для кеширования в соотношении один ко многим
        /// </summary>
        /// <param name="indexes">Набор индексов</param>
        protected virtual void InitializeOneToManyIndexes(List<IEqualityComparer<T>> indexes)
            {

            }
        /// <summary>
        /// Текст запроса возвращающий данные для построения кеша, в возвращаемых результатах обязательно должна присутствувать колонка Id
        /// </summary>
        protected abstract string SelectQuery
            {
            get;
            }
        /// <summary>
        /// Текст запроса возвращающего последнюю дату изменений данных, используется для определения актуальности кеша и необходимости его обновления
        /// </summary>
        protected abstract string LatModifiedDateQuery
            {
            get;
            }
        /// <summary>
        /// Текст запроса возвращающего количество обрабатываемых объектов, используется для определения актуальности кеша и необходимости его обновления
        /// </summary>
        protected abstract string LastProcessedCountQuery
            {
            get;
            }

        private long GetMayCachedCount()
            {
            Query query = DB.NewQuery(LastProcessedCountQuery);
            object result = query.SelectScalar();
            if (result is long)
                {
                return (long)result;
                }
            if (result is int)
                {
                return (int)result;
                }
            return lastCachedCount + 1;
            }

        protected virtual DateTime GetLastModifiedDate()
            {
            Query query = DB.NewQuery(LatModifiedDateQuery);
            object result = query.SelectScalar();
            if (result is DateTime)
                {
                return (DateTime)result;
                }
            return DateTime.MaxValue;
            }


        private bool isHaveToBeUpdated()
            {
            DateTime currentDBLastModifiedDate = GetLastModifiedDate();
            long mayCachedCount = GetMayCachedCount();
            if (lastModifiedDate < currentDBLastModifiedDate || lastCachedCount != mayCachedCount)
                {
                lastModifiedDate = currentDBLastModifiedDate;
                lastCachedCount = mayCachedCount;
                return true;
                }
            return false;
            }

        /// <summary>
        /// Создает новый объект для кеширования
        /// </summary>
        /// <param name="row">Строка содержащая результаты выполнения запроса</param>
        /// <returns></returns>
        protected abstract T createNew(DataRow row);

        /// <summary>
        /// Обновляет кеш, если данные в нем устарели
        /// </summary>
        public virtual void Refresh()
            {

#if DEBUG
            if (!TransactionManager.TransactionManagerInstance.IsInTransaction())//должно выполнятся в бизнес - транзакции для того что бы у нас были валидные данные
                {
                throw new TransactionManager.TransactionNotGetException("Невозможно загрузить данные");
                }
#endif
            try
                {
                if (!isHaveToBeUpdated())
                    {
                    return;
                    }
                initDefaultCache();
                initCustomCaches();
                using (DataTable cacheTable = createCacheTable())
                    {
                    if (cacheTable == null)
                        {
                        lastModifiedDate = DateTime.MinValue;
                        return;
                        }
                    List<T> allObjects;
                    List<long> allObjectsIds;
                    fillDefaultCache(cacheTable, out allObjects, out allObjectsIds);
                    fillCustomIndexedCaches(allObjects, allObjectsIds);
                    allObjects.Clear();
                    allObjectsIds.Clear();
                    }
                }
            catch (Exception e)
                {
                lastModifiedDate = DateTime.MinValue;
                throw e;
                }
            }

        private void fillDefaultCache(DataTable cacheTable, out List<T> allObjects, out List<long> allObjectsIds)
            {
            allObjects = new List<T>();
            allObjectsIds = new List<long>();
            foreach (DataRow row in cacheTable.Rows)
                {
                T cacheObject = createNew(row);
                long objectId = row.TryGetColumnValue<long>("Id", 0);
                allObjects.Add(cacheObject);
                allObjectsIds.Add(objectId);
                if (objectId != 0 && !cacheObjects.ContainsKey(cacheObject))
                    {
                    cacheObjects.Add(cacheObject, objectId);
                    if (!objectsForIds.ContainsKey(objectId))
                        {
                        objectsForIds.Add(objectId, cacheObject);
                        }
                    selfRefsDictionary.Add(cacheObject, cacheObject);
                    }
                }
            }

        private void initDefaultCache()
            {
            cacheObjects.Clear();
            objectsForIds.Clear();
            selfRefsDictionary.Clear();
            }

        private void initCustomCaches()
            {
            initOneToOneCaches();
            initOneToManyCaches();
            }

        private void initOneToManyCaches()
            {
            foreach (var item in oneToManyindexedCaches)
                {
                item.Value.Clear();
                }
            foreach (var item in oneToManyindexedCachesReversed)
                {
                item.Value.Clear();
                }
            oneToManyindexedCaches.Clear();
            oneToManyindexedCachesReversed.Clear();
            oneToManyIndexes.Clear();
            this.prepareOneToManyCustomCaches();
            }

        private void initOneToOneCaches()
            {
            foreach (var item in indexedCaches)
                {
                item.Value.Clear();
                }
            foreach (var item in indexedCachesReversed)
                {
                item.Value.Clear();
                }
            indexedCaches.Clear();
            indexedCachesReversed.Clear();
            oneToOneIndexes.Clear();
            this.prepareOneToOneCustomCaches();
            }

        private void prepareOneToManyCustomCaches()
            {
            this.InitializeOneToManyIndexes(oneToManyIndexes);
            foreach (IEqualityComparer<T> indexer in oneToManyIndexes)
                {
                this.oneToManyindexedCaches.Add(indexer, new Dictionary<T, IEnumerable<long>>(indexer));
                this.oneToManyindexedCachesReversed.Add(indexer, new Dictionary<long, T>());
                }
            }

        private void prepareOneToOneCustomCaches()
            {
            this.InitializeIndexes(oneToOneIndexes);
            foreach (IEqualityComparer<T> indexer in oneToOneIndexes)
                {
                this.indexedCaches.Add(indexer, new Dictionary<T, long>(indexer));
                this.indexedCachesReversed.Add(indexer, new Dictionary<long, T>());
                }
            }

        private void fillCustomIndexedCaches(List<T> allObjects, List<long> allObjectsIds)
            {
            fllOneToOneCahces(allObjects, allObjectsIds);
            fillOneToManyCaches(allObjects, allObjectsIds);
            }

        private void fillOneToManyCaches(List<T> allObjects, List<long> allObjectsIds)
            {
            foreach (KeyValuePair<IEqualityComparer<T>, Dictionary<T, IEnumerable<long>>> keyValuePair in this.oneToManyindexedCaches)
                {
                IEqualityComparer<T> indexer = keyValuePair.Key;
                Dictionary<T, IEnumerable<long>> cache = keyValuePair.Value;
                Dictionary<long, T> cacheReversed = oneToManyindexedCachesReversed[indexer];
                for (int i = 0; i < allObjects.Count; i++)
                    {
                    T cacheObject = allObjects[i];
                    long cacheObjectId = allObjectsIds[i];
                    IEnumerable<long> cachedContent = null;
                    if (cache.TryGetValue(cacheObject, out cachedContent))
                        {
                        ((List<long>)cachedContent).Add(cacheObjectId);
                        }
                    else
                        {
                        cache.Add(cacheObject, new List<long>() { cacheObjectId });
                        }
                    if (!cacheReversed.ContainsKey(cacheObjectId))
                        {
                        cacheReversed.Add(cacheObjectId, cacheObject);
                        }
                    }
                }
            }

        private void fllOneToOneCahces(List<T> allObjects, List<long> allObjectsIds)
            {
            foreach (KeyValuePair<IEqualityComparer<T>, Dictionary<T, long>> keyValuePair in this.indexedCaches)
                {
                IEqualityComparer<T> indexer = keyValuePair.Key;
                Dictionary<T, long> cache = keyValuePair.Value;
                Dictionary<long, T> cacheReversed = indexedCachesReversed[indexer];
                for (int i = 0; i < allObjects.Count; i++)
                    {
                    T cacheObject = allObjects[i];
                    long cacheObjectId = allObjectsIds[i];
                    if (cacheObjectId != 0 && !cache.ContainsKey(cacheObject))
                        {
                        cache.Add(cacheObject, cacheObjectId);
                        }
                    if (!cacheReversed.ContainsKey(cacheObjectId))
                        {
                        cacheReversed.Add(cacheObjectId, cacheObject);
                        }
                    }
                }
            }


        private DataTable createCacheTable()
            {
            var queryText = SelectQuery;

            if (!string.IsNullOrEmpty(queryText))
                {
                Query query = DB.NewQuery(queryText);
                DataTable table = query.SelectToTable();
                return table;
                }

            return null;
            }

        /// <summary>
        /// Возвращает кешированный Id объекта, с использованием индекса один к одному
        /// </summary>
        /// <param name="indexer">Экземпляр индекса</param>
        /// <param name="cachedObject">Объект который содержит заполненные поля по которым происходит поиск в кеше</param>
        /// <returns>Id объекта</returns>
        protected long GetCachedObjectId(IEqualityComparer<T> indexer, T cachedObject)
            {
            long result = 0;
            if (indexedCaches.ContainsKey(indexer) && indexedCaches[indexer] != null)
                {
                indexedCaches[indexer].TryGetValue(cachedObject, out result);
                }
            return result;
            }

        /// <summary>
        /// Возвращает кешированные Id - шники объектов, с использованием индекса один ко многим
        /// </summary>
        /// <param name="indexer">Экземпляр индекса</param>
        /// <param name="cachedObject">Объект который содержит заполненные поля по которым происходит поиск в кеше</param>
        /// <returns>Id - шники объектов</returns>
        protected IEnumerable<long> GetCachedObjectIds(IEqualityComparer<T> indexer, T searchObject)
            {
            Dictionary<T, IEnumerable<long>> cache;
            if (oneToManyindexedCaches.TryGetValue(indexer, out cache) && cache != null)
                {
                IEnumerable<long> result;
                if (cache.TryGetValue(searchObject, out result))
                    {
                    return result;
                    }
                }
            return null;
            }

        /// <summary>
        /// Возвращает объект - ключ, в кеше один ко многим
        /// </summary>
        /// <param name="indexer">индекс</param>
        /// <param name="Id">Id объекта - ключа</param>
        /// <returns>Экземпляр ключевого объекта</returns>
        protected T GetFromOneToManyCachedObject(IEqualityComparer<T> indexer, long Id)
            {
            return getCustomCachedObjectById(indexer, Id, oneToManyindexedCachesReversed);
            }

        /// <summary>
        /// Возвращает кешированный объект из кеша типа - один к одному
        /// </summary>
        /// <param name="indexer">Индекс</param>
        /// <param name="Id">id объекта</param>
        /// <returns>Экземпляр кешированного объекта</returns>
        protected T GetCachedObject(IEqualityComparer<T> indexer, long Id)
            {
            return getCustomCachedObjectById(indexer, Id, indexedCachesReversed);
            }

        /// <summary>
        /// Возвращает значение объекта из словаря кешей
        /// </summary>
        /// <param name="indexer">Индекс по которому ищется нужный кеш и происходит поиск в самом кеше</param>
        /// <param name="Id">Id кешированного объекта</param>
        /// <param name="searchedStore">Словарь кешей</param>
        /// <returns>Экземпляр кешированного объекта</returns>
        private static T getCustomCachedObjectById(IEqualityComparer<T> indexer, long Id, Dictionary<IEqualityComparer<T>, Dictionary<long, T>> searchedStore)
            {
            T result = null;
            Dictionary<long, T> cacheReversed = null;
            if (searchedStore.TryGetValue(indexer, out cacheReversed))
                {
                cacheReversed.TryGetValue(Id, out result);
                }
            return result;
            }

        /// <summary>
        /// Возвращает Id кешированного объекта или 0 - если объект не найдет
        /// </summary>
        /// <param name="cachedObject">Объект по полям которому ищется айдишник кешированного объекта</param>
        public long GetCachedObjectId(T cachedObject)
            {
            long result = 0;
            cacheObjects.TryGetValue(cachedObject, out result);
            return result;
            }

        /// <summary>
        /// Возвращает екземпляр кешированного объекта по айдишнику если такого объекта не существует возвращает null
        /// </summary>
        /// <param name="Id">Id</param>
        public T GetCachedObject(long Id)
            {
            T result = null;
            objectsForIds.TryGetValue(Id, out result);
            return result;
            }

        /// <summary>
        /// Возвращает кешированный объект на основнии другого объекта, с такими же полями используемыми при сравнении объетов на равенство
        /// </summary>
        /// <param name="other">Объект с настроенными полями для поиска кешированного объекта</param>
        /// <returns>Экземпляр кешированного объекта</returns>
        public T GetCachedObject(T other)
            {
            T result = null;
            selfRefsDictionary.TryGetValue(other, out result);
            return result;
            }
        }
    }
