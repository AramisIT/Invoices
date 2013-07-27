using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Excel.DataFormatting;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Базовый клас для обработки загружаемых данных
    /// </summary>
    public abstract class CustomExpressionHandlerBase : ICustomExpressionHandler
        {
        protected SystemInvoiceDBCache catalogsCachedData = null;

        public CustomExpressionHandlerBase( SystemInvoiceDBCache catalogsCachedData )
            {
            this.catalogsCachedData = catalogsCachedData;
            }

        public abstract object ProcessRow( params object[] parameters );
        }
    }
