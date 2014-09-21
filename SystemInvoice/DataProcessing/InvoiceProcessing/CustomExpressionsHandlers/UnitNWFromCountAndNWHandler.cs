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
        public UnitNWFromCountAndNWHandler(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        public override object ProcessRow(params object[] parameters)//•	ВесЕдТовара[Вес нетто,Количество]
            {
            if (parameters.Length < 2)
                {
                return null;
                }
            var netWeightTotalValue = parameters[0].ToString();
            var itemsCountValue = parameters[1].ToString();
            double netWeightTotal;
            double countTotal;
            if (!netWeightTotalValue.TryConvertToDouble(out netWeightTotal) ||
                !itemsCountValue.TryConvertToDouble(out countTotal))
                {
                return null;
                }
            double val = netWeightTotal / countTotal;
            return Math.Round(val, 3).ToString();
            }
        }
    }
