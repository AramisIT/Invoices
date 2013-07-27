using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara
    {
    /// <summary>
    /// Формирует дату инвойса как комбинацию колонок - день,месяц,год. Используется в заре
    /// </summary>
    public class InvoiceDateZaraHandler : CustomExpressionHandlerBase
        {
        public InvoiceDateZaraHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//ДатаИнвойса[1,2,3]
            {
            if (parameters.Length < 3)
                {
                return "";
                }
            string day = parameters[0].ToString();
            string month = parameters[1].ToString();
            string year = parameters[2].ToString();
            return string.Format( "{0}.{1}.{2}", day, month, year );
            }
        }
    }
