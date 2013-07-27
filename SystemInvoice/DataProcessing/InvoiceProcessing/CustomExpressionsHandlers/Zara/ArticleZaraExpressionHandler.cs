using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara
    {
    /// <summary>
    /// Рассчитывает артикул для зары. 3-й параметр при указании 4-х параметров не участвует в определении артикула (изначально было 4 параметра), но он сохранен, что б не сломались старые форматы загрузки
    /// </summary>
    public class ArticleZaraExpressionHandler : CustomExpressionHandlerBase
        {
        public ArticleZaraExpressionHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//АртикулЗара[1,2,3,4]
            {                                                        //АртикулЗара[1,2,3]
            int length = parameters.Length;
            if (length < 3)
                {
                return null;
                }
            int lastIndex = length - 1;
            string lastPart = parameters[lastIndex].ToString().Replace("0","");
            if (!string.IsNullOrEmpty( lastPart ))
                {
                lastPart = string.Format( "-{0}", lastPart );
                }
            string article = string.Format( "{0}/{1}{2}", parameters[0], parameters[1],lastPart );
            return article;
            }
        }
    }
