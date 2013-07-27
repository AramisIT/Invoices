using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors.SummDistribution
    {
    /// <summary>
    /// Сравнивает информацию о группах строк по колличеству, для сортировки от большего колличества у  меньшему
    /// </summary>
    public class CountComparer : IComparer<ArticlePriceGroupInfo>
        {
        public int Compare(ArticlePriceGroupInfo x, ArticlePriceGroupInfo y)
            {
            return -x.Count.CompareTo(y.Count);
            }
        }
    }
