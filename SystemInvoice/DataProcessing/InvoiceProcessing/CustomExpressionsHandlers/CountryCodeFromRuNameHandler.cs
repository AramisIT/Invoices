using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает международный буквенный код страны по русскоязычному наименованию
    /// </summary>
    public class CountryCodeFromRuNameHandler:CustomExpressionHandlerBase
        {
        public CountryCodeFromRuNameHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//СтранаРусКод[1]
            {
            if (parameters.Length < 1)
                {
                return "";
                }
            string ruName = parameters[0].ToString();
            if (string.IsNullOrEmpty( ruName ))
                {
                return string.Empty;
                }
            return catalogsCachedData.CountryCahceObjectsStore.GetShortNameForCountryRuName( ruName );
            }
        }
    }
