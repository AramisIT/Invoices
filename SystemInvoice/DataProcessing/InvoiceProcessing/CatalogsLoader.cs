using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.GroupOfGoodsCreation;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {
    /// <summary>
    /// Пытается создать новые элементы справочников на основании данных текущего документа
    /// </summary>
    class CatalogsLoader
        {
        private SystemInvoiceDBCache cachedData = null;

        public CatalogsLoader(SystemInvoiceDBCache invoiceDBCache)
            {
            this.cachedData = invoiceDBCache;
            }

        public bool TryCreateNewCatalogsItems(DataTable dataTable)
            {

            bool isInCurrentTransaction = TransactionManager.TransactionManagerInstance.IsInTransaction();
            try
                {
                if (!isInCurrentTransaction)
                    {
                    //Начинаем транзакцию, что бы не было ситуации когда несколько пользователей одновременно добавят одни и теже данные
                    TransactionManager.TransactionManagerInstance.BeginBusinessTransaction();
                    }
                if (!tryCheckLoadedTradeMarks(dataTable))
                    {
                    return false;
                    }
                if (!tryCheckLoadedManufacturers(dataTable))
                    {
                    return false;
                    }
                this.CreateNonExistedGroupOfGoods(dataTable);
                return createNomenclatures(dataTable);
                }
            finally
                {
                if (!isInCurrentTransaction)
                    {
                    TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                    }
                }
            }

        private bool tryCheckLoadedManufacturers(DataTable currentProcessedTable)
            {
            HashSet<string> inExcelTableManufacturers = this.selectFromTableManufacturers(currentProcessedTable);
            if (inExcelTableManufacturers != null && !cachedData.ManufacturersCreator.TryCreateNewManufacturers(inExcelTableManufacturers))
                {
                "Ошибка создания новых производителей".AlertBox();
                return false;
                }
            return true;
            }

        private bool tryCheckLoadedTradeMarks(DataTable currentProcessedTable)
            {
            HashSet<string> inExcelTableTradeMarks = this.selectFromTableTradeMarks(currentProcessedTable);
            if (inExcelTableTradeMarks != null && !cachedData.TradeMarksCreator.TryCreateNewTradeMarks(inExcelTableTradeMarks))
                {
                "Ошибка при создании новых торговых марок".AlertBox();
                return false;
                }
            return true;
            }

        private void CreateNonExistedGroupOfGoods(DataTable currentProcessedTable)
            {
            GroupsOfGoodsCreationHandler creationHandler = new GroupsOfGoodsCreationHandler(cachedData);
            creationHandler.CreateGroupsIfNeed(currentProcessedTable);
            }

        private bool createNomenclatures(DataTable currentProcessedTable)
            {
            DateTime from = DateTime.Now;
            var nomenclatureCreator = cachedData.NomenclatureCreator;
            DataTable table = currentProcessedTable;
            bool loadTradeMarks = table.Columns.Contains(ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME);
            if (table == null || !table.Columns.Contains(ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME) || !table.Columns.Contains(ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME))
                {
                return false;
                }
            nomenclatureCreator.BeginCreation();
            foreach (DataRow row in currentProcessedTable.Rows)
                {
                addNomenclatureToCreationListIfNeed(nomenclatureCreator, row);
                }
            double period = (DateTime.Now - from).TotalMilliseconds;
            return nomenclatureCreator.TryCreate();
            }

        private void addNomenclatureToCreationListIfNeed(NomenclatureObjectsCreator nomenclatureCreator, DataRow row)
            {
            string trademark = Helpers.InvoiceDataRetrieveHelper.GetRowTradeMark(row);
            string article = Helpers.InvoiceDataRetrieveHelper.GetRowArticle(row);
            if (nomenclatureCreator.CanAddNewNomenclature(article, trademark))
                {
                string manufacturerName = Helpers.InvoiceDataRetrieveHelper.GetRowManufacturerName(row);
                string customCodeName = Helpers.InvoiceDataRetrieveHelper.GetRowCustomsCode(row);
                string countryName = Helpers.InvoiceDataRetrieveHelper.GetRowCountryCode(row);
                string unitOfMeasureCode = Helpers.InvoiceDataRetrieveHelper.GetRowUnitOfMeasure(row);
                string externCode = Helpers.InvoiceDataRetrieveHelper.GetRowCustomsCodeExtern(row);
                string barCode = Helpers.InvoiceDataRetrieveHelper.GetRowBarCode(row);
                double netWightFrom = 0;
                double netWeightTo = 0;
                int totalCount = Helpers.InvoiceDataRetrieveHelper.GetNomenclaturesCount(row);
                double totalNetWeight = Helpers.InvoiceDataRetrieveHelper.GetRowNetWeight(row);
                double unitNetWeight = Helpers.InvoiceDataRetrieveHelper.GetRowItemNetWeight(row);
                if (totalCount > 0 && totalNetWeight > 0)
                    {
                    netWightFrom = Math.Round(Math.Floor(1000 * totalNetWeight / totalCount) / 1000, 3);
                    netWeightTo = Math.Round(Math.Ceiling(1000 * totalNetWeight / totalCount) / 1000, 3);
                    }
                else
                    {
                    netWeightTo = netWightFrom = unitNetWeight;
                    }
                double grossWeight = Helpers.InvoiceDataRetrieveHelper.GetRowGrossWeight(row);
                double price = Helpers.InvoiceDataRetrieveHelper.GetRowPrice(row);
                string nameOriginal = Helpers.InvoiceDataRetrieveHelper.GetRowOriginalName(row);
                string nameDecl = Helpers.InvoiceDataRetrieveHelper.GetRowDeclarationName(row);
                string nameInvoice = Helpers.InvoiceDataRetrieveHelper.GetRowInvoiceName(row);
                string groupName = Helpers.InvoiceDataRetrieveHelper.GetRowGroupName(row);
                string subGroupName = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupName(row);
                string subGroupCode = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupCode(row);
                long groupId = cachedData.GetSubGroupId(groupName, subGroupName, subGroupCode);
                nomenclatureCreator.AddNomenclature(article, trademark, manufacturerName, customCodeName, nameInvoice, countryName,
                    unitOfMeasureCode, externCode, barCode, netWightFrom, netWeightTo, grossWeight, price, nameOriginal, nameDecl, groupId);
                }
            }

        /// <summary>
        /// Формирует список всех производителей загруженных из входящего файла
        /// </summary>
        private HashSet<string> selectFromTableManufacturers(DataTable table)
            {
            string columnName = ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME;
            return selectUniqueStringValues(table, columnName);
            }

        /// <summary>
        /// Формирует список всех торговых марок загруженных из входящего файла
        /// </summary>
        private HashSet<string> selectFromTableTradeMarks(DataTable table)
            {
            string columnName = ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME;
            return selectUniqueStringValues(table, columnName);
            }

        private HashSet<string> selectUniqueStringValues(DataTable table, string columnName)
            {
            if (table == null || string.IsNullOrEmpty(columnName) || !table.Columns.Contains(columnName))
                {
                return null;
                }
            HashSet<string> items = new HashSet<string>();
            foreach (DataRow row in table.Rows)
                {
                string newItem = row.TryGetColumnValue<string>(columnName, "").Trim();
                if (!string.IsNullOrEmpty(newItem) && !items.Contains(newItem))
                    {
                    items.Add(newItem);
                    }
                }
            return items;
            }
        }
    }
