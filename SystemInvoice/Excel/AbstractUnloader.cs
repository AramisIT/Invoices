using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;
using System.IO;
using System.Threading.Tasks;
using SystemInvoice.Excel.DataFormatting;
using SystemInvoice.Excel.DataFormatting.Formatters;


namespace SystemInvoice.Excel
    {
    /// <summary>
    /// Делегат используемый для заполнения начальными данными шапки таблицы
    /// </summary>
    /// <param name="propertyName">Имя колонки/свойство объекта выгружаемого объекта, которому соответствует определенная колонка в шапке таблицы</param>
    /// <param name="rowIndex">Индекс строки в Excel - документе</param>
    /// <returns>Значение в ячейке документа</returns>
    public delegate InitializedCellData InitializeRowData( string propertyName, int rowIndex );
    /// <summary>
    /// Выполняет выгрузку данных в Excel - файл на основании карты привязки, для каждого поля/колонки объекта который должен быть выгружен должна присутствувать запись имя-выражение 
    /// для ExcelMapper, где имя - имя колонки/колонка в таблице, выражение - индекс колонки в Excel - таблице
    /// </summary>
    public abstract class AbstractUnloader
        {
        protected abstract int GetRowsCount();
        protected abstract object OnPropertyGet( string propertyName );
        protected abstract void OnRowProcessBegin( int rowIndex );
        protected abstract bool IsPropertyExists( string propertyName );
        protected abstract ExcelStyle GetCurrentObjectStyle( string propertyName );
        protected abstract string GetHeaderCellColor( string propertyName, int rowIndex );
        public event InitializeRowData OnInitializeRowData;
        // public event GetColorDelegate OnGetHeaderColor;

        protected ExcelStylesStore stylesStore = null;

        protected void Unload( string fileName, ExcelMapper mapper, int startIndex )
            {
            ExcelXlsWorkbook book = new ExcelXlsWorkbook();
            stylesStore = new ExcelStylesStore( book );
            Worksheet sheet = book[0];
            int rowsCount = GetRowsCount();
            Dictionary<string, int> itemsDict = createMapperIndexItems( mapper );
            int maxColumnIndex = itemsDict.Values.Max();
            for (int i = 0; i < maxColumnIndex; i++)
                {
                sheet.Columns( i ).Width = 100;
                }
            initializeExcelHeader( startIndex, sheet, itemsDict );
            for (int i = 0; i < rowsCount; i++)
                {
                OnRowProcessBegin( i );
                Row newRow = sheet[startIndex + i];
                foreach (string name in itemsDict.Keys)
                    {
                    int itemIndex = itemsDict[name];
                    setCurrentCellValue( newRow, name, itemIndex );
                    setCurrentCellStyle( newRow, name, itemIndex );
                    }
                }
            book.Export( fileName );
            }

        private void setCurrentCellStyle( Row newRow, string name, int itemIndex )
            {
            ExcelStyle currentObjectStyle = GetCurrentObjectStyle( name );
            if (currentObjectStyle != null)
                {
                newRow[itemIndex - 1].Style = currentObjectStyle;
                }
            //   newRow[itemIndex - 1].Style.Alignment.WrapText = true;
            }

        private void setCurrentCellValue( Row newRow, string name, int itemIndex )
            {
            object currentValue = OnPropertyGet( name );
            if (currentValue == DBNull.Value || currentValue == null)
                {
                currentValue = "";
                }
            newRow[itemIndex - 1].Value = currentValue;
            }

        /// <summary>
        /// Создает "шапку" в выгружаемом файле - заполняет данными область таблицы которая находится перед первой строкой в которую выгружаются данные
        /// </summary>
        /// <param name="startIndex">Начальный индекс с которого начинается область выгружаемых данных, шапка занимает область до этого индекса</param>
        /// <param name="sheet">Excel - лист</param>
        /// <param name="itemsDict">Набор колонок</param>
        private void initializeExcelHeader( int startIndex, Worksheet sheet, Dictionary<string, int> itemsDict )
            {
            if (OnInitializeRowData != null && startIndex > 0)
                {
                for (int i = 0; i < startIndex; i++)
                    {
                    Row newRow = sheet[i];
                    foreach (string name in itemsDict.Keys)
                        {
                        InitializedCellData initializeData = OnInitializeRowData( name, i );
                        string headerCellColor = GetHeaderCellColor( name, i );
                        this.initializeHeaderCell( newRow[itemsDict[name] - 1], initializeData, headerCellColor );
                        if (initializeData != null && initializeData.ColSpan == itemsDict.Keys.Count)
                            {
                            break;
                            }
                        }
                    }
                }
            }
        /// <summary>
        /// Заполняет выгружаемую ячейку шапки
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <param name="initializeData">данные и внешний вид ячейки</param>
        /// <param name="headerColor">цвет ячейки</param>
        private void initializeHeaderCell( Cell cell, InitializedCellData initializeData, string headerColor )
            {
            cell.Style = stylesStore.GetStyle( headerColor );
            if (initializeData != null)
                {
                cell.Value = initializeData.Text;
                cell.ColumnSpan = initializeData.ColSpan;
                cell.RowSpan = initializeData.RowSpan;
                cell.Style.Font.Bold = initializeData.IsBold;
                if (cell.ColumnSpan > 1 || cell.RowSpan > 1)
                    {
                    Cell toCell = cell.ParentRow.ParentSheet[cell.RowSpan + cell.ParentRow.RowIndex - 1][cell.CELL.ColumnIndex + cell.ColumnSpan + -1];
                    new Range( cell, toCell ).Merge();
                    }
                }
            }

        /// <summary>
        /// Создает словарь для колонок выгружаемой таблицы где значение - номер выгружаемой колонки в Excel
        /// </summary>
        private Dictionary<string, int> createMapperIndexItems( ExcelMapper mapper )
            {
            Dictionary<string, int> mapperItemsDict = new Dictionary<string, int>();
            int index = 0;
            foreach (string name in mapper.Keys)
                {
                Expression expression = mapper[name];
                if (expression.ExpressionType.Equals( ExcelMapper.IndexKey ) && IsPropertyExists( name ))
                    {
                    if (int.TryParse( expression.ExpressionBody, out index ))
                        {
                        mapperItemsDict.Add( name, index );
                        }
                    }
                }
            return mapperItemsDict;
            }
        }
    }
