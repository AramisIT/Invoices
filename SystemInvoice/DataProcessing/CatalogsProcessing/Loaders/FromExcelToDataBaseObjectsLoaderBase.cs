using System;
using System.Collections.Generic;
using System.Linq;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using Aramis.Core;
using Aramis.DatabaseConnector;
using AramisWpfComponents.Excel;
using SystemInvoice.Excel;
using SystemInvoice.Excel.DataFormatting.Formatters;
using SystemInvoice.DataProcessing.CatalogsProcessing.NotificationWindow;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Базовый клас для загрузки справочника из ексель - файла. 
    /// </summary>
    /// <typeparam name="T">Тип справочника который мы загружаем в систему</typeparam>
    public abstract class FromExcelToDataBaseObjectsLoaderBase<T> where T : DatabaseObject, new()
        {
        ExcelMapper mapper = new ExcelMapper();
        ObjectLoader<T> loader = new ObjectLoader<T>();
        protected SystemInvoiceDBCache cachedData = null;

        public FromExcelToDataBaseObjectsLoaderBase(SystemInvoiceDBCache cachedData)
            {
            this.cachedData = cachedData;
            InitializeMapping(mapper);
            }
        /// <summary>
        /// Добавляет привязку свойства объекта к колонке в ексель - файле
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="excelColumnIndex">Индекс колонки. Начальный отсчет с 1 (для того что бы брать индексы отображаемые в MS Excel.</param>
        protected void AddPropertyMapping(string propertyName, int excelColumnIndex)
            {
            mapper.TryAddExpression(propertyName, "index", excelColumnIndex.ToString());
            }

        /// <summary>
        /// Добавляет привязку свойства объекта к результату выполнения функции
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="processingFunc">функция</param>
        protected void AddCustomMapping(string propertyName, Func<Row, object> processingFunc)
            {
            loader.RegisterFormatter(propertyName, new CustomExcelRowProcessingFormatterConstructor(processingFunc), true);
            mapper.TryAddExpression(propertyName, propertyName, propertyName);
            }

        /// <summary>
        /// Загружает справочник
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="startIndex">начальный индекс</param>
        /// <param name="loadedCount">колличество загруженных строк</param>
        /// <param name="nonLoadedCount">колличество незагруженных строк</param>
        public bool TryLoad(string fileName, int startIndex, out int loadedCount, out int nonLoadedCount)
            {
            loadedCount = 0;
            nonLoadedCount = 0;
            bool successed = false;
            IEnumerable<T> items = loader.Transform(mapper, fileName, startIndex, out successed);//создаем екземпляры объектов справочников
            if (successed && items != null)
                {
                if (UploadNotificationWindow.Window != null)
                    {
                    UploadNotificationWindow.Window.TotalCount = items.Count();
                    UploadNotificationWindow.Window.Current = 0;
                    }
                foreach (T item in items)
                    {
                    if (checkBeforeCreate(item))
                        {
                        if (item.Write() == WritingResult.Success)
                            {
                            loadedCount++;
                            }
                        else
                            {
                            nonLoadedCount++;
                            }
                        }
                    if (UploadNotificationWindow.Window != null)//обновляем информацию об обработанных строках в информационном окне
                        {
                        UploadNotificationWindow.Window.Current++;
                        }
                    }
                return true;
                }
            return false;
            }

        private bool checkBeforeCreate(T item)
            {
            return CheckItemBegoreCreate(item);
            }
        /// <summary>
        /// Выполняет проверку объекта перед записью, и устанавливает при необходимости дополнительные свойства
        /// </summary>
        /// <param name="itemToCheck">Элемент который необходимо проверить</param>
        protected abstract bool CheckItemBegoreCreate(T itemToCheck);

        /// <summary>
        /// Заполняет привязку полей объекта к колонкам Excel - файла
        /// </summary>
        protected abstract void InitializeMapping(ExcelMapper mapper);

        /// <summary>
        /// Возвращает начальный индекс строки с которого нужно выполнить загрузку
        /// </summary>
        protected virtual int StartRowIndex { get { return 1; } }

        /// <summary>
        /// Выполняет загрузку справочника
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
            {
            bool isInCurrentTransaction = TransactionManager.TransactionManagerInstance.IsInTransaction();
            try
                {
                if (!isInCurrentTransaction)
                    {//Начинаем транзакцию, что бы не было ситуации когда несколько пользователей одновременно добавят одни и теже данные
                    TransactionManager.TransactionManagerInstance.BeginBusinessTransaction();
                    }
                if (!this.OnLoadBegin())
                    {
                    "Загрузка отменена.".AlertBox();
                    return;
                    }
                UploadNotificationWindow.ShowWindow();
                int loaded, notLoaded;
                bool isLoaded = TryLoad(fileName, StartRowIndex, out loaded, out notLoaded);
                UploadNotificationWindow.CloseWindow();
                if (isLoaded)
                    {
                    string.Format("Загружено {0} элемент(ов) справочника из входящего файла. Не загруженно {1}.", loaded, notLoaded).AlertBox();
                    }
                this.OnLoadComplete();
                }
            catch (Exception e)
                {
                Console.WriteLine(e.ToString());
                "Ошибка при обработке Excel - файла".AlertBox();
                }
            finally
                {
                UploadNotificationWindow.CloseWindow();
                if (!isInCurrentTransaction)
                    {
                    TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                    }
                }
            }
        /// <summary>
        /// Вызывается перед загрузкой.
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnLoadBegin()
            {
            return true;
            }
        /// <summary>
        /// Вызывается после загрузки
        /// </summary>
        protected virtual void OnLoadComplete() { }
        }
    }
