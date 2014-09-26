using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.ApprovalsProcessing.ByNomenclatureUpdating;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.ApprovalsModification;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.CatalogsInTableSearch;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.CustomDataProcessing;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.Graf31Calculation;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.GroupOfGoodsCreation;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.RowsGrouping;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.SpecificCachesManagement;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using Aramis.DatabaseConnector;
using Aramis.Core;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {
    /// <summary>
    /// Выполняет обработку документа после загрузки - поиск разрешительных, группировка строк, расчет графы 31 и т.д.
    /// </summary>
    public class InvoiceLoadedDocumentHandler
        {
        private CatalogsSearchHandler catalogsSearchHandler = null;
        private ApprovalsSearcher approvalsSearcher = null;
        private GroupingHandler groupingHandler = null;
        private GrafCalcHandler grafCalculationHandler = null;
        private GroupsOfGoodsCreationHandler groupOfGoodsCreationHandler = null;
        private InvoiceNumberBNSHandler bnsCreateHandler = null;
        private NamesTranslationHandler namesTranslationHandler = null;
        private Invoice invoice;
        private NomenclatureRemovingHistoryUpdater nomenclatureRemovingHistoryUpdater = null;
        private InvoiceAfterAprovalsUpdater invoiceAfterAprovalsUpdater = null;
        private UnitOfMeasureCodeRetreiveHandler unitOfMeasureCodeRetreiveHandler = null;
        private NetWeightUpdater netWeightUpdater = null;
        private SyncronizationManager syncronizationManager = null;
        private SizeTranslationHandler sizeTranslationHandler = null;
        private CatalogsLoader catalogsLoader = null;


        public InvoiceLoadedDocumentHandler(Invoice invoice, SystemInvoiceDBCache dbCache, SyncronizationManager syncronizationManager)
            {
            this.syncronizationManager = syncronizationManager;
            this.invoice = invoice;
            catalogsSearchHandler = new CatalogsSearchHandler(dbCache);
            approvalsSearcher = new ApprovalsSearcher(dbCache, invoice);
            groupingHandler = new GroupingHandler(() => !invoice.ExcelLoadingFormat.SaveOriginalRowsSet);
            grafCalculationHandler = new GrafCalcHandler(dbCache, invoice);
            groupOfGoodsCreationHandler = new GroupsOfGoodsCreationHandler(dbCache);
            bnsCreateHandler = new InvoiceNumberBNSHandler();
            nomenclatureRemovingHistoryUpdater = new NomenclatureRemovingHistoryUpdater(dbCache, invoice);
            this.invoiceAfterAprovalsUpdater = new InvoiceAfterAprovalsUpdater(this.invoice, dbCache);
            this.netWeightUpdater = new NetWeightUpdater(dbCache);
            this.namesTranslationHandler = new NamesTranslationHandler(invoice, dbCache);
            this.unitOfMeasureCodeRetreiveHandler = new UnitOfMeasureCodeRetreiveHandler(invoice, dbCache);
            this.sizeTranslationHandler = new SizeTranslationHandler(dbCache);
            this.catalogsLoader = new CatalogsLoader(dbCache, invoice);
            }

        /// <summary>
        /// Обновляет графу 31 и группирует строки
        /// </summary>
        /// <param name="table"></param>
        public void RefreshGroupingAndGrafHeaders(DataTable table)
            {
            groupingHandler.MakeGrouping(table);
            grafCalculationHandler.FillGrafCells(table);
            }

        /// <summary>
        /// Выполняет по новой обработку некоторых колонок (включая графу 31), таких как состав (перевод), наименования (перевод).
        /// </summary>
        public void RefreshTableState()
            {
            this.catalogsLoader.TryCreateNewCatalogsItems(this.invoice.Goods);
            this.sizeTranslationHandler.MakeTranslation(this.invoice.Goods);
            this.RefreshGroupingAndGrafHeaders(this.invoice.Goods);
            this.namesTranslationHandler.SetNamesTranslation(this.invoice.Goods);
            this.unitOfMeasureCodeRetreiveHandler.SetUnitOfMeasures(this.invoice.Goods);
            catalogsSearchHandler.FindCatalogs(this.invoice.Goods);
            approvalsSearcher.FindApprovals(this.invoice.Goods);
            }

        /// <summary>
        /// Обрабатывает загруженную из экселя табличную часть документа
        /// </summary>
        public bool TryHandleLoadedDocument(bool refreshOnly = false)
            {
            DataTable table = this.invoice.Goods;
            return tryHandleLoadedDocument(table, refreshOnly);
            }


        /// <summary>
        /// Обрабатывает загруженную из экселя табличную часть документа
        /// </summary>
        public bool TryHandleLoadedTable(DataTable table, bool refreshOnly = false)
            {
            if (tryHandleLoadedDocument(table, refreshOnly))
                {
                DataTable invoiceTable = invoice.Goods;
                invoiceTable.Clear();
                foreach (DataRow row in table.Rows)
                    {
                    DataRow invoiceRow = invoiceTable.NewRow();
                    for (int i = 0; i < invoiceTable.Columns.Count; i++)
                        {
                        invoiceRow[i] = row[i];
                        }
                    invoiceTable.Rows.Add(invoiceRow);
                    }
                DateTime from = DateTime.Now;
                syncronizationManager.RefreshAll();
                Console.WriteLine("refresh syncronizedFields: {0}", (DateTime.Now - from).TotalMilliseconds);
                return true;
                }
            return false;
            }


        /// <summary>
        /// Обрабатывает загруженную из экселя табличную часть документа
        /// </summary>
        private bool tryHandleLoadedDocument(DataTable table, bool refreshOnly = false)
            {
            try
                {
                DateTime from = DateTime.Now;
                if (refreshOnly)
                    {
                    bnsCreateHandler.CreateInvoiceNumbersIfNeed(table);
                    }
                Console.WriteLine("bns: {0}", (DateTime.Now - from).TotalMilliseconds);
                catalogsSearchHandler.FindCatalogs(table);
                Console.WriteLine("catalogs: {0}", (DateTime.Now - from).TotalMilliseconds);
                approvalsSearcher.FindApprovals(table);
                Console.WriteLine("approvals: {0}", (DateTime.Now - from).TotalMilliseconds);
                groupingHandler.MakeGrouping(table);
                Console.WriteLine("grouping: {0}", (DateTime.Now - from).TotalMilliseconds);
                grafCalculationHandler.FillGrafCells(table);
                Console.WriteLine("grafCalc: {0}", (DateTime.Now - from).TotalMilliseconds);
                nomenclatureRemovingHistoryUpdater.RefreshRequiredNomenclatureCache();
                if (!refreshOnly)
                    {
                    groupOfGoodsCreationHandler.CreateGroupsIfNeed(table);
                    }
                }
            catch (SystemInvoice.DataProcessing.Cache.TradeMarksCache.TradeMarkCacheObjectsStore.TradeMarkConflictException conflictExceprion)
                {
                "Неправильный формат загрузки. Торговая марка не может быть в загружаемом файле, если она указана в самом формате.".AlertBox();
                return false;
                }
            return true;
            }

        /// <summary>
        /// Обновляет списки разрешительных в документах
        /// </summary>
        public void RefreshApprovals()
            {
            invoiceAfterAprovalsUpdater.RefreshApprovals();
            }

        /// <summary>
        /// Обновляет списки разрешительных, после автоматического удаления номенклатуры из разрешительных при изменении там. кода
        /// </summary>
        /// <param name="updates">Изменения с информацией об удаленных разрешительных</param>
        public void RefreshApprovals(IEnumerable<ApprovalsUpdateResult> updates)
            {
            invoiceAfterAprovalsUpdater.RefreshApprovals(updates);
            }

        /// <summary>
        /// Обновляет внутреннюю привязку некоторых ячеек в строках к справочникам в системе (номенклатура,...)
        /// </summary>
        public void RefreshCatalogs()
            {
            this.catalogsSearchHandler.FindCatalogs(this.invoice.Goods);
            }

        /// <summary>
        /// Обновляет кэш в котором хранится инфа об истории удаленных из РД номенклатур
        /// </summary>
        public void RefreshRequiredNomenclatureCache()
            {
            this.nomenclatureRemovingHistoryUpdater.RefreshRequiredNomenclatureCache();
            }

        /// <summary>
        /// Проверяет вес нетто для всех строк и приводит его в соответствие с диапазоном веса нетто в номенклатуре
        /// </summary>
        public void MakeNetWeightReplacement()
            {
            this.netWeightUpdater.MakeNetWeightReplacement(this.invoice.Goods);
            }
        }
    }
