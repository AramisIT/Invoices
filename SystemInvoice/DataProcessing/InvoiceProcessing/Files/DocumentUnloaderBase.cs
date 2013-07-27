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

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Files
    {
    /// <summary>
    /// Базовый клас для выгрузки табличной части инвойса в эксель
    /// </summary>
    public abstract class DocumentUnloaderBase
        {
        /// <summary>
        /// Хранит выгружаемую часть инвойса и формат загрузки
        /// </summary>
        protected Invoice Invoice { get; private set; }
        /// <summary>
        /// Осуществляет сохранение таблицы в файл
        /// </summary>
        protected TableUnloader Unloader = new TableUnloader();
        /// <summary>
        /// Хранит привязуку колонок таблицы к колонкам в выгружаемом файле
        /// </summary>
        private ExcelMapper mapper = null;
        /// <summary>
        /// Хранит текст текущей ячейки шапки
        /// </summary>
        private string currentTopHeader = string.Empty;
        /// <summary>
        /// Начальный индекс строки с которого записываются данные
        /// </summary>
        private int processingStartIndex = 0;
        /// <summary>
        /// Формат загрузки
        /// </summary>
        private ExcelLoadingFormat loadingFormat
            {
            get { return Invoice.ExcelLoadingFormat; }
            }

        public DocumentUnloaderBase(Invoice invoice)
            {
            this.Invoice = invoice;
            if (invoice == null)
                {
                throw new NotImplementedException(ProcessingConsts.LOAD_FORMAT_NOT_FOUND_MESSAGE);
                }
            }

        /// <summary>
        /// Выполняет сохранение таблицы в файл
        /// </summary>
        /// <param name="fileName">Путь к сохраняемому файлу</param>
        /// <param name="table">Таблица</param>
        /// <param name="startIndex">Индекс строки с которого начинают записыватся данные</param>
        public void SaveTable(string fileName, DataTable table, int startIndex)
            {
            Unloader.OnInitializeRowData -= unloader_OnInitializeRowData;
            Unloader.OnGetTableHeaderColor -= unloader_OnGetHeaderColor;
            Unloader.OnGetCellColor -= unloader_OnGetCellColor;
            Unloader.OnInitializeRowData += unloader_OnInitializeRowData;
            Unloader.OnGetTableHeaderColor += unloader_OnGetHeaderColor;
            Unloader.OnGetCellColor += unloader_OnGetCellColor;
            processingStartIndex = startIndex;
            currentTopHeader = string.Empty;
            mapper = this.createMapper();
            if (table == null || table.Rows.Count == 0 || string.IsNullOrEmpty(fileName))
                {
                return;
                }
            Unloader.Save(table, fileName, mapper, startIndex);
            System.Diagnostics.Process.Start(fileName);
            }

        /// <summary>
        /// Возвращает информацию о ячейке шапки
        /// </summary>
        InitializedCellData unloader_OnInitializeRowData(string propertyName, int rowIndex)
            {
            string headerText = getColumnName(propertyName);
            int colSpan = 1;
            int rowSpan = 1;
            bool isBold = false;
            if (rowIndex == 0 && processingStartIndex == 2)
                {
                if (string.IsNullOrEmpty(currentTopHeader) && mapper[propertyName].ExpressionType.Equals(ExcelMapper.IndexKey) && mapper[propertyName].ExpressionBody.Equals("1"))
                    {
                    headerText = "Красным цветом выделенны ячейки с ошибками, серым - строки с новыми позициями.";
                    colSpan = mapper.Values.Count;
                    isBold = true;
                    currentTopHeader = headerText;
                    }
                else
                    {
                    return null;
                    }
                }
            return new InitializedCellData(headerText, rowSpan, colSpan, isBold);
            }

        string unloader_OnGetCellColor(DataRow row, string columnName)
            {
            return OnGetUnloadCellColor(row, columnName, mapper);
            }

        /// <summary>
        /// Возвращает цвет выгружаемой ячейки
        /// </summary>
        protected virtual string OnGetUnloadCellColor(DataRow row, string columnName, ExcelMapper mapper)
            {
            return null;
            }

        string unloader_OnGetHeaderColor(string columnName, int rowIdnex)
            {
            return ProcessingConsts.EXCEL_UNLOADING_HEADER_COLOR;
            }

        /// <summary>
        /// Возвращает что то типа "Caption", выгружаемой колонки DataTable
        /// </summary>
        /// <param name="propertyName">имя колонки в DataTable</param>
        /// <returns>Имя колонки отображаемое в шапке таблицы</returns>
        private static string getColumnName(string propertyName)
            {
            if (propertyName.Equals(ProcessingConsts.GRAF31_COLUMN_EN_NAME))
                {
                return ProcessingConsts.GRAF31_COLUMN_UK_NAME;
                }
            if (Invoice.InvoiceColumnNames.ContainsKey(propertyName))
                {
                return Invoice.InvoiceColumnNames[propertyName];
                }
            return propertyName;
            }

        /// <summary>
        /// Формирует информацию о соответствии колонок в таблице
        /// </summary>
        private ExcelMapper createMapper()
            {
            ExcelMapper mapper = new ExcelMapper();
            foreach (DataRow row in loadingFormat.ColumnsMappings.Rows)
                {
                int columnIndex = 0;
                int columnNameIndex = row.TryGetColumnValue<int>(ProcessingConsts.EXCEL_LOAD_FORMAT_TARGET_COLUMN_COLUMN_NAME, -1);
                string unloadIndex = row.TryGetColumnValue<string>(UnloadIndexColumnName, "");
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

        /// <summary>
        /// Возвращает начальный индекс строки в выгружаемом файле, с которого начинают записыватся данные. Строки перед начальным - отведены под шапку таблицы.
        /// </summary>
        protected abstract string UnloadIndexColumnName
            {
            get;
            }
        }
    }
