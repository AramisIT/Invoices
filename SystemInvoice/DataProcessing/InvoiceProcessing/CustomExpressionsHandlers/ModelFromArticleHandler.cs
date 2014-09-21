using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    public class ModelFromArticleHandler : FromArticleHandlerBase
        {
        public ModelFromArticleHandler(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cacheObject)
            {
            if (cacheObject != null)
                {
                return cacheObject.Model;
                }
            return string.Empty;
            }
        }
    }
