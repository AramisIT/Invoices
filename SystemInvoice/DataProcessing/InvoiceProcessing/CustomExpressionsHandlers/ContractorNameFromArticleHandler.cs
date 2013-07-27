using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает контрагента из номенклатуры
    /// </summary>
    public class ContractorNameFromArticleHandler:FromArticleHandlerBase
        {
        public ContractorNameFromArticleHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cacheObject)//Артикул.Контр[Артикул,Торговая марка]
            {
            var manufacturerCachedObject = catalogsCachedData.ManufacturerCacheObjectsStore.GetCachedObject(cacheObject.ManufacturerId);
            if (manufacturerCachedObject != null)
                {
                return manufacturerCachedObject.ManufacturerName;
                }
            return string.Empty;
            }
        }
    }
