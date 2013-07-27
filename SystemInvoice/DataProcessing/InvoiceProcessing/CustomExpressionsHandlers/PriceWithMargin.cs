using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает цену с учетом наценки
    /// </summary>
    public class PriceWithMargin : CustomExpressionHandlerBase
        {
        public  PriceWithMargin( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow( params object[] parameters )//С_Наценкой[Цена,Процент наценки]
            {
            if (parameters.Length < 2)
                {
                return null;
                }
            object itemPriceValue = parameters[0];
            object itemMarginValue = parameters[1];
            double itemPrice, margin;
            if (!double.TryParse( itemMarginValue.ToString(), out margin ) ||
                !double.TryParse( itemPriceValue.ToString(), out itemPrice ))
                {
                return string.Empty;
                }
            double result = Math.Round( itemPrice  + margin, 2 );
            return result.ToString();
            }
        }
    }
