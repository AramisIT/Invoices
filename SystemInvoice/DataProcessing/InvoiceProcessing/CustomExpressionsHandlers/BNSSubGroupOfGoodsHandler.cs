using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает имя подгруппы для БНС - а как первые 4 знака из колонки во входящем файле
    /// </summary>
    public class BNSSubGroupOfGoodsHandler : CustomExpressionHandlerBase
        {
        public BNSSubGroupOfGoodsHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//BNSПодгруппа[1]
            {
            if (parameters.Length < 1)
                {
                return null;
                }
            string totalStr = parameters[0].ToString();
            return totalStr.Length <= 4 ? totalStr : totalStr.Substring( 0, 4 );
            }
        }
    }
