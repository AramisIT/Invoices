using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает внутренний таможенный код из справочника номенклатура
    /// </summary>
    public class CustomsCodeFromArticleHandler : FromArticleHandlerBase
        {
        public CustomsCodeFromArticleHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cachedObject)//Артикул.ТамКод[Артикул,Торговая марка]
            {
            var customsCodeCacheObject = catalogsCachedData.CustomsCodesCacheStore.GetCachedObject(cachedObject.CustomsCodeId);
            if (customsCodeCacheObject != null)
                {
                return customsCodeCacheObject.Code;
                }
            return string.Empty;
            }
        }
    }
