using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Умножает друг на друга 2 значения, результат выводится с точностью до трех знаков
    /// </summary>
    public class MultiplyExpressionHandler : CustomExpressionHandlerBase
        {
        public MultiplyExpressionHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//Произв[0,1]
            {
            if (parameters.Length < 2)
                {
                return "";
                }
            double param1, param2;
            if (double.TryParse( parameters[0].ToString(), out param1 ) && double.TryParse( parameters[1].ToString(), out param2 ))
                {
                return Math.Round( param1 * param2, 3 ).ToString();
                }
            return "";
            }
        }
    }
