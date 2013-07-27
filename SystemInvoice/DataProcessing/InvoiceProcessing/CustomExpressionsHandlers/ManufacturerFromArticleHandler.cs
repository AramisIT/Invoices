using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает производителя из загруженной номенклатуры
    /// </summary>
    public class ManufacturerFromArticleHandler:FromArticleHandlerBase
        {
        public ManufacturerFromArticleHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cachedObject)//Артикул.Производ[Артикул,Торговая марка]
            {
            return cachedObject.ManufacturerId;
            }
        }
    }
