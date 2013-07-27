using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Хранит в себе информацию об ошибках в таблице
    /// </summary>
    public class RowErrors : Dictionary<int, RowColumnsErrors>
        {
        /// <summary>
        /// Проверяет, содержит ли строка ошибки. Ошибки с уведомлениями не учитываются
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        public bool IsRowValid( int rowIndex )
            {
            if (ContainsKey( rowIndex ))
                {
                RowColumnsErrors columnsErrors = this[rowIndex];
                foreach (var values in columnsErrors.Values)
                    {
                    if (values.NonNotificationErrorsCount > 0)
                        {
                        return false;
                        }
                    }
                return true;
                }
            return true;
            }

        /// <summary>
        /// Проверяет, содержит ли строка ошибки или ошибки с уведомлениями
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        public bool HaveIncorrectValues(int rowIndex)
            {
            if (ContainsKey(rowIndex))
                {
                return this[rowIndex].Count > 0;
                }
            return false;
            }

        /// <summary>
        /// Проверяет валидность ячейки
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <param name="columnName">Колонка</param>
        public bool IsCellValid( int rowIndex, string columnName )
            {
            if (!IsRowValid( rowIndex ))
                {
                return this[rowIndex].IsCellValid( columnName );
                }
            return true;
            }

        /// <summary>
        /// Проверяет была ли найдена номенклатурная позиция для строки
        /// </summary>
        /// <param name="rowIndex">индекс строки</param>
        public bool IsRowLoaded( int rowIndex )
            {
            if (this.ContainsKey( rowIndex ))
                {
                return this[rowIndex].IsRowLoaded;
                }
            return true;
            }

        /// <summary>
        /// Возвращает колличество незагруженных строк
        /// </summary>
        public int NonLoadedRowsCount
            {
            get
                {
                int rowsCount = 0;
                foreach (int rowIndex in this.Keys)
                    {
                    if (!IsRowLoaded( rowIndex ))
                        {
                        rowsCount++;
                        }
                    }
                return rowsCount;
                }
            }

        /// <summary>
        /// Возвращает первую ошибку в ячейке
        /// </summary>
        public CellError GetError( int rowIndex, string columnName )
            {
            if (!IsCellValid( rowIndex, columnName ))
                {
                return this[rowIndex][columnName][0];
                }
            return null;
            }

        /// <summary>
        /// Возвращает текст информационных ошибок
        /// </summary>
        public string GetNotification(int rowIndex, string columnName)
            {
            if (HaveDisplayError(rowIndex, columnName))
                {
                return this[rowIndex][columnName].CustomErrorsMessage;
                }
            return null;
            }

        /// <summary>
        /// Проверяет нужно ли для данной ячейки отображать ошибку
        /// </summary>
        public bool HaveDisplayError(int rowIndex, string columnName)
            {
            try
                {
                if (HaveIncorrectValues(rowIndex))
                    {
                    return !this[rowIndex].IsCellValid(columnName);
                    }
                return false;
                }
            catch
                {
                return false;
                }
            }
        }
    }
