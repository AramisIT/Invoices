using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara
    {
    /// <summary>
    /// Используется для расчета внешнего/внутреннего таможенного кода
    /// </summary>
    public class ExternCustomsCodeZaraHandler : CustomExpressionHandlerBase
        {
        public ExternCustomsCodeZaraHandler(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        /// <summary>
        /// Рассчитывается как комбинация колонок
        /// </summary>
        public override object ProcessRow(params object[] parameters)//Тамож.Код.Вн.Зара[1,2,3,4]
            {
            StringBuilder builder = new StringBuilder();
            builder.Clear();
            string zeros = "00000000000";
            for (int i = 0; i < 4 && i < parameters.Length; i++)
                {
                string value = parameters[i].ToString();
                int length = value.Length;
                int haveToGetLength = 0;
                if (i == 0)
                    {
                    haveToGetLength = 4;
                    }
                else
                    {
                    haveToGetLength = 2;
                    }
                if (length < haveToGetLength)
                    {
                    builder.Append(zeros.Substring(0, haveToGetLength - length));
                    }
                builder.Append(value);
                }
            string summ = builder.ToString();
            return summ;
            }
        }
    }
