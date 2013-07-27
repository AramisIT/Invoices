using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Хранит в себе ошибки стоки.
    /// </summary>
    public class RowColumnsErrors : Dictionary<string, CellErrorsCollection>
        {
        /// <summary>
        /// Проверяет валидность ячейки в строке
        /// </summary>
        /// <param name="columnName">колонка в которой находится ячейка</param>
        public bool IsCellValid( string columnName )
            {
            if (ContainsKey( columnName ))
                {
                return this[columnName].Count == 0;
                }
            return true;
            }

        /// <summary>
        /// Возвращает - была ли найдена номенклатурная позиция для данной строки
        /// </summary>
        public bool IsRowLoaded
            {
            get
                {
                if (this.ContainsKey( RowCheckError.ROW_IS_INVALID_FAKE_COLUMN_NAME ))
                    {
                    return false;
                    }
                return true;
                }
            }
        }
    }
