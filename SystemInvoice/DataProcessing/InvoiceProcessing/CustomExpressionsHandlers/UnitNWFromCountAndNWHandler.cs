using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает вес единицы товара из колличества и веса нетто
    /// </summary>
    public class UnitNWFromCountAndNWHandler : CustomExpressionHandlerBase
        {
        public UnitNWFromCountAndNWHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//•	ВесЕдТовара[Вес нетто,Колличество]
            {
            if (parameters.Length < 2)
                {
                return null;
                }
            object netWeightTotalValue = parameters[0];
            object itemsCountValue = parameters[1];
            double netWeightTotal;
            int countTotal;
            if (!double.TryParse( netWeightTotalValue.ToString(), out netWeightTotal ) ||
                !int.TryParse( itemsCountValue.ToString(), out countTotal ))
                {
                return null;
                }
            double val = netWeightTotal / countTotal;
            return Math.Round( val, 3 ).ToString();
            }
        }
    }
