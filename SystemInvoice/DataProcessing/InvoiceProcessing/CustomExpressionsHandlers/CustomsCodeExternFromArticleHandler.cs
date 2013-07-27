using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает внешний таможенный код из справочника номенклатура
    /// </summary>
    class CustomsCodeExternFromArticleHandler: FromArticleHandlerBase
        {
        public CustomsCodeExternFromArticleHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cachedObject)//ТамКодВнешний[Артикул,Торговая марка]
            {
            return cachedObject.CustomsCodeExtern;
            }
        }
    }
