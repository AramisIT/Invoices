using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает наименоваиние инвойса из загруженной номенклатуры
    /// </summary>
    public class InvoiceNameFromArticleHandler:FromArticleHandlerBase
        {
        public InvoiceNameFromArticleHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cachedObject)//Артикул.НИнв[Артикул,Торговая марка]
            {
            return cachedObject.NameInvoice;
            }
        }
    }
