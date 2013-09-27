using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.Helpers;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Рассчитывает сумму на основании цены и количества
    /// </summary>
    public class SummCalculationHandler : CustomExpressionHandlerBase
        {
        public SummCalculationHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//Сумма[Количество,Цена с наценкой]
            {
            if (parameters.Length < 2)
                {
                return null;
                }
            object itemsCountValue = parameters[0];
            object itemPriceValue = parameters[1];
            object marginValue = parameters.Length >= 3 ? parameters[2] : "0.0";
            double itemPrice, margin;
            int itemsCount;
            if (!int.TryParse( itemsCountValue.ToString(), out itemsCount ) ||
                !double.TryParse( itemPriceValue.ToString(), out itemPrice ))
                {
                return null;
                }
            if (!double.TryParse( marginValue.ToString(), out margin ))
                {
                margin = 0;
                }
            if (itemsCount == 0)
                {
                return "0";
                }
            double result = InvoiceProcessingHelper.GetTotalSumm( itemPrice, itemsCount, margin );
            return result.ToString();
            }
        }
    }
