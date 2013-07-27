using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает перевод размера на украинскую систему
    /// </summary>
    public class SizeTranslateExpressionHandler:CustomExpressionHandlerBase
        {
        public SizeTranslateExpressionHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow( params object[] parameters )//РазмерПеревод[Группа товара,7,Подгруппа товара,Код подгруппы,ТорговаяМарка,Артикул]
            {
            string groupName = "";
            string sizeEn;
            string subGroupName;
            string subGroupCode;

            string article = "";
            string tradeMark = "";

            if(parameters.Length<3)
                {
                if(parameters.Length>=1)
                    {
                    return parameters[0];
                    }
                return string.Empty;
                }
            int startOtherFrom = 0;
            if (parameters.Length >= 4)
                {
                startOtherFrom = 1;
                groupName = (string)parameters[0];
                }
            if(parameters.Length>=5)
                {
                tradeMark = (string) parameters[4];
                }
            if(parameters.Length>=6)
                {
                article = (string) parameters[5];
                }
            sizeEn = (string)parameters[startOtherFrom];
            subGroupName = (string)parameters[startOtherFrom + 1];
            subGroupCode = (string)parameters[startOtherFrom + 2];
            return catalogsCachedData.GetTranslatedSize(groupName, subGroupName, subGroupCode, sizeEn,
                                                        tradeMark, article);
            }
        }
    }
