using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using Aramis.DatabaseConnector;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает торговую марку по артикулу и текущему контрагенту документа Инвойс
    /// </summary>
    class TradeMarkFromArticleHandler : CustomExpressionHandlerBase
        {
        private Func<IContractor> getCurrentContractor;

        public TradeMarkFromArticleHandler(SystemInvoiceDBCache cachedData, Func<IContractor> getCurrentContractor)
            : base(cachedData)
            {
            this.getCurrentContractor = getCurrentContractor;
            }

        public override object ProcessRow(params object[] parameters)//класы - наследники должны принимать параметры в формате: [Артикул]
            {
            string article = null;
            if (parameters.Length >= 1)
                {
                article = parameters[0].ToString();
                }

            var contractor = getCurrentContractor();
            if (string.IsNullOrEmpty(article) || contractor == null || contractor.IsNew)
                {
                return string.Empty;
                }

            var q = DB.NewQuery(@"select top 1 rtrim(tm.Description) TradeMark, n.Article
	from Nomenclature n
	join TradeMark tm on tm.Id = n.TradeMark and n.Contractor = @Contractor and n.Article = @Article");
            q.AddInputParameter("Contractor", contractor.Id);
            q.AddInputParameter("Article", article);

            return q.SelectString();
            }
        }
    }
