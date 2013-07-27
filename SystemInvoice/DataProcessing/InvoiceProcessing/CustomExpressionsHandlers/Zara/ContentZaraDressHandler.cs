using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara
    {
    /// <summary>
    /// Расчитывает состав для зары, перебирая колонки указанные в параметрах исходя из принцыпа - первая колонка - процент состава, следующая материал, дальше снова процент и т.д.
    /// </summary>
    public class ContentZaraDressHandler:CustomExpressionHandlerBase
        {
        public ContentZaraDressHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//Состав[1,2,...]
            {
            string contentStr = "";
            for (int i = 0; i < parameters.Length / 2; i++)
                {
                string contentPercent = parameters[i].ToString().Trim();
                string contentName = parameters[i + 1].ToString().Trim();
                double doublePrecentage = 0;
                if (!string.IsNullOrEmpty( contentName ) && double.TryParse( contentPercent, out doublePrecentage ))
                    {
                    contentStr += string.Format( "{0}% {1} ", doublePrecentage, contentName );
                    }
                }
            return contentStr;
            }
        }
    }
