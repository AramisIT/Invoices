using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.RowsGrouping
    {
    /// <summary>
    /// Выполняет проверку строк в инвойсе на предмет соответствия числовым данным, если какое - то значение не соответствует числу - устанавливает ноль для этого значения в ячейке
    /// </summary>
    public class GroupedRowsDataChecker
        {
        /// <summary>
        /// Проверяет числовые данные в строке, для неверных данных устанавливает 0
        /// </summary>
        public void CheckRow(DataRow rowToCheck)
            {
            checkCellForInt(rowToCheck, ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME);
            checkCellForDouble(rowToCheck, ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME);
            checkCellForDouble(rowToCheck, ProcessingConsts.ColumnNames.SUM_COLUMN_NAME);
            checkCellForDouble(rowToCheck, ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME);
            checkCellForDouble(rowToCheck, ProcessingConsts.ColumnNames.GROSS_WEIGHT_COLUMN_NAME);
            checkCellForDouble(rowToCheck, ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME);
            }

        private static void checkCellForInt(DataRow rowToCheck, string columnName)
            {
            string intStr = rowToCheck.TrySafeGetColumnValue(columnName, string.Empty);
            if (!string.IsNullOrEmpty(intStr))
                {
                int intVal = 0;
                if (int.TryParse(intStr, out intVal))
                    {
                    string reconsturcted = intVal.ToString();
                    if (!intStr.Equals(reconsturcted))
                        {
                        rowToCheck[columnName] = reconsturcted;
                        }
                    }
                }
            }

        private static void checkCellForDouble(DataRow rowToCheck, string columnName)
            {
            string doubleStr = rowToCheck.TrySafeGetColumnValue(columnName, string.Empty);
            if (!string.IsNullOrEmpty(doubleStr))
                {
                double doubleVal = 0;
                if (double.TryParse(doubleStr, out doubleVal))
                    {
                    string reconsturcted = doubleVal.ToString();
                    if (!doubleStr.Equals(reconsturcted))
                        {
                        rowToCheck[columnName] = reconsturcted;
                        }
                    }
                }
            }
        }
    }
