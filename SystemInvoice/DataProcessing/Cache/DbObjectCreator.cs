using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.DatabaseConnector;

namespace SystemInvoice.DataProcessing.Cache
    {
    /// <summary>
    /// Базовый клас, создающий новые екземпляры документов/справочников в системе и записывающий их в БД
    /// </summary>
    /// <typeparam name="D">Тип создаваемого объекта</typeparam>
    /// <typeparam name="C">Тип объекта - кеша, данные которого используются для создания документа/справочника</typeparam>
    public abstract class DbObjectCreator<D, C>
        where D : IDatabaseObject
        where C : CacheObject<C>
        {
        /// <summary>
        /// Кеш используемый для определения того, существует ли уже такой документ/справочник в системе. (При этом проверка осуществляется согласно алгоритму в кеше а не согласно ограничениям БД)
        /// </summary>
        private CacheObjectsStore<C> cacheStore = null;

        public DbObjectCreator(CacheObjectsStore<C> cacheStore)
            {
            this.cacheStore = cacheStore;
            }
        /// <summary>
        /// Список объектов которые удлось создать
        /// </summary>
        private List<D> createdObjects = new List<D>();
        /// <summary>
        /// Список объектов кеша на основании которых создаются документы/справочники
        /// </summary>
        private HashSet<C> cachedObjectsToCreate = new HashSet<C>();

        /// <summary>
        /// Создает екземпляр объекта документа/справочника, который необходимо записать в БД
        /// </summary>
        /// <param name="cacheObject">Кешированный объект данные которого используются для создания объекта документа/справочника</param>
        protected abstract D createDBObject(C cacheObject);
        /// <summary>
        /// Осуществляет удаление объекта и связанных с ним объектов из БД
        /// </summary>
        protected abstract void deleteObject(D objectToDelete);
        /// <summary>
        /// Сообщение об ошибке создания объектов при неудасной попытке записать все объеты в БД
        /// </summary>
        /// <param name="failCount">Количество объектов, запись которых в БД завершилась ошибкой</param>
        protected abstract string failToCreateMessage(int failCount);

        private int create()
            {
            if (createdObjects.Count > 0)//в принципе такое не может быть вызвано, но на всякий случай я делаю эту проверку, что бы убедится, что в случае неудачи мы не удалим "лишние" объекты
                {
                throw new NotImplementedException("Обнаружены созданные ранее объекты");
                }
            int createdSuccess = 0;
            foreach (C item in cachedObjectsToCreate)
                {
                if (item == null)
                    {
                    continue;
                    }
                D dataBaseObjectItem = createDBObject(item);
                if (dataBaseObjectItem == null)
                    {
                    continue;
                    }
                if (dataBaseObjectItem.Write() == WritingResult.Success)
                    {
                    createdObjects.Add(dataBaseObjectItem);
                    createdSuccess++;
                    }
                else
                    {
                    int i = 0;
                    }
                }
            return createdSuccess;
            }
        /// <summary>
        /// Выполняет скалярный запрос к БД
        /// </summary>
        /// <param name="queryText"></param>
        protected void ExceuteQuery(string queryText)
            {
            Query query = DB.NewQuery(queryText);
            query.Execute();
            }

        /// <summary>
        /// Вызывается перед началом процесса создания объектов
        /// </summary>
        protected virtual void BeginTransaction()
            {

            }
        /// <summary>
        /// Вызывается в случае успешного создания объектов
        /// </summary>
        protected virtual void CommitTransaction()
            {
            Refresh();
            }

        /// <summary>
        /// Выполняет удаление созданных объектов
        /// </summary>
        protected virtual void RollBackTransaction()
            {
            if (TransactionManager.TransactionManagerInstance.IsInTransaction())
                {
                DeleteCreated();
                Refresh();
                }
            else
                {
#if DEBUG
                throw new TransactionManager.TransactionNotGetException( "Удаление объектов невозможно." );
#endif
                }
            }

        /// <summary>
        /// Создает объекты справочников/документов на основании добавленных ранее в экземпляр "создателя" объектов кеша
        /// </summary>
        public bool TryCreate()
            {
            //Все операции по созданию/уничтожению объектов должны выплнятся в блокирующей транзакции, для того что бы быть уверенными что
            //у нас не возникнет ситуации, когда два пользователя одновременно создадут 2 одинаковых объекта, и один из них решит его удалить, 
            //в то время как другой будет думать что он был создан успешно и создаст на удаленный объект ссылки в других документах. Кроме того,
            //выполнение в транзакции, гарантирует нам что в момент автоматической обработки документа (которая тоже выполняется в транзакции),
            //у нас будут обрабатыватся только те данные которые не будут удалены из базы в случае если создание объектов завершится неудачей и пользователь
            //решит откатить изменения. Нужно учитывать очень важный факт!!! - время одной транзакции ограниченно переменной TRANSACTION_MAX_ALIVE_PERIOD в TransactionManager
            //для того что бы не блокировать слишком долго работу остальных пользователей, соответственно, если пользователь не успеет принтять решение по удалению объектов
            //в течение этого времени, транзакция автоматически завершится, и созданные объекты не смогут быть удаленны из системы (поскольку другие пользователи могут
            //установить на них ссылки из других документов/справочников), такие объекты в случае необходимости нужно будет удалять из системы вручную.
            if (!TransactionManager.TransactionManagerInstance.IsInTransaction())
                {
#if DEBUG
                throw new TransactionManager.TransactionNotGetException( "Создание объектов невозможно." );
#endif
                return false;
                }
            try
                {
                BeginTransaction();
                int itemsToHaveCreateCount = cachedObjectsToCreate.Count;
                int createdCount = create();
                int nonCreated = itemsToHaveCreateCount - createdCount;
                if (nonCreated > 0)
                    {
                    string ask = string.Concat(failToCreateMessage(nonCreated), " Продолжить загрузку?");
                    if (ask.Ask())
                        {
                        CommitTransaction();
                        return true;
                        }
                    else
                        {
                        RollBackTransaction();
                        return false;
                        }
                    }
                CommitTransaction();
                }
            catch
                {
                RollBackTransaction();
                return false;
                }
            return true;
            }

        public void DeleteCreated()
            {
            foreach (D dbObject in createdObjects)
                {
                deleteObject(dbObject);
                }
            }

        public void Refresh()
            {
            cacheStore.Refresh();
            createdObjects.Clear();
            cachedObjectsToCreate.Clear();
            }

        /// <summary>
        /// Добавляет объект кеша на основании которого создается записываемый в БД объект документа/справочника
        /// </summary>
        public bool TryAddToCreationList(C newItem)
            {
            if (CheckCanAddItem(newItem) && !cachedObjectsToCreate.Contains(newItem))
                {
                cachedObjectsToCreate.Add(newItem);
                return true;
                }
            return false;
            }

        /// <summary>
        /// Проверяет можно ли записывать елемент документа/справочника в БД
        /// </summary>
        protected virtual bool CheckCanAddItem(C newItem)
            {
            return cacheStore.GetCachedObjectId(newItem) == 0;
            }

        /// <summary>
        /// Количество успешно записанных объектов
        /// </summary>
        public int ObjectsToCreateCount
            {
            get
                {
                return cachedObjectsToCreate.Count;
                }
            }

        }
    }
