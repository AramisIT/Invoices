using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zepter
    {
    /// <summary>
    /// Получает состав для Цептера, для новых позицый формирует наименование с учетом состава
    /// </summary>
    public class ZepterDeclarationHandler : CustomExpressionHandlerBase
        {
        public ZepterDeclarationHandler(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        public override object ProcessRow(params object[] parameters)//Цептер.Декл[Артикул,Торговая марка,1,2] 1 - колонка с наименованием, 2 - колонка с соством
            {
            string article = null, tradeMark = null, fromDocumentDeclaration = string.Empty, content = string.Empty;
            if (parameters.Length >= 4)
                {
                content = parameters[3].ToString();
                }
            if (parameters.Length >= 3)
                {
                fromDocumentDeclaration = parameters[2].ToString();
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
            if (cachedObject != null)//если номенклатура найдена - возвращаем значение из базы
                {
                return cachedObject.NameDecl;
                }
            //если не найдена - фомируем из колонок декларация и состав
            string declaration = fromDocumentDeclaration;
            if (!string.IsNullOrEmpty(content))
                {
                declaration = string.Format("{0} ({1})", declaration, content);
                }
            return declaration;
            }
        }
    }
