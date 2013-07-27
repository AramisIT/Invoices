using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает исходное наименование из справочника номенклатура
    /// </summary>
    public class OriginalNameFromArticleHandler : FromArticleHandlerBase
        {
        public OriginalNameFromArticleHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cachedObject)//Артикул.НИсх[Артикул,Торговая марка]
            {
            return cachedObject.NameOriginal;
            }
        }
    }
