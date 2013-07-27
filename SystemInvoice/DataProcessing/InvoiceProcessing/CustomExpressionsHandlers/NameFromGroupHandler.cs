using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает наименование для группы/подгруппы товара
    /// </summary>
    public class NameFromGroupHandler : CustomExpressionHandlerBase
        {
        public NameFromGroupHandler(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        public override object ProcessRow(params object[] parameters)//Наименование[Группа товара,Подгруппа товара,Код подгруппы]
            {
            string groupOfGoodsName = "";
            string subGroupOfGoodsName = "";
            string groupCode = "";
            if (parameters.Length < 3)
                {
                return "";
                }
            groupOfGoodsName = (string)parameters[0];
            subGroupOfGoodsName = (string)parameters[1];
            groupCode = (string)parameters[2];
            return catalogsCachedData.GetName(groupOfGoodsName, subGroupOfGoodsName, groupCode);
            }
        }
    }
