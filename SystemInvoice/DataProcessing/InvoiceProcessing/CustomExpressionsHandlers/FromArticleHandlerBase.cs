using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Базовый клас для получения данных из справочника номенклатуры. Для получения таких данных необходимо чтобы существовал 
    /// елемент справочника с таким же артикулом и торговой маркой как и в передаваемых параметрах
    /// </summary>
    public abstract class FromArticleHandlerBase : CustomExpressionHandlerBase
        {
        public FromArticleHandlerBase(SystemInvoiceDBCache data)
            : base(data)
            {
            }

        public override object ProcessRow(params object[] parameters)//класы - наследники должны принимать параметры в формате: [Артикул,Торговая марка, n],где  n - опциональный параметр который возвращает значение по умолчанию, если не найдена для обрабатываемой строки номенклатура
            {
            string article = null, tradeMark = null, defaultValue = string.Empty;
            if (parameters.Length >= 3)
                {
                defaultValue = parameters[2].ToString();
                }
            if (parameters.Length >= 2)
                {
                tradeMark = parameters[1].ToString();
                }
            if (parameters.Length >= 1)
                {
                article = parameters[0].ToString();
                }
            NomenclatureCacheObject cachedObject = base.catalogsCachedData.GetCachedNomenclature(article, tradeMark);
            if (cachedObject == null)
                {
                return defaultValue;
                }
            return ProcessExpression(cachedObject);
            }

        /// <summary>
        /// Возвращает нужное значение кешированного объекта
        /// </summary>
        protected abstract object ProcessExpression(NomenclatureCacheObject cachedObject);
        }
    }
