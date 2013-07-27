using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.InvoiceProcessing.Helpers;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Syncronizers
    {
    /// <summary>
    /// Синхронизирует колонки связанные с расчетом суммы
    /// </summary>
    public class SummSyncronizer : ISyncronizer
        {
        //public event Action<DataRow> OnRowChanged = null;

        //private void raiseRowChanged(DataRow row)
        //    {
        //    if (OnRowChanged != null && row != null)
        //        {
        //        OnRowChanged(row);
        //        }
        //    }

        #region реализация ISyncronizer
        /// <summary>
        /// Синхронизирует ячейки в колонке сумма, со значениями в колонке колличество, цена, процент наценки,
        ///  а также значения для колонок наценка, цена с наценкой со значениями цена, процент наценки
        /// </summary>
        public void Syncronize(DataRow row, string columnName, RequestForSyncronizationSource source)
            {
            syncronizeRow(row);
            if (source == RequestForSyncronizationSource.DataCellChangedSource)
                {
                syncronizeAll(row);
                }
            }

        public bool NeedSyncronization(string columnName)
            {
            return columnName.Equals(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME)
                || columnName.Equals(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME)
                || columnName.Equals(ProcessingConsts.ColumnNames.MARGIN_PRECENTAGE_COLUMN_NAME);
            } 
        #endregion

        /// <summary>
        /// Обновляет значения во всех строках с одним и тем же артикулом как и в строке инициировавшей синхронизацию
        /// </summary>
        /// <param name="row">Строка инициировавшая синхронизацию</param>
        private void syncronizeAll(DataRow row)
            {
            string priceToUpdate = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME, string.Empty);
            string article = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME, string.Empty);
            string tradeMark = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME, string.Empty);
            foreach (DataRow rowToTryUpdate in row.Table.Rows)
                {
                if (haveToBeUpdated(rowToTryUpdate, article, tradeMark, priceToUpdate))
                    {
                    rowToTryUpdate[ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME] = priceToUpdate;
                    syncronizeRow(rowToTryUpdate);
                    }
                }
            }

        private bool haveToBeUpdated(DataRow rowToTryUpdate, string article, string tradeMark, string priceToUpdate)
            {
            if (rowToTryUpdate == null || string.IsNullOrEmpty(article) || string.IsNullOrEmpty(tradeMark) ||
                string.IsNullOrEmpty(priceToUpdate))
                {
                return false;
                }
            string priceInUpdatedRow = rowToTryUpdate.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME, string.Empty);
            string articleInUpdatedRow = rowToTryUpdate.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME, string.Empty);
            string tradeMarkInUpdatedRow = rowToTryUpdate.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME, string.Empty);
            if (article.Equals(articleInUpdatedRow) && tradeMark.Equals(tradeMarkInUpdatedRow) &&
                !priceToUpdate.Equals(priceInUpdatedRow))
                {
                return true;
                }
            return false;
            }

        /// <summary>
        /// Синхронизирует между собой колонки в определенной строке
        /// </summary>
        private void syncronizeRow(DataRow row)
            {
            string countColumnName = Documents.InvoiceColumnNames.Count.ToString();
            string summColumnName = Documents.InvoiceColumnNames.Sum.ToString();
            string marginPercentageColumnName = Documents.InvoiceColumnNames.MarginPrecentage.ToString();
            string priceWithMarginColumnName = Documents.InvoiceColumnNames.PriceWithMargin.ToString();
            string marginColumnName = Documents.InvoiceColumnNames.Margin.ToString();
            //    string priceWithMarginColumnName
            double inDocumentPriceValue, margin, marginPercentage;
            int itemsCount;
            string valueToConvert = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME,
                                                                      string.Empty);
            string itemsCountStr = row.TryGetColumnValue<string>(countColumnName, "");
            string itemsMarginStr = row.TrySafeGetColumnValue<string>(marginColumnName, "");
            string marginPercentageStr = row.TrySafeGetColumnValue<string>(marginPercentageColumnName, "");
            string priceWithMarginStr = row.TrySafeGetColumnValue<string>(priceWithMarginColumnName, "");
            if (!double.TryParse(valueToConvert, out inDocumentPriceValue))
                {
                return;
                }
            if (!int.TryParse(itemsCountStr, out itemsCount))
                {
                return;
                }
            if (!double.TryParse(itemsMarginStr, out margin))
                {
                margin = 0;
                }
            if (!double.TryParse(marginPercentageStr, out marginPercentage))
                {
                marginPercentage = 0;
                }
            double marginCalculated = Math.Round(marginPercentage * inDocumentPriceValue, 2);
            double priceWithMargin = Math.Round(inDocumentPriceValue * (1 + marginPercentage), 2);
            double totalSumm = InvoiceProcessingHelper.GetTotalSumm(priceWithMargin, itemsCount, margin);
            string currentSummValue = row.TrySafeGetColumnValue<string>(summColumnName, "");
            string newSummValue = Math.Round(totalSumm, 2).ToString();
            string marginCalcStrValue = marginCalculated.ToString();
            string priceWithMarginStrValue = priceWithMargin.ToString();
            if (!currentSummValue.Equals(newSummValue))
                {
                row[summColumnName] = newSummValue;
                }
            if (!itemsMarginStr.Equals(marginCalcStrValue))
                {
                row[marginColumnName] = marginCalcStrValue;
                }
            if (!priceWithMarginStr.Equals(priceWithMarginStrValue))
                {
                row[priceWithMarginColumnName] = priceWithMarginStrValue;
                }
            }
        }
    }
