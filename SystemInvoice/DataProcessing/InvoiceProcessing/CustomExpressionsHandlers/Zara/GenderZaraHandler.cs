using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara
    {
    /// <summary>
    /// Определяет пол в заре как сокращение от наименования пола в входящем файле
    /// </summary>
    public class GenderZaraHandler : CustomExpressionHandlerBase
        {
        public GenderZaraHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//ПолЗара[1]
            {
            if (parameters.Length < 1 || !(parameters[0] is string))
                {
                return "";
                }
            string gender = (string)parameters[0];
            if (string.IsNullOrEmpty( gender ) || gender.Length < 3)
                {
                return gender;
                }
            return string.Concat( gender.Substring( 0, 3 ).ToUpper(), "." );
            }
        }
    }
