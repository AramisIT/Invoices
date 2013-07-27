using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors.SummDistribution
    {
    /// <summary>
    /// Содержит информацию о группе строк с одинаковым артикулом и ценой. Так же хранит информацию о разницах в цене с наценкой и без для групп товаров
    /// </summary>
    public class ArticlePriceGroupInfo
        {
        public string Article { get; private set; }
        public string Price { get; private set; }
        public int Count { get; private set; }

        public ArticlePriceGroupInfo(string article, string price, int count)
            {
            this.Article = article;
            this.Price = price;
            this.Count = count;
            withMarginArticlePriceInfo = new WithMarginArticlePriceInfo();
            }

        public override int GetHashCode()
            {
            return Article.GetHashCode() ^ Price.GetHashCode() ^ Count.GetHashCode();
            }

        public override bool Equals(object obj)
            {
            ArticlePriceGroupInfo other = obj as ArticlePriceGroupInfo;
            if (other == null)
                {
                return false;
                }
            return other.Article.Equals(Article) && other.Price.Equals(Price) && other.Count.Equals(Count);
            }

        private WithMarginArticlePriceInfo withMarginArticlePriceInfo = null;

        /// <summary>
        /// Возвращает данные по распределению по наценкам
        /// </summary>
        public WithMarginArticlePriceInfo WithMarginArticlePriceInfo
            {
            get
                {
                return withMarginArticlePriceInfo;
                }
            }
        /// <summary>
        /// Добавляет разницу в цене с учетом наценки и без наценки для группы товаров
        /// </summary>
        /// <param name="rows">Строки</param>
        /// <param name="isPlus">Знак разницы</param>
        public void AppendMarginInfo(ArticleWithSomePriceCollection rows, bool isPlus)
            {
            withMarginArticlePriceInfo.AppendMarginInfo(rows, isPlus);
            }

        public int MinGreaterThanDefaultPrice
            {
            get
                {
                return withMarginArticlePriceInfo.MinGreaterThanDefaultPrice;
                }
            }
        }
    }
