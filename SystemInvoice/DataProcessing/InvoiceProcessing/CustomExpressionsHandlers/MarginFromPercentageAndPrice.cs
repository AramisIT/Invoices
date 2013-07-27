using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает абсолютное значение наценки, на основании процента наценки и цены
    /// </summary>
    public class MarginFromPercentageAndPrice: CustomExpressionHandlerBase
        {
        public MarginFromPercentageAndPrice( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//Н_Процент[Цена,% Наценки]
            {
            if (parameters.Length < 2)
                {
                return null;
                }
            string unitPriceStr = parameters[0].ToString();
            string percentageStr = parameters[1].ToString();
            double unitPrice, percentage;
            if (!double.TryParse( unitPriceStr, out unitPrice ) || 
                !double.TryParse( percentageStr, out percentage ))
                {
                return null;
                }
            double calclatedValue = Math.Round( unitPrice * percentage, 2 );
            return calclatedValue.ToString();
            }
        }
    }
