using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Article
    {
    /// <summary>
    /// Проверяет артикул во входящем файле
    /// </summary>
    public class ArticleChecker : LoadedDocumentCheckerBase
        {
        public ArticleChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            if (rowToCheck == null)
                {
                return;
                }
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            string tradeMark =  rowToCheck.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME, "");
            long tradeMarkID = dbCache.TradeMarkCacheObjectsStore.GetTradeMarkId(tradeMark);
            string article = rowToCheck.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME, "");
            if (nomenclatureId > 0)
                {
                var nomenclatureCached = dbCache.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureId);
                string cachedArticle = nomenclatureCached == null ? string.Empty : nomenclatureCached.Article;
                if (!cachedArticle.Equals(article))
                    {
                    //в целом данное условие при правильной работе програмы не может выполнятся, поскольку для получения номенклатуры нам нужен артикул
                    AddError(ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME, new ArticleError(article, cachedArticle, ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME));
                    }
                return;
                }
            //Артикул помечается ошибкой - если он незаполнен (пустое значение) или если он заполнен но для этой торговой марки товара с таким артикулом не существует,
            //если торговой марки нету в системе - артикул не помечается ошибкой (когда заполнен), поскольку в этом случае мы не можем найти товар в первую очередь из-за торговой марки
            if (tradeMarkID != 0 || string.IsNullOrEmpty(article))
                {
                AddError(ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME, new ArticleError());
                }
            }
        }
    }
