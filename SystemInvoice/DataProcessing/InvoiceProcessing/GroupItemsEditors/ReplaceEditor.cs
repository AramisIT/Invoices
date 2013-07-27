using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Documents;
using System.Data;
using SystemInvoice.DataProcessing.Cache;


namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors
    {
    /// <summary>
    /// Заменяет значение в группе выбранных строк для определенной колонки и определенного заменяемого значения
    /// </summary>
    public class ReplaceEditor
        {
        private IEditableRowsSource editableRowsSource = null;

        public ReplaceEditor( IEditableRowsSource editableRowsSource )
            {
            this.editableRowsSource = editableRowsSource;
            }

        /// <summary>
        /// Устанавливает значения в определенную колонку таблицы
        /// </summary>
        /// <param name="valueToReplace"></param>
        /// <param name="valueByWhichReplace">Устанавливаемое значение</param>
        /// <param name="columnToReplace">Колонка в которой нужно установить значение</param>
        public void Replace(string valueByWhichReplace, string columnToReplace )
            {
            List<DataRow> rowsToUpdate = editableRowsSource.DisplayingRows.ToList();
            foreach (DataRow row in rowsToUpdate)
                {
                row[columnToReplace] = valueByWhichReplace;
                }
            }
        }
    }
