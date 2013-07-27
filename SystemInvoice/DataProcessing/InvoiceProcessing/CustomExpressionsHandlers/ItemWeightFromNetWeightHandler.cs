using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает вес единицы товара из общего веса нетто и колличества. На момент написания коментария нигде не использовался
    /// </summary>
    public class ItemWeightFromNetWeightHandler:CustomExpressionHandlerBase
        {
        public ItemWeightFromNetWeightHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//Ед.Товара[0,1]
            {
            if (parameters.Length < 2)
                {
                return "";
                }
            object netWeightValue = parameters[0];
            object itemsCountValue = parameters[1];
            double netWeight;
            int itemsCount;
            if (!double.TryParse( netWeightValue.ToString(), out netWeight ) ||
                !int.TryParse( itemsCountValue.ToString(), out itemsCount ))
                {
                return "";
                }
            return Math.Round(((double)netWeight / itemsCount),3).ToString();
            }
        }
    }
