using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.CatalogsInTableSearch
    {
    /// <summary>
    /// Находит соответствия строк табличной части инвойса соответствующим елементам справочников, заполняет соответствие в табличную часть
    /// </summary>
    public class CatalogsSearchHandler
        {
        private SystemInvoiceDBCache dbCache = null;

        public CatalogsSearchHandler(SystemInvoiceDBCache dbCache)
            {
            this.dbCache = dbCache;
            }

        public void FindCatalogs(DataTable tableToProcess)
            {
            foreach (DataRow row in tableToProcess.Rows)
                {
                this.processRowCatalogsMappings(row);
                }
            }

        private void processRowCatalogsMappings(DataRow row)
            {
            //Ищем номенклатуру, подгруппу.
            string article = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.Article.ToString(), "");
            string tradeMarkName = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.ItemTradeMark.ToString(), "");
            string groupName = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.GroupOfGoods.ToString(), "");
            string subGroupName = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.SubGroupOfGoods.ToString(), "");
            string subGroupCode = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.GroupCode.ToString(), "");
            long nomenlatureId = dbCache.GetNomenclatureId(article, tradeMarkName);
            long groupId = dbCache.GetSubGroupId(groupName, subGroupName, subGroupCode);
            long currentNomenclature =
                row.TrySafeGetColumnValue<long>(ProcessingConsts.ColumnNames.FOUNDED_NOMENCLATURE_COLUMN_NAME, 0);
            long currentSubGroupOfGoods =
                row.TrySafeGetColumnValue<long>(ProcessingConsts.ColumnNames.FOUNDED_SUB_GROUP_OF_GOODS, 0);
            //записываем в табличную часть инвойса найденные значения
            if (currentNomenclature != nomenlatureId)
                {
                row[ProcessingConsts.ColumnNames.FOUNDED_NOMENCLATURE_COLUMN_NAME] = nomenlatureId;
                }
            if (currentSubGroupOfGoods != groupId)
                {
                row[ProcessingConsts.ColumnNames.FOUNDED_SUB_GROUP_OF_GOODS] = groupId;
                }
            }
        }
    }
