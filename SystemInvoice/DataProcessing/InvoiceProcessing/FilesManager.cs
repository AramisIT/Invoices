using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking;
using SystemInvoice.Documents;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.Files;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {
    //Клас - фасад для взаимодействия с файловой системой
    /// <summary>
    /// Выполняет все работу связанную с файлами загрузкой/выгрузкой файлов.
    /// </summary>
    public class FilesManager
        {
        private InvoiceLoadedDocumentHandler loadedDocumentHandler = null;
        private NewDocumentLoader newDocumentLoader = null;
        private DocumentToEditUnloader documentToEditUnloader = null;
        private EditedDocumentLoader editedDocumentLoader = null;
        private ProcessedDocumentUnloader processedDocumentUnloader = null;
        UnloadItemsInfo currentUnloadFileInfo = null;
        SystemInvoiceDBCache dbCache = null;
        InvoiceChecker checker = null;
        private Invoice invoice;

        public FilesManager(Invoice invoice, SystemInvoiceDBCache dbCache, InvoiceChecker invoiceChecker, InvoiceLoadedDocumentHandler loadedDocumentHandler)
            {
            this.invoice = invoice;
            this.dbCache = dbCache;
            this.checker = invoiceChecker;
            newDocumentLoader = new NewDocumentLoader(invoice, dbCache);
            documentToEditUnloader = new DocumentToEditUnloader(invoice, invoiceChecker);
            editedDocumentLoader = new EditedDocumentLoader(invoice, dbCache);
            processedDocumentUnloader = new ProcessedDocumentUnloader(invoice);
            this.loadedDocumentHandler = loadedDocumentHandler;
            }

        /// <summary>
        /// Возвращает количество выгруженных для редактирования строк
        /// </summary>
        public int ToEditUnloadedCount
            {
            get
                {
                return currentUnloadFileInfo == null ? 0 : currentUnloadFileInfo.UnloadedRowsCount;
                }
            }

        /// <summary>
        /// Загружает выгруженный ранее и отредактированный затем файл инвойса. Данные в табличной части заменяются данными в загружаемом файле
        /// </summary>
        /// <param name="filePath">Путь к загружаемому файлу</param>
        public bool TryLoadEditedDocument(string filePath)
            {
            bool isSuccess = this.editedDocumentLoader.TryProcessNewItems(filePath, this.currentUnloadFileInfo);
            this.currentUnloadFileInfo = null;
            return isSuccess;
            }

        /// <summary>
        /// Выгружает обработанный документ в итоговый файл инвойса.
        /// </summary>
        /// <param name="filePath">Путь к сохраняемому файлу</param>
        public bool TrySaveProcessedDocument(string filePath)
            {
            try
                {
                int errorsCount = this.checker.GetTotalErrorsCount();
                if (errorsCount > 0)
                    {
                    string message = string.Format("Вы уверены, что хотите выгрузить данные, в таблице {0} ошибок", errorsCount);
                    if (!message.Ask())
                        {
                        return false;
                        }
                    }
                processedDocumentUnloader.SaveAllItems(filePath);
                }
            catch (Exception e)
                {
                return false;
                }
            return true;
            }

        /// <summary>
        /// Загружает новый документ для обработки, и выгружает строки с новыми позициями/ошибками
        /// </summary>
        /// <param name="openFilePath">Путь к загружаемому файлу</param>
        /// <param name="saveFilePath">Путь к выгружаемому файлу с ошибками</param>
        public bool TryProcessNewDocument(string openFilePath, string saveFilePath)
            {
            if (tryLoadUnprocessedFile(openFilePath))
                {
                bool isRelease = true;
#if DEBUG
                isRelease = false;
#endif
                if (isRelease)
                    {
                    this.checker.CheckTable(true);
                    return invoice.ExcelLoadingFormat.ExportToCheckExcelManually || unloadUnprocessed(saveFilePath, true);
                    }
                else
                    {
                    return false;
                    }
                }
            return false;
            }

        /// <summary>
        /// Выгружает строки с новыми позициями/ошибками в файл для обработки, новые строки и ячейки с ошибками помечены соответствующим цветом
        /// </summary>
        public bool TrySaveErrorsToEdit(string saveFilePath)
            {
            dbCache.RefreshCache();
            return unloadUnprocessed(saveFilePath, false);
            }
        /// <summary>
        /// Выгружает все строки в новый файл, новые строки и ячейки с ошибками помечены соответствующим цветом
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool TrySaveAllToEdit(string filePath)
            {
            dbCache.RefreshCache();
            return (currentUnloadFileInfo = this.documentToEditUnloader.SaveAllItemsToEdit(filePath)) != null;
            }

        private bool unloadUnprocessed(string saveFilePath, bool isAuto)
            {
            return (currentUnloadFileInfo = this.documentToEditUnloader.SaveNewAndUnprocessedItems(saveFilePath, isAuto)) != null;
            }

        private bool tryLoadUnprocessedFile(string filePath)
            {
            DateTime from = DateTime.Now;
            try
                {
                if (!TransactionManager.TransactionManagerInstance.BeginBusinessTransaction())
                    {
                    return false;
                    }
                DataTable created = null;
                if (this.newDocumentLoader.TryCreateTable(filePath, out created))
                    {
                    double loadProcTime = (DateTime.Now - from).TotalMilliseconds;
                    Console.WriteLine("load processing thorought: {0}", loadProcTime);
                    loadedDocumentHandler.TryHandleLoadedTable(created, true);//TryHandleLoadedDocument(true);
                    double handleTime = (DateTime.Now - from).TotalMilliseconds - loadProcTime;
                    Console.WriteLine("handle processing thorought: {0}", handleTime);
                    }
                else
                    {
                    return false;
                    }
                }
            finally
                {
                TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                }
            return true;
            }

        /// <summary>
        /// Загружает обработанный файл. Если непосредственно перед загрузкой из документа был выгружен файл для редактирования, то данные в табличной части документа
        /// будут заменены данными файлами (перед загрузкой документ заблокирован для редактирования вручную). Если файл не выгружался - будут просто созданны новые элементы
        /// справочника из загружаемого файла и редактируемый документ будет обновлен на предмет наличия ошибок ( к примеру было несколько строк с новыми позициями, мы загрузили 
        /// документ с этими позициями, и после загрузки документа и новых позиций в справочник, эти строки больше не отмечены как строки с ошибками).
        /// </summary>
        /// <param name="filePath">Путь к загружаемому файлу</param>
        public void LoadProcessedFile(string filePath)
            {
            DateTime fromTotal = DateTime.Now;
            try
                {
                DateTime from = DateTime.Now;
                if (!TransactionManager.TransactionManagerInstance.BeginBusinessTransaction())
                    {
                    return;
                    }
                this.dbCache.RefreshCache();
                double loadProcTime = (DateTime.Now - from).TotalMilliseconds;
                from = DateTime.Now;
                //    Console.WriteLine("cacheRefresh: {0}", loadProcTime);
                if (TryLoadEditedDocument(filePath))
                    {
                    loadProcTime = (DateTime.Now - from).TotalMilliseconds;
                    //      Console.WriteLine("edited loaded throught: {0}", loadProcTime);
                    from = DateTime.Now;
                    loadedDocumentHandler.TryHandleLoadedDocument();
                    }
                loadProcTime = (DateTime.Now - from).TotalMilliseconds;
                //    Console.WriteLine("handeling throught: {0}", loadProcTime);
                }
            finally
                {
                TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                }
            Console.WriteLine("total throught: {0}", (DateTime.Now - fromTotal).TotalMilliseconds);
            }
        }
    }
