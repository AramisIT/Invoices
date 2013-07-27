using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors.SummDistribution
    {
    /// <summary>
    /// Сравнивает строки по цене и артикулу
    /// </summary>
    public class ArticleAndPriceRowComparer : IComparer<DataRow>
        {
        private string articleColumnName = null;
        private string priceColumnName = null;

        public ArticleAndPriceRowComparer(string articleColumnName, string priceColumnName)
            {
            this.articleColumnName = articleColumnName;
            this.priceColumnName = priceColumnName;
            }

        public int Compare(DataRow x, DataRow y)
            {
            string xArticle = x.TrySafeGetColumnValue(articleColumnName, string.Empty);
            string yArticle = y.TrySafeGetColumnValue(articleColumnName, string.Empty);
            string xPrice = x.TrySafeGetColumnValue(priceColumnName, string.Empty);
            string yPrice = y.TrySafeGetColumnValue(priceColumnName, string.Empty);
            int articleCompare = string.Compare(xArticle, yArticle);// xArticle.Compare(yArticle);
            int priceCompare = string.Compare(xPrice, yPrice);// xPrice.Compare(yPrice);
            return articleCompare != 0 ? articleCompare : priceCompare;
            }
        }
    }
