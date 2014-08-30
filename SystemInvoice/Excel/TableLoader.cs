using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace SystemInvoice.Excel
    {
    /// <summary>
    /// Загружает Ехеl в существующий DataTable, или выгружает в новый
    /// </summary>
    public class TableLoader : AbstractLoader
        {
        /// <summary>
        /// Текущая таблица в которую добавляются новые строки
        /// </summary>
        private DataTable currentTable = null;
        /// <summary>
        /// Текущая строка, которой присваиваются значения
        /// </summary>
        private DataRow currentRow = null;
        /// <summary>
        /// Заменяет null - значения пустыми строками или нулями
        /// </summary>
        protected bool replaceNullValues = false;

        public TableLoader(bool replaceNullValues)
            {
            this.replaceNullValues = replaceNullValues;
            }

        public TableLoader()
            : this(false)
            {
            }

        /// <summary>
        /// Создает новый экземпляр таблицы заполненной текстовыми данными считанными из Excel - файла
        /// </summary>
        /// <param name="mapper">Набор колонок с соответствующими преобразователями</param>
        /// <param name="fileName">Путь к обрабатываемому файлу</param>
        /// <param name="startIndex">Начальный индекс обрабатываемой строки</param>
        /// <param name="successed">Была ли операция выполнена без ошибок</param>
        /// <returns>Результирующая таблица</returns>
        public DataTable Transform(ExcelMapper mapper, string fileName, int startIndex, out bool successed)
            {
            DataTable tableToFill = createMappingTable(mapper);
            successed = TryFill(tableToFill, mapper, fileName, startIndex);
            return tableToFill;
            }

        /// <summary>
        /// Создает новый экземпляр таблицы заполненной текстовыми данными считанными из Excel - файла
        /// </summary>
        /// <param name="mapper">Набор колонок с соответствующими преобразователями</param>
        /// <param name="fileName">Путь к обрабатываемому файлу</param>
        /// <param name="startIndex">Начальный индекс обрабатываемой строки</param>
        /// <returns>Результирующая таблица</returns>
        public DataTable Transform(ExcelMapper mapper, string fileName, int startIndex)
            {
            bool successed = false;
            return Transform(mapper, fileName, startIndex, out successed);
            }

        /// <summary>
        /// Заполняет существующую таблицу данными считаными из Excel - файла
        /// </summary>
        /// <param name="tableToFill">Таблица</param>
        /// <param name="mapper">Набор колонок с соответствующими преобразователями</param>
        /// <param name="fileName">Путь к обрабатываемому файлу</param>
        /// <param name="startIndex">Начальный индекс обрабатываемой строки</param>
        public bool TryFill(DataTable tableToFill, ExcelMapper mapper, string fileName, int startIndex)
            {
            if (tableToFill == null || mapper == null || string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                {
                return false;
                }
            currentTable = tableToFill;
            return TryLoad(fileName, mapper, 0, startIndex, -1);
            }

        protected override Type getFormatterType(string propertyName)
            {
            if (!isPropertyExists(propertyName))
                {
                return null;
                }
            return currentTable.Columns[propertyName].DataType;
            }

        protected override bool isPropertyExists(string propertyName)
            {
            return currentTable == null ? false : currentTable.Columns.Contains(propertyName);
            }

        protected override void OnRowProcessingBegin()
            {
            currentRow = currentTable.NewRow();
            }

        protected override void OnRowProcessingComplete()
            {
            if (replaceNullValues)
                {
                object[] items = currentRow.ItemArray;
                for (int i = 0; i < currentTable.Columns.Count; i++)
                    {
                    DataColumn column = currentTable.Columns[i];
                    if (items[i] == null || items[i] == DBNull.Value)
                        {
                        if (column.DataType == typeof(string))
                            {
                            items[i] = " ";
                            }
                        if (column.DataType == typeof(long))
                            {
                            items[i] = 0;
                            }
                        }
                    }
                currentTable.Rows.Add(items);
                }
            else
                {
                currentTable.Rows.Add(currentRow);
                }
            }

        protected override void OnPropertySet(string propertyName, object value)
            {
            if (value != null)
                {
                currentRow[propertyName] = value;
                }
            else
                {
                currentRow[propertyName] = DBNull.Value;
                }
            }

        /// <summary>
        /// Создает новый экземпляр таблицы на основании ключей набора выражений
        /// </summary>
        private DataTable createMappingTable(ExcelMapper mapper)
            {
            DataTable table = new DataTable();
            foreach (string key in mapper.Keys)
                {
                table.Columns.Add(key, typeof(string));
                }
            return table;
            }
        }
    }
