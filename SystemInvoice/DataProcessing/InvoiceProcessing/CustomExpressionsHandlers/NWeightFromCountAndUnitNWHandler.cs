using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает вес нетто из веса единицы товара и колличества
    /// </summary>
    public class NWeightFromCountAndUnitNWHandler : CustomExpressionHandlerBase
        {
        public NWeightFromCountAndUnitNWHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//ВесНетто[Вес ед. товара,Количество]
            {
            if (parameters.Length < 2)
                {
                return null;
                }
            object unitNetWeightValue = parameters[0];
            object itemsCountValue = parameters[1];
            double unitNetWeight;
            int countTotal;
            if (!double.TryParse( unitNetWeightValue.ToString(), out unitNetWeight ) ||
                !int.TryParse( itemsCountValue.ToString(), out countTotal ))
                {
                return null;
                }
            double val = unitNetWeight * countTotal;
            return Math.Round( val, 3 ).ToString();
            }
        }
    }
