using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors.SummDistribution
    {
    /// <summary>
    /// Содержит информацию о распределении наценок по номенклатуре
    /// </summary>
    public class WithMarginArticlePriceInfo
        {
        class RowInfo
            {
            public int Count;
            public double Margin;
            public int IntPrice;
            }
        /// <summary>
        /// Получает данные о различии цены с учетом наценки
        /// </summary>
        public int MinGreaterThanDefaultPrice { get; private set; }
        /// <summary>
        /// Распределегие наценок
        /// </summary>
        private List<RowInfo> margins = new List<RowInfo>();

        /// <summary>
        /// Добавляет информацию о разнице цены с учето наценки и без
        /// </summary>
        public void AppendMarginInfo(ArticleWithSomePriceCollection rows, bool isPlus)
            {
            int sign = isPlus ? 1 : -1;
            int minPriceAppend = 0;
            margins.Clear();
            foreach (DataRow row in rows)
                {
                int count = this.getItemsCount(row);
                int price = this.getItemsPrice(row);
                double margin = this.getItemsMargin(row);
                margins.Add(new RowInfo() { Count = count, IntPrice = price, Margin = margin });

                int oldPrice = (int)(Math.Round(price * 1.0 * (1 + margin), 0)) * count;
                int newPrice = (int)(Math.Round((price + 1 * sign) * (1 + margin), 0)) * count;
                minPriceAppend += oldPrice - newPrice;
                }
            MinGreaterThanDefaultPrice = (int)Math.Abs(minPriceAppend);
            }

        public int GetUpdateRate(int unitPriceUpdateRange, bool isPlus)
            {
            int sign = isPlus ? 1 : -1;
            int calculatedPrice = 0, oldTotalPrice = 0;
            foreach (RowInfo rowInfo in margins)
                {
                int oldPrice = (int)(Math.Round(rowInfo.IntPrice * 1.0 * (1 + rowInfo.Margin), 0)) * rowInfo.Count;
                int newPrice = (int)(Math.Round((rowInfo.IntPrice + unitPriceUpdateRange * sign) * (1 + rowInfo.Margin), 0)) * rowInfo.Count;
                int otherDiff = newPrice - oldPrice;
                calculatedPrice += newPrice;
                oldTotalPrice += oldPrice;
                }
            return Math.Abs(calculatedPrice - oldTotalPrice);
            }


        public void UpdatePriceInfo(int updateDiff, bool p)
            {
            int sign = p ? 1 : -1;
            foreach (RowInfo rowInfo in margins)
                {
                int newPrice = rowInfo.IntPrice + updateDiff * sign;
                rowInfo.IntPrice = newPrice;
                }
            }

        private int getItemsCount(DataRow row)
            {
            string countStr = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME, string.Empty);
            int count = 0;
            int.TryParse(countStr, out count);
            return count;
            }

        private int getItemsPrice(DataRow row)
            {
            string priceStr = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME, string.Empty);
            double price = 0;
            double.TryParse(priceStr, out price);
            int intPrice = (int)(price * 100);
            return intPrice;
            }

        private double getItemsMargin(DataRow row)
            {
            string marginPercentageStr = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.MARGIN_PRECENTAGE_COLUMN_NAME, string.Empty);
            double marginPercentage = 0;
            double.TryParse(marginPercentageStr, out marginPercentage);
            return marginPercentage;
            }
        }

    }
