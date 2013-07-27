using System;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Helpers
    {
    /// <summary>
    /// Хэлпер с алгоритмами используемыми разными компонентами
    /// </summary>
    public class InvoiceProcessingHelper
        {
        /// <summary>
        /// Рассчитывает сумму на основании цены, колличества, наценки
        /// </summary>
        public static double GetTotalSumm( double price, int count, double margin )
            {
            if (count == 0)
                {
                return 0;
                }
            return Math.Round( count * price, 2 );// (price + margin), 2 );
            }
        }
    }
