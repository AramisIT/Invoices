using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara
    {
    /// <summary>
    /// Получает длинну стельки для обуви
    /// </summary>
    public class InsoleLengthHandler : CustomExpressionHandlerBase
        {
        public InsoleLengthHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//ДлиннаCтельки[Артикул,Торговая марка,Подгруппа товара,Код подгруппы,Размер]
            {                                                        //ДлиннаCтельки[Группа товара,Артикул,Торговая марка,Подгруппа товара,Код подгруппы,Размер]
            if (parameters.Length < 5)
                {
                return "";
                }
            string groupName = "";
            int startIndex = 0;
            if (parameters.Length == 6)
                {
                startIndex = 1;
                groupName = parameters[0].ToString();
                }
            string article = parameters[startIndex].ToString();
            string tradeMark = parameters[startIndex +1].ToString();
            string subGroupName = parameters[startIndex + 2].ToString();
            string subGroupCode = parameters[startIndex +3].ToString();
            string size = parameters[startIndex+ 4].ToString();
            return catalogsCachedData.GetInsoleLength( groupName, subGroupName, subGroupCode, article, tradeMark, size );
            }
        }
    }
