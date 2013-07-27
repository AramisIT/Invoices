using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Осуществляет перевод пола. На момент написания коментария уже неиспользовался
    /// </summary>
    public class GenderTranslatehandler : CustomExpressionHandlerBase
        {
        private long contentPropertyID = 0;
        public GenderTranslatehandler( SystemInvoiceDBCache dbCache )
            : base( dbCache )
            {
            contentPropertyID = dbCache.PropertyOfGoodsCacheObjectsStore.GetCachedObjectId( "Пол" );
            }

        public override object ProcessRow(params object[] parameters)//ПолПеревод[1]
            {
            if (parameters.Length < 1)
                {
                return "";
                }
            string genderNonUkrainianName = parameters[0].ToString();
            long SubGroupOfGoodsId = 0;
            string translated = catalogsCachedData.PropertyTypesCacheObjectsStore.GetGenderTranslation( SubGroupOfGoodsId, genderNonUkrainianName );
            return translated;
            }
        }
    }