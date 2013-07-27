using System.Collections.Generic;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.GroupOfGoodsCreation
    {
    /// <summary>
    /// Создает новые группы товаров из табличной части инвойса, если их еще нету в базе.
    /// </summary>
    public class GroupsOfGoodsCreationHandler
    {
        private SystemInvoiceDBCache dbCache = null;
        public GroupsOfGoodsCreationHandler( SystemInvoiceDBCache dbCache )
            {
            this.dbCache = dbCache;
            }

        /// <summary>
        /// Создает новые группы/подгруппы товара которых небыло в системе
        /// </summary>
        public void CreateGroupsIfNeed( DataTable tableToProcess )
            {
            if (tryRefreshGroupOfGoods( tableToProcess ))
                {
                tryRefreshSubGroupOfGoods( tableToProcess );
                }
            }

        /// <summary>
        /// Создает новые подгруппы товара если их еще не было в системе
        /// </summary>
        private bool tryRefreshSubGroupOfGoods( DataTable tableToProcess )
            {
            dbCache.SubGroupOfGoodsObjectsCreator.Refresh();
            foreach (DataRow row in tableToProcess.Rows)
                {
                string groupName = row.TrySafeGetColumnValue<string>( InvoiceColumnNames.GroupOfGoods.ToString(), "" ).Trim();
                string subGroupName = row.TrySafeGetColumnValue<string>( InvoiceColumnNames.SubGroupOfGoods.ToString(), "" ).Trim();
                string groupCode = row.TrySafeGetColumnValue<string>( InvoiceColumnNames.GroupCode.ToString(), "" ).Trim();
                string manufacturer = row.TrySafeGetColumnValue<string>( InvoiceColumnNames.ItemContractor.ToString(), "" ).Trim();
                string tradeMark = row.TrySafeGetColumnValue<string>( InvoiceColumnNames.ItemTradeMark.ToString(), "" ).Trim();
                if (!string.IsNullOrEmpty( subGroupName ) || !string.IsNullOrEmpty( subGroupName ))
                    {
                    dbCache.SubGroupOfGoodsObjectsCreator.AddToCreationList( groupName, subGroupName, groupCode, manufacturer, tradeMark );
                    }
                }
            return dbCache.SubGroupOfGoodsObjectsCreator.TryCreate();
            }

        /// <summary>
        /// Создает новые группы товара если их еще не было в системе
        /// </summary>
        private bool tryRefreshGroupOfGoods( DataTable tableToProcess )
            {
            dbCache.GroupOfGoodsStore.Refresh();
            HashSet<string> gropOfGoodsNames = new HashSet<string>();
            foreach (DataRow row in tableToProcess.Rows)
                {
                string groupName = row.TrySafeGetColumnValue<string>( InvoiceColumnNames.GroupOfGoods.ToString(), "" ).Trim();
                gropOfGoodsNames.Add( groupName );
                }
            return dbCache.GroupOfGoodsCreator.TryCreateGroupOfGoods( gropOfGoodsNames );
            }
        }
    }
