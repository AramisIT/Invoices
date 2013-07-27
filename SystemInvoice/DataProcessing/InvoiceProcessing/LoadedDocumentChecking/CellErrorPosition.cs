using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Хранит в себе информацию о расположении ошибки
    /// </summary>
    public class CellErrorPosition
        {
        /// <summary>
        /// Индекс строки в DataTable в которой находится ошибка
        /// </summary>
        public int RowIndex { get; private set; }
        /// <summary>
        /// Имя колонки в которой находится ошибка
        /// </summary>
        public string ColumnName { get; private set; }
        public CellErrorPosition( int rowIndex, string columnName )
            {
            this.RowIndex = rowIndex;
            this.ColumnName = columnName;
            }

        public override int GetHashCode()
            {
            return RowIndex.GetHashCode() ^ ColumnName.GetHashCode();
            }

        public override bool Equals( object obj )
            {
            CellErrorPosition other = obj as CellErrorPosition;
            if (other == null)
                {
                return false;
                }
            return RowIndex.Equals( other.RowIndex ) && ColumnName.Equals( other.ColumnName );
            }
        }
    }
