using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using AramisWpfComponents.Excel;
using System.IO;
using System.Threading.Tasks;
using SystemInvoice.Excel.DataFormatting;
using SystemInvoice.Excel.DataFormatting.Formatters;


namespace SystemInvoice.Excel
    {

    public delegate string GetCurrentColorHandler( DataRow row );

    public delegate string GetTableHeaderColorDelegate( string columnName, int headerRowIndex );

    public delegate string GetTableCellColorHandler( DataRow row, string columnName );
    /// <summary>
    /// Выгружает таблицу DataTable в Excel
    /// </summary>
    public class TableUnloader : AbstractUnloader
        {
        /// <summary>
        /// Вызывается для получения цвета выгружаемой ячейки
        /// </summary>
        public event GetTableCellColorHandler OnGetCellColor;
        /// <summary>
        /// Вызывается для получения цвета строки
        /// </summary>
        public event GetCurrentColorHandler OnGetRowColor;
        /// <summary>
        /// Вызывается для получения цвета ячейки шапки
        /// </summary>
        public event GetTableHeaderColorDelegate OnGetTableHeaderColor;
        /// <summary>
        /// Текущая таблица которая выгружается в Excel
        /// </summary>
        private DataTable currentTable = null;
        /// <summary>
        /// Текущая обрабатываемая строка
        /// </summary>
        private DataRow currentRow = null;
        /// <summary>
        /// Индекс текущей строки
        /// </summary>
        protected int currentRowIndex = -1;

        protected override int GetRowsCount()
            {
            return currentTable.Rows.Count;
            }

        protected override object OnPropertyGet( string propertyName )
            {
            return currentRow[propertyName];
            }

        protected override bool IsPropertyExists( string propertyName )
            {
            if (currentTable == null)
                {
                return false;
                }
            return currentTable.Columns.Contains( propertyName );
            }

        protected override void OnRowProcessBegin( int rowIndex )
            {
            currentRowIndex = rowIndex;
            currentRow = currentTable.Rows[rowIndex];
            }
        /// <summary>
        /// Сохраняет таблицу в в Excel - файл
        /// </summary>
        /// <param name="dataTable">Таблица</param>
        /// <param name="fileName">Путь к выгружаемому файлу</param>
        /// <param name="excelMapper">Опичание выгружаемых в Excel- колонок</param>
        public void Save( DataTable dataTable, string fileName, ExcelMapper excelMapper, int startRowIndex )
            {
            currentTable = dataTable;
            Unload( fileName, excelMapper, startRowIndex );
            }

        protected override ExcelStyle GetCurrentObjectStyle( string propertyName )
            {
            return getCurrentCellStyle( propertyName ) ?? getCurrentRowStyle();
            }

        private ExcelStyle getCurrentCellStyle( string propertyName )
            {
            if (OnGetCellColor != null)
                {
                return getCellStyledColor( propertyName );
                }
            return null;
            }

        private ExcelStyle getCellStyledColor( string propertyName )
            {
            string color = OnGetCellColor( currentRow, propertyName );
            if (!string.IsNullOrEmpty( color ))
                {
                return stylesStore.GetStyle( color );
                }
            return null;
            }

        private ExcelStyle getCurrentRowStyle()
            {
            ExcelStyle rowStyle = null;
            if (OnGetRowColor != null)
                {
                string rowColor = OnGetRowColor( currentRow );
                rowStyle = stylesStore.GetStyle( rowColor );
                }
            return rowStyle;
            }

        protected override string GetHeaderCellColor( string propertyName, int rowIndex )
            {
            if (OnGetTableHeaderColor != null)
                {
                return OnGetTableHeaderColor( propertyName, rowIndex );
                }
            return string.Empty;
            }
        }
    }
