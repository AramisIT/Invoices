using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает пол на основании группы/подгруппы товара
    /// </summary>
    public class MarksAndSpenserGenderHandler : CustomExpressionHandlerBase
        {
        long genderPropertyID = 0;

        public MarksAndSpenserGenderHandler( SystemInvoiceDBCache catalogsCachedData )
            : base( catalogsCachedData )
            {
            genderPropertyID = catalogsCachedData.PropertyOfGoodsCacheObjectsStore.GetCachedObjectId( new Cache.PropertyOfGoodsCache.PropertyOfGoodsCacheObject( "Пол" ) );
            }

        public override object ProcessRow(params object[] parameters)//Пол[Группа товара,Подгруппа товара,Код подгруппы]
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
            return catalogsCachedData.GetGender( groupOfGoodsName, subGroupOfGoodsName, groupCode );
            }
        }
    }
