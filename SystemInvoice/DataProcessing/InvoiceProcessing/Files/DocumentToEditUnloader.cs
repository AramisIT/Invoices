using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using Aramis.DatabaseConnector;
using Aramis.Core;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Files
    {
    /// <summary>
    /// Выгружает табличную часть инвойса для редактирования
    /// </summary>
    public class DocumentToEditUnloader : DocumentUnloaderBase
        {
        /// <summary>
        /// Используется для проверки наличия ошибок, новой номенклатуры
        /// </summary>
        private InvoiceChecker invoiceChecker = null;
        /// <summary>
        /// Хранит информацию о соответствии выгруженных строк, строкам в исходной табличной части инфойса
        /// </summary>
        private Dictionary<DataRow, int> newRowsIdnexes = new Dictionary<DataRow, int>();

        public DocumentToEditUnloader( Invoice invoice, InvoiceChecker invoiceChecker )
            : base( invoice )
            {
            this.Unloader = new AggregatingTableUnloader( string.Empty, false );
            this.invoiceChecker = invoiceChecker;
            }

        #region Выгрузка необрабатываемых строк

        protected override string UnloadIndexColumnName
            {
            get { return ProcessingConsts.EXCEL_UNLOAD_UNPROCESSED_COLUMNS_INDEX_MAPPING; }
            }

        /// <summary>
        /// Выгружает строки с новыми позициями и строки с ошибками
        /// </summary>
        /// <param name="fileName">Путь к сохраняемому файлу</param>
        /// <param name="isAuto">Является ли выгрузка автоматической (при загрузке необработанного документа) или инициирована пользователем (по нажатию кнопки выгрузить).</param>
        /// <returns>Информация о выгруженных строках</returns>
        public UnloadItemsInfo SaveNewAndUnprocessedItems( string fileName, bool isAuto )
            {
            return unloadItems( fileName, isAuto, false );
            }

        /// <summary>
        /// Выгружает все строки табличной части инфойса
        /// </summary>
        /// <param name="fileName">Путь к сохраняемому файлу</param>
        /// <returns>Информация о выгруженных строках</returns>
        public UnloadItemsInfo SaveAllItemsToEdit( string fileName )
            {
            return unloadItems( fileName, true, true );
            }

        /// <summary>
        /// Выгружает табличную часть инвойса в файл
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        /// <param name="isAuto">Является ли выгрузка автоматической</param>
        /// <param name="unloadAll">Выгружать все</param>
        /// <returns>Информация о выгруженных строках</returns>
        private UnloadItemsInfo unloadItems( string fileName, bool isAuto, bool unloadAll )
            {
            Dictionary<int, int> unloadedIndexesDict = new System.Collections.Generic.Dictionary<int, int>();
            DataTable newItemsTable = this.getItems( this.Invoice.Goods, unloadedIndexesDict, unloadAll );
            if (newItemsTable.Rows.Count == 0)
                {
                if (!isAuto)
                    {
                    "Новых элементов не обнаружено".AlertBox();
                    }
                return null;
                }
            base.SaveTable( fileName, newItemsTable, 2 );
            UnloadItemsInfo lastUploadFilesInfo = new UnloadItemsInfo( unloadedIndexesDict, this.Invoice.Goods.Rows.Count );
            return lastUploadFilesInfo;
            }

        /// <summary>
        /// Получает выгружаемую таблицу из табличной части инвойса, сохраняет информацию о соответствии выгруженной таблицы и исходной
        /// </summary>
        private DataTable getItems( DataTable dataTable, Dictionary<int, int> unloadedIndexesDict, bool unloadAll )
            {
            newRowsIdnexes.Clear();
            DataTable newItemsTable = dataTable.Clone();
            int currentNewRowIndex = 0;
            int processingRowsCount = dataTable.Rows.Count;
            for (int i = 0; i < processingRowsCount; i++)
                {
                DataRow row = dataTable.Rows[i];
                long foundedNomenclatureId = row.TrySafeGetColumnValue<long>( "FoundedNomenclature", 0 );
                bool isChecked = invoiceChecker.IsRowValid( i );
                if (unloadAll || (foundedNomenclatureId == 0 || !isChecked))
                    {
                    DataRow newRow = newItemsTable.CopyRow( row );
                    unloadedIndexesDict.Add( i, currentNewRowIndex++ );
                    newRowsIdnexes.Add( newRow, i );
                    }
                }
            return newItemsTable;
            }

        protected override string OnGetUnloadCellColor( DataRow row, string columnName, ExcelMapper mapper )
            {            
            if (newRowsIdnexes.ContainsKey( row ))
                {
                return invoiceChecker.GetCellCollor( newRowsIdnexes[row], columnName );
                }
            return null;
            }
        #endregion
        }
    }
