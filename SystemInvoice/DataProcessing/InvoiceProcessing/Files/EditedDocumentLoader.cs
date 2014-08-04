using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.ApprovalsModification;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.GroupOfGoodsCreation;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using Aramis.DatabaseConnector;
using Aramis.Core;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Files
    {
    /// <summary>
    /// Загружает отредактированный документ в табличную часть инвойса
    /// </summary>
    public class EditedDocumentLoader
        {
        private DataTable currentProcessedTable = null;
        private SystemInvoiceDBCache cachedData = null;
        private Invoice invoice = null;
        private ApprovalsFromInvoiceCreator fromInvoiceCreator = null;
        private CatalogsLoader catalogsLoader = null;

        private ExcelLoadingFormat loadingFormat
            {
            get { return invoice.ExcelLoadingFormat; }
            }

        public EditedDocumentLoader(Invoice invoice, SystemInvoiceDBCache cachedData)
            {
            this.invoice = invoice;
            this.cachedData = cachedData;
            this.fromInvoiceCreator = new ApprovalsFromInvoiceCreator(cachedData);
            this.catalogsLoader = new CatalogsLoader(cachedData, invoice);
            if (invoice == null)
                {
                throw new NotImplementedException("Oтсутствует формат загрузки");
                }
            }

        /// <summary>
        /// Формирует привязку колонок загружаемого файла к колонкам таблицы, в кторую загружаются данные
        /// </summary>
        private ExcelMapper createMapper()
            {
            ExcelMapper mapper = new ExcelMapper();
            foreach (DataRow row in loadingFormat.ColumnsMappings.Rows)
                {
                int columnIndex = 0;
                int columnNameIndex = row.TryGetColumnValue<int>(ProcessingConsts.EXCEL_LOAD_FORMAT_TARGET_COLUMN_COLUMN_NAME, -1);
                string unloadIndex = row.TryGetColumnValue<string>(ProcessingConsts.EXCEL_UNLOAD_UNPROCESSED_COLUMNS_INDEX_MAPPING, "");
                if (columnNameIndex >= 0 && !string.IsNullOrEmpty(unloadIndex))
                    {
                    string columnName = ((InvoiceColumnNames)columnNameIndex).ToString();
                    if (int.TryParse(unloadIndex, out columnIndex))
                        {
                        mapper.TryAddExpression(columnName, ExcelMapper.IndexKey, unloadIndex);
                        }
                    }
                }
            return mapper;
            }

        #region Загрузка обработанных элементов
        /// <summary>
        /// Загружает обработанный документ, который был выгружен ранее для этого же инвойса
        /// </summary>
        /// <param name="fileName">Путь к загружаемому файлу</param>
        /// <param name="lastUploadFilesInfo">Информация о выгруженом файле перед его ручной обработкой</param>
        public bool TryProcessNewItems(string fileName, UnloadItemsInfo lastUploadFilesInfo)
            {
            DateTime from = DateTime.Now;
            if (!checkCatalogs(fileName))
                {
                return false;
                }
            Console.WriteLine((DateTime.Now - from).TotalMilliseconds);
            from = DateTime.Now;
            if (refreshExistedTable(lastUploadFilesInfo))
                {
                Console.WriteLine((DateTime.Now - from).TotalMilliseconds);
                from = DateTime.Now;
                fromInvoiceCreator.ModifyApprovalsCatalog(this.invoice.Goods);
                Console.WriteLine((DateTime.Now - from).TotalMilliseconds);
                from = DateTime.Now;
                return true;
                }
            return false;
            }

        /// <summary>
        /// Загружает документ и создает новые элемнеты в справочниках
        /// </summary>
        /// <param name="fileName">путь к загружаемому файлу</param>
        private bool checkCatalogs(string fileName)
            {
            IContractor currentContractor = this.invoice.Contractor;
            TableLoader loader = new TableLoader();
            cachedData.TradeMarksCreator.Refresh();
            cachedData.ManufacturersCreator.Refresh();
            try
                {
                ExcelMapper mapper = this.createMapper();
                bool processed = false;
                int startLoadIndex = 2;

                if (!checkLoadingInfo(currentContractor))
                    {
                    return false;
                    }
                if (!tryLoad(fileName, loader, mapper, ref processed, startLoadIndex))
                    {
                    return false;
                    }
                return true;// catalogsLoader.TryCreateNewCatalogsItems(currentProcessedTable);
                }
            catch (Cache.TradeMarksCache.TradeMarkCacheObjectsStore.TradeMarkConflictException conflictExceprion)
                {
                "Неправильный формат загрузки. Торговая марка не может быть в загружаемом файле, если она указана в самом формате.".AlertBox();
                return false;
                }
            }

        private bool tryLoad(string fileName, TableLoader loader, ExcelMapper mapper, ref bool processed, int startLoadIndex)
            {
            currentProcessedTable = loader.Transform(mapper, fileName, startLoadIndex, out processed);
            if (!processed)
                {
                return false;
                }
            return true;
            }

        private bool checkLoadingInfo(IContractor currentContractor)
            {
            if (currentContractor.Id == 0)
                {
                "Необходимо выбрать производителя".AlertBox();
                return false;
                }
            if (this.invoice.ExcelLoadingFormat.Id == 0)
                {
                "Необходимо выбрать формат загрузки".AlertBox();
                return false;
                }
            return true;
            }
        /// <summary>
        /// Обновляет информацию в табличной части инвойса, данными загруженными из входящего файла
        /// </summary>
        private bool refreshExistedTable(UnloadItemsInfo lastUploadFilesInfo)
            {
            if (lastUploadFilesInfo == null)
                {
                return true;
                }
            Dictionary<int, int> uploadMappings = lastUploadFilesInfo.unloadRowsMappings;
            DataTable tableToUpdate = this.invoice.Goods;
            int rowsCount = tableToUpdate.Rows.Count;
            if (lastUploadFilesInfo == null || rowsCount != lastUploadFilesInfo.TargetTableRowsCount)
                {
                return false;
                }
            foreach (int rowToUpdateIndex in uploadMappings.Keys)
                {
                int processedTableIndex = uploadMappings[rowToUpdateIndex];
                if (rowToUpdateIndex >= rowsCount || processedTableIndex >= currentProcessedTable.Rows.Count)
                    {
                    break;
                    }
                DataRow rowToUpdate = tableToUpdate.Rows[rowToUpdateIndex];
                DataRow rowFromUpdate = currentProcessedTable.Rows[processedTableIndex];
                foreach (DataColumn column in currentProcessedTable.Columns)
                    {
                    if (tableToUpdate.Columns.Contains(column.ColumnName))
                        {
                        rowToUpdate[column.ColumnName] = rowFromUpdate[column.ColumnName];
                        }
                    }
                }
            return true;
            }

        #endregion

        }
    }
