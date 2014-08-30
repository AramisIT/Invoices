using System;
using System.Collections.Generic;
using System.Data;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.RowsGrouping
    {
    /// <summary>
    /// Группирует строки в табличной части инвойса, и суммирует значения колличества, суммы, общего веса нетто/брутто для сгрупированных строк
    /// </summary>
    public class GroupingHandler
        {
        GroupedRowsDataChecker groupingRowChecker = new GroupedRowsDataChecker();
        private Func<bool> needGroupping;

        public GroupingHandler(Func<bool> needGroupping)
            {
            this.needGroupping = needGroupping;
            }
        /// <summary>
        /// Выполняет группировку строк в таблице
        /// </summary>
        public void MakeGrouping(DataTable tableToProcess)
            {
            if (!needGroupping()) return;

            //Формируем набор обрабатываемых строк
            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in tableToProcess.Rows)
                {
                rows.Add(row);
                }
            //сортируем набор в таком порядке, что бы все строки которые попадали в группировку шли одна за одной
            GoodsRowsComparer rowComparer = new GoodsRowsComparer();
            rows.Sort(rowComparer);
            //последовательно обрабатываем отсортированный массив формируя группы строк и выполняя группировку
            DataRow accRow = null;
            List<DataRow> newRows = new List<DataRow>();
            foreach (DataRow row in rows)
                {
                if (accRow == null)
                    {
                    accRow = tableToProcess.NewRow();
                    foreach (DataColumn column in tableToProcess.Columns)
                        {
                        accRow[column] = row[column];
                        }
                    continue;
                    }
                if (rowComparer.Equals(accRow, row))//проверяем входит ли строка в текущую группу
                    {
                    this.addToAggregateRow(accRow, row);
                    }
                else
                    {//добавляем сгруппированную строку в список и начинаем новую группу
                    newRows.Add(accRow);
                    accRow = tableToProcess.NewRow();
                    foreach (DataColumn column in tableToProcess.Columns)
                        {
                        accRow[column] = row[column];
                        }
                    }
                }
            if (accRow != null)
                {
                newRows.Add(accRow);
                }
            tableToProcess.Rows.Clear();
            //сортируем сгруппированные строки с тем что бы можно было потом сформировать шапки графы 31 для групп полученных строк (строки соответствующие одной шапке должны идти подряд) 
            newRows.Sort(new ForHeaderGraf31ConstructComparer());
            long lineNumber = 1;
            foreach (DataRow row in newRows)//Записываем номера строк для итогового набора
                {
                row[ProcessingConsts.ColumnNames.LINE_NUMBER_COLUMN_NAME] = lineNumber++;
                groupingRowChecker.CheckRow(row);
                tableToProcess.Rows.Add(row);
                }
            }

        private void addToAggregateRow(DataRow accRow, DataRow row)
            {
            double netWeightAggregate = 0;
            double netWeightAdditional = 0;
            double grossWeightAggregate = 0;
            double grossWeightAdditonal = 0;
            double itemsCountAggregate = 0;
            double itemsCountAdditonal = 0;
            double totalSummAggregate = 0;
            double totalSummAdditional = 0;
            if (double.TryParse(row.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.GROSS_WEIGHT_COLUMN_NAME, "0"), out grossWeightAdditonal))
                {
                if (!double.TryParse(accRow.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.GROSS_WEIGHT_COLUMN_NAME, "0"), out grossWeightAggregate))
                    {
                    grossWeightAggregate = 0;
                    }
                accRow[ProcessingConsts.ColumnNames.GROSS_WEIGHT_COLUMN_NAME] = (grossWeightAdditonal + grossWeightAggregate).ToString();
                }
            if (double.TryParse(row.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME, "0"), out itemsCountAdditonal))
                {
                if (!double.TryParse(accRow.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME, "0"), out itemsCountAggregate))
                    {
                    itemsCountAggregate = 0;
                    }
                accRow[ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME] = (itemsCountAggregate + itemsCountAdditonal).ToString();
                }
            if (double.TryParse(row.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME, "0"), out netWeightAdditional))
                {
                if (!double.TryParse(accRow.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME, "0"), out netWeightAggregate))
                    {
                    netWeightAggregate = 0;
                    }
                accRow[ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME] = (netWeightAdditional + netWeightAggregate).ToString();
                }
            if (double.TryParse(row.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.SUM_COLUMN_NAME, "0"), out totalSummAdditional))
                {
                if (!double.TryParse(accRow.TryGetColumnValue<string>(ProcessingConsts.ColumnNames.SUM_COLUMN_NAME, "0"), out totalSummAggregate))
                    {
                    totalSummAggregate = 0;
                    }
                accRow[ProcessingConsts.ColumnNames.SUM_COLUMN_NAME] = (totalSummAdditional + totalSummAggregate).ToString();
                }
            }
        }
    }
