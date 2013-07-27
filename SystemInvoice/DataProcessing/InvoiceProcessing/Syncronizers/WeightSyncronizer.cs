using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Syncronizers
    {
    /// <summary>
    /// Синхронизирует общий вес нетто с весом нетто единицы товара и колличеством
    /// </summary>
    public class WeightSyncronizer : ISyncronizer
        {
        #region реализация ISyncronizer

        public bool NeedSyncronization(string columnName)
            {
            return columnName.Equals(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME);
            }

        public void Syncronize(DataRow row, string columnName, RequestForSyncronizationSource source)
            {
            if (columnName.Equals(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME))//если изменился общий вес - меняем вес единицы товара, в противном случае наоборот
                {
                syncNetWeightColumns(row, false);
                }
            else
                {
                syncNetWeightColumns(row, true);
                }
            //if (source == RequestForSyncronizationSource.DataCellChangedSource)
            //    {
            //    foreach (DataRow rowToTryUpdate in row.Table.Rows)
            //        {
            //        if (columnName.Equals(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME))
            //            {
            //            syncNetWeightColumns(rowToTryUpdate, false);
            //            }
            //        else
            //            {
            //            syncNetWeightColumns(rowToTryUpdate, true);
            //            }
            //        }
            //    }
            } 
        #endregion
        /// <summary>
        /// Синзронизирует вес нетто с весом единицы товара в строке
        /// </summary>
        /// <param name="rowToCheck">Строка которую нужно синхронизировать</param>
        /// <param name="fromUnitToTotal">Обновлять значение общего веса на основании веса единицы товара или общий вес</param>
        private void syncNetWeightColumns(DataRow rowToCheck, bool fromUnitToTotal)
            {
            string countColumnName = Documents.InvoiceColumnNames.Count.ToString();
            string netWeightColumnName = SystemInvoice.Documents.InvoiceColumnNames.NetWeight.ToString();
            string unitNetWeightColumnName = SystemInvoice.Documents.InvoiceColumnNames.UnitWeight.ToString();
            string unitWeightVal = rowToCheck.TrySafeGetColumnValue<string>(unitNetWeightColumnName, "");
            string netWeightVal = rowToCheck.TrySafeGetColumnValue<string>(netWeightColumnName, "");
            string countVal = rowToCheck.TrySafeGetColumnValue<string>(countColumnName, "");

            double netWeight, unitNetWeight;
            int count;
            if (!double.TryParse(netWeightVal, out netWeight) ||
                !double.TryParse(unitWeightVal, out unitNetWeight) ||
                !int.TryParse(countVal, out count))
                {
                return;
                }

            if (fromUnitToTotal)
                {
                string oldValue = rowToCheck[netWeightColumnName].ToString();
                string newValue = Math.Round(unitNetWeight * count, 3).ToString();
                if (!oldValue.Equals(newValue))
                    {
                    rowToCheck[netWeightColumnName] = newValue;
                    }
                }
            else
                {
                if (count == 0)
                    {
                    return;
                    }
                string oldValue = rowToCheck[unitNetWeightColumnName].ToString();
                string newValue = Math.Round(netWeight / count, 3).ToString();
                if (!oldValue.Equals(newValue))
                    {
                    rowToCheck[unitNetWeightColumnName] = newValue;
                    }
                }
            }
        }
    }
