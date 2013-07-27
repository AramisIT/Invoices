using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors.SummDistribution;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors
    {
    /// <summary>
    /// Увеличивает/уменьшает значение цены номенклатурных позиций для изменния общей суммы на определенное значение. HAVE TO REFACTOR!!!
    /// </summary>
    public class SummCalculator
        {
        private const int criticalAmount = 5;
        private string itemsCountColumnName = null;
        private string itemsArticleColumnName = null;
        private string itemsPriceColumnName = null;
        private IEditableRowsSource editableRowsSource = null;

        public SummCalculator(IEditableRowsSource editableRowsSource)
            {
            this.editableRowsSource = editableRowsSource;
            itemsCountColumnName = SystemInvoice.Documents.InvoiceColumnNames.Count.ToString();
            itemsArticleColumnName = SystemInvoice.Documents.InvoiceColumnNames.Article.ToString();
            itemsPriceColumnName = SystemInvoice.Documents.InvoiceColumnNames.Price.ToString();
            }

        public void ArrangePrice(double diff)
            {
            IList<DataRow> rows = editableRowsSource.DisplayingRows;
            double distributedLeft = 0;
            bool isPlus = diff > 0;
            diff = Math.Abs(diff);
            if (diff == 0)
                {
                return;
                }
            Dictionary<ArticlePriceGroupInfo, ArticleWithSomePriceCollection> collectionForArticlesCountDictionary = getArticlesForCountDict(rows, isPlus);
            int currencyToProcessItems = getProcessCurencyElementaryItems(diff);//абсолютное значение (в копейках/центах... - в елементарных единицах) на которое нужно изменить сумму
            //список всех групп с одинаковой ценой и артикулом
            List<ArticlePriceGroupInfo> articlePriceGroupInfos = new List<ArticlePriceGroupInfo>(collectionForArticlesCountDictionary.Keys);
            articlePriceGroupInfos.Sort(new CountComparer());//сортируем от большего колличества к меньшему
            Dictionary<ArticlePriceGroupInfo, int> updateAmounts = new Dictionary<ArticlePriceGroupInfo, int>();//насколько мы можем изменить цену

            int amountToDistribute = currencyToProcessItems;
            int currentDistributed = amountToDistribute;
            int totalCount = 0;
            foreach (ArticlePriceGroupInfo gInfo in collectionForArticlesCountDictionary.Keys)
                {
                totalCount += gInfo.Count;
                }
            //распределяем пока есть что распределять
            while (currentDistributed > 0)
                {
                //пытаемся распределить
                if ((currentDistributed = this.distributePrice(updateAmounts, articlePriceGroupInfos, amountToDistribute, isPlus)) == amountToDistribute)
                    {//если ничего не распределилось - выходим
                    distributedLeft = currentDistributed;
                    break;
                    "Невозможно распределить сумму".AlertBox();
                    return;
                    }
                amountToDistribute = currentDistributed;
                }
            //проверяем что не бульше 5 копеек и что итоговая цена больше 0 для каждой номенклатурной позиции
            bool ischecked = true;
            foreach (ArticlePriceGroupInfo gInfo in updateAmounts.Keys)
                {
                double priceDouble = 0;
                if (!double.TryParse(gInfo.Price, out priceDouble))
                    {
                    ischecked = false;
                    }
                int currentPrice = (int)(priceDouble * 100);
                int priceToChange = updateAmounts[gInfo];
                if (priceToChange > 5)
                    {
                    ischecked = false;
                    break;
                    }
                if (!isPlus && (currentPrice - priceToChange) <= 0)
                    {
                    ischecked = false;
                    }
                }

            if (!ischecked)
                {
                "Невозможно распределить сумму".AlertBox();
                return;
                }
            //устанавливаем зачения
            foreach (ArticlePriceGroupInfo gInfo in updateAmounts.Keys)
                {
                int priceToChange = updateAmounts[gInfo];
                if (!isPlus)
                    {
                    priceToChange = -priceToChange;
                    }
                updateArticleUnitPrices(collectionForArticlesCountDictionary[gInfo], Math.Round(priceToChange * 1.0 / 100, 2));
                }
            }
        /// <summary>
        /// Распределяет сумму по группам с одинаковыми артикулами/ценой, начиная с групп с наибольшим колличеством товаром, заканчивая наименьшим
        /// </summary>
        private int distributePrice(Dictionary<ArticlePriceGroupInfo, int> updateAmounts, List<ArticlePriceGroupInfo> articlePriceGroupInfos, int amountToDistribute, bool isPlus)
            {
            try
                {
                int effectiveCount = 0;
                HashSet<ArticlePriceGroupInfo> groups = new HashSet<ArticlePriceGroupInfo>();
                for (int i = 0; i < articlePriceGroupInfos.Count; i++)
                    {
                    int currentGroupCount = 0;
                    ArticlePriceGroupInfo currentGroup = articlePriceGroupInfos[i];
                    if (updateAmounts.TryGetValue(currentGroup, out currentGroupCount))
                        {
                        if (currentGroupCount >= criticalAmount)
                            {
                            continue;
                            }
                        }
                    if ((currentGroup.MinGreaterThanDefaultPrice + effectiveCount) > amountToDistribute)// if ((currentGroup.Count + currentCount) > amountToDistribute)
                        {
                        continue;
                        }
                    else
                        {
                        effectiveCount += currentGroup.MinGreaterThanDefaultPrice;//currentGroup.Count;
                        groups.Add(currentGroup);
                        }
                    }
                if (effectiveCount == 0)
                    {
                    return amountToDistribute;
                    }
                int distributedCount = 0;
                int forItemUpdateAmount = Math.Min(criticalAmount, amountToDistribute / effectiveCount);
                int undistributableDefault = amountToDistribute - forItemUpdateAmount * effectiveCount;
                foreach (ArticlePriceGroupInfo groupInfo in groups)
                    {
                    int maxAllowedDistributionRate = groupInfo.MinGreaterThanDefaultPrice * forItemUpdateAmount + groupInfo.MinGreaterThanDefaultPrice * undistributableDefault / effectiveCount;
                    int oldValue = 0;
                    if (!updateAmounts.TryGetValue(groupInfo, out oldValue))
                        {
                        updateAmounts.Add(groupInfo, 0);
                        }
                    int updateValue = Math.Min(criticalAmount, oldValue + forItemUpdateAmount);
                    int updateDiff = updateValue - oldValue;

                    int distributedCurrentAmount = 0;
                    int canDistribute = 0;
                    while ((canDistribute = groupInfo.WithMarginArticlePriceInfo.GetUpdateRate(updateDiff, isPlus)) > maxAllowedDistributionRate && --updateDiff > 0) ;
                    if (updateDiff > 0)
                        {
                        groupInfo.WithMarginArticlePriceInfo.UpdatePriceInfo(updateDiff, isPlus);
                        updateValue = oldValue + updateDiff;
                        distributedCount += canDistribute; //!!!groupInfo.Count * updateDiff;//
                        updateAmounts[groupInfo] = updateValue;
                        }
                    }
                return amountToDistribute - distributedCount;
                }
            catch
                {
                }
            return 0;
            }



        /// <summary>
        /// Обновляет значения в табличной части инвойса
        /// </summary>
        /// <param name="collectionToChange">Информация об обновлямых значениях</param>
        /// <param name="diff">разница</param>
        private void updateArticleUnitPrices(ArticleWithSomePriceCollection collectionToChange, double diff)
            {
            double beforeTotalPrice = 0, afterTotalPrice = 0;
            foreach (DataRow dataRow in collectionToChange)
                {
                string priceStr = getRowPrice(dataRow);
                int count = getRowCount(dataRow);
                double priceValue = 0;
                if (double.TryParse(priceStr, out priceValue))
                    {
                    beforeTotalPrice += priceValue * count;
                    }
                }

            foreach (DataRow dataRow in collectionToChange)
                {
                string priceStr = getRowPrice(dataRow);
                double priceValue = 0;
                if (double.TryParse(priceStr, out priceValue))
                    {
                    priceValue += diff;
                    if (priceValue > 0)
                        {
                        dataRow[itemsPriceColumnName] = priceValue.ToString();
                        }
                    }
                }

            foreach (DataRow dataRow in collectionToChange)
                {
                string priceStr = getRowPrice(dataRow);
                int count = getRowCount(dataRow);
                double priceValue = 0;
                if (double.TryParse(priceStr, out priceValue))
                    {
                    afterTotalPrice += priceValue * count;
                    }
                }
            }

        private int getProcessCurencyElementaryItems(double diff)
            {
            return (int)Math.Round(diff * 100, 0);
            }


        /// <summary>
        /// Формирует группы строк с одинаковым артикулом и ценой, а так же форимрует данные о велечние наценки
        /// </summary>
        /// <param name="rows">Таблица</param>
        /// <param name="isPlus">Знак с которым будет учитыватся наценка (для того что бы при увеличении или уменьшении суммы мы потом использовали один алгоритм)</param>
        private Dictionary<ArticlePriceGroupInfo, ArticleWithSomePriceCollection> getArticlesForCountDict(IList<DataRow> rows, bool isPlus)
            {
            Dictionary<ArticlePriceGroupInfo, ArticleWithSomePriceCollection> articlesForCountDict = new Dictionary<ArticlePriceGroupInfo, ArticleWithSomePriceCollection>();
            List<DataRow> allRows = sortRowsByArticleAndPrice(rows);
            string currentArticle = "";
            string currentPrice = "";
            int currentCount = 0;
            bool initialized = false;
            ArticleWithSomePriceCollection currentCollection = null;
            foreach (DataRow row in allRows)
                {
                string rowArticle = getRowArticle(row);
                string rowPrice = getRowPrice(row);
                int rowCount = getRowCount(row);
                if (!initialized)
                    {
                    currentArticle = rowArticle;
                    currentPrice = rowPrice;
                    initialized = true;
                    currentCollection = new ArticleWithSomePriceCollection();
                    currentCollection.Add(row);
                    currentCount += rowCount;
                    continue;
                    }
                if (!currentArticle.Equals(rowArticle) || !currentPrice.Equals(rowPrice))
                    {
                    addGroupToDict(currentArticle, currentPrice, currentCount, articlesForCountDict, currentCollection);
                    currentCollection = new ArticleWithSomePriceCollection();
                    currentCount = 0;
                    currentArticle = rowArticle;
                    currentPrice = rowPrice;
                    }
                currentCount += rowCount;
                currentCollection.Add(row);
                }
            addGroupToDict(currentArticle, currentPrice, currentCount, articlesForCountDict, currentCollection);
            foreach (var pair in articlesForCountDict)
                {
                pair.Key.AppendMarginInfo(pair.Value, isPlus);
                }
            return articlesForCountDict;
            }

        private void addGroupToDict(string currentArticle, string currentPrice, int currentCount,
                                    Dictionary<ArticlePriceGroupInfo, ArticleWithSomePriceCollection> articlesForCountDict, ArticleWithSomePriceCollection currentCollection)
            {
            ArticlePriceGroupInfo groupInfo = new ArticlePriceGroupInfo(currentArticle, currentPrice, currentCount);
            if (!articlesForCountDict.ContainsKey(groupInfo))
                {
                articlesForCountDict.Add(groupInfo, currentCollection);
                }
            }

        private int getRowCount(DataRow row)
            {
            string countStr = row.TrySafeGetColumnValue(itemsCountColumnName, string.Empty);
            int count = 0;
            int.TryParse(countStr, out count);
            return count;
            }

        private string getRowPrice(DataRow row)
            {
            return row.TrySafeGetColumnValue(itemsPriceColumnName, string.Empty);
            }

        private string getRowArticle(DataRow row)
            {
            return row.TrySafeGetColumnValue(this.itemsArticleColumnName, string.Empty);
            }

        private List<DataRow> sortRowsByArticleAndPrice(IList<DataRow> dataRowCollection)
            {
            List<DataRow> inputRowsCollection = new List<DataRow>();
            foreach (DataRow row in dataRowCollection)
                {
                inputRowsCollection.Add(row);
                }
            inputRowsCollection.Sort(new ArticleAndPriceRowComparer(this.itemsArticleColumnName, this.itemsPriceColumnName));
            return inputRowsCollection;
            }
        }
    }
