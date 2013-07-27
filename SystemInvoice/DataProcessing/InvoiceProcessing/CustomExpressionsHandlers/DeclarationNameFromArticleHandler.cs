using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает наименование декларации из справочника номенклатура
    /// </summary>
    public class DeclarationNameFromArticleHandler : FromArticleHandlerBase
        {
        public DeclarationNameFromArticleHandler(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cachedObject)//Артикул.НДекл[Артикул,Торговая марка]
            {
            return cachedObject.NameDecl;
            }
        }
    }
