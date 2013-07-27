using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает код единицы измерения по ее наименованию
    /// </summary>
    public class GetUnitOfMeasureCodeFromNameHandler : CustomExpressionHandlerBase
        {
        public GetUnitOfMeasureCodeFromNameHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        public override object ProcessRow(params object[] parameters)//Ед.Код[1]
            {
            string shortName = (string)parameters[0];
            return catalogsCachedData.UnitOfMeasureCacheObjectsStore.GetUnitOfMeasureCodeFromShortName( shortName );
            }
        }
    }
