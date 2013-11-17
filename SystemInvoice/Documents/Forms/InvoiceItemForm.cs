using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SystemInvoice.DataProcessing.InvoiceProcessing.Filtering;
using SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog;
using SystemInvoice.DataProcessing.InvoiceProcessing.Helpers;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.ApprovalsModification;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.CatalogsInTableSearch;
using SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.SpecificCachesManagement;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;
using Aramis.UI.WinFormsDevXpress;
using System.IO;
using System.Diagnostics;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using Aramis.DatabaseConnector;
using SystemInvoice.DataProcessing;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking;
using SystemInvoice.DataProcessing.ApprovalsProcessing.ByNomenclatureUpdating;
using SystemInvoice.DataProcessing.ApprovalsProcessing;
using DevExpress.XtraGrid;

namespace SystemInvoice.Documents.Forms
    {
    [View(DBObjectGuid = "0ECCE2A0-F801-411B-AE06-E6E488720327", ViewType = ViewFormType.DocItem, IsMDI = true)]
    public partial class InvoiceItemForm : DevExpress.XtraBars.Ribbon.RibbonForm, IItemForm
        {
        private FilesManager filesManager = null;
        private SystemInvoiceDBCache cachedData = null;
        private FilterManager filterManager = null;
        private static HashSet<string> invoiceColumnNamesSet = new HashSet<string>();
        private ItemsGroupEditor itemsGroupEditor = null;
        private GridView mainView = null;
        private string lastUnloadUnprocessedFileName = "";
        private GridViewManager gridViewManager = null;
        private InvoiceLoadedDocumentHandler loadedDocumentHandler = null;
        private SyncronizationManager syncronizationManager = null;
        private InvoiceChecker invoiceChecker = null;

        static InvoiceItemForm()
            {
            foreach (string key in Invoice.InvoiceColumnNames.Keys)
                {
                invoiceColumnNamesSet.Add(key);
                }
            }

        public InvoiceItemForm()
            {
            InitializeComponent();
            mainView = this.GoodsControl.MainView as GridView;
            mainView.KeyDown += mainView_KeyDown;
            mainView.FocusedColumnChanged += mainView_FocusedColumnChanged;
            mainView.FocusedRowChanged += mainView_FocusedRowChanged;
            goodsGridView.KeyDown += mainView_KeyDown;
            Load += InvoiceItemForm_Load;
            ApprovalsByNomenclatureUpdater.OnApprovalsUpdated += ApprovalsByNomenclatureUpdater_OnApprovalsUpdated;
            }

        void mainView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
            {
            this.checkFilterState();
            }

        void mainView_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
            {
            this.checkFilterState();
            }

        private void checkFilterState()
            {
            if (string.IsNullOrEmpty(this.mainView.ActiveFilterString) && !isFilterChanging)
                {
                this.barBtnFilterUnresolved.Checked = false;
                }
            }

        void mainView_KeyDown(object sender, KeyEventArgs e)
            {
            if (e.KeyCode == Keys.Delete)
                {
                e.Handled = true;
                }
            }

        void ApprovalsByNomenclatureUpdater_OnApprovalsUpdated(IEnumerable<ApprovalsUpdateResult> updates)
            {
            DataTable tableInvoice = this.Invoice.Goods;
            if (!this.IsDisposed)
                {
                syncronizationManager.DenySyncronization();
                loadedDocumentHandler.RefreshApprovals(updates);
                loadedDocumentHandler.RefreshRequiredNomenclatureCache();
                gridViewManager.RefreshChecking();
                syncronizationManager.AllowSyncronization();
                }
            }

        private void refreshInvoiceApprovalsCache()
            {
            syncronizationManager.DenySyncronization();
            loadedDocumentHandler.RefreshApprovals();
            syncronizationManager.AllowSyncronization();
            }

        void InvoiceItemForm_Load(object sender, EventArgs e)
            {
            this.GoodsControl.Paint += GoodsControl_Paint;
            }

        void GoodsControl_Paint(object sender, PaintEventArgs e)
            {
            if (mainView == null || this.Invoice == null)
                {
                return;
                }
            if (mainView.Columns.Count == this.Invoice.Goods.Columns.Count)
                {
                this.initializeAdditionalObjects();
                this.GoodsControl.Paint -= GoodsControl_Paint;
                this.GoodsControl.EmbeddedNavigator.ButtonClick += EmbeddedNavigator_ButtonClick;
                }
            }

        void EmbeddedNavigator_ButtonClick(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e)
            {
            if (e.Button.ButtonType == DevExpress.XtraEditors.NavigatorButtonType.Remove)
                {
                e.Handled = true;
                }
            }

        private void initializeAdditionalObjects()
            {
            DateTime from = DateTime.Now;
            cachedData = new SystemInvoiceDBCache(this.Invoice);
            this.syncronizationManager = new SyncronizationManager(this.Invoice);
            this.syncronizationManager.OnSyncronized += syncronizationManager_OnSyncronized;
            this.loadedDocumentHandler = new InvoiceLoadedDocumentHandler(this.Invoice, cachedData, syncronizationManager);
            if (this.Invoice.Id > 0)
                {
                cachedData.RefreshCache();
                loadedDocumentHandler.RefreshCatalogs();
                loadedDocumentHandler.RefreshRequiredNomenclatureCache();
                }
            this.invoiceChecker = new InvoiceChecker(Invoice, cachedData);
            this.invoiceChecker.ErrorsCountChanged += () => this.barBtnErrorsCountSource.Caption = invoiceChecker.ErrorsDescription;
            gridViewManager = new GridViewManager(Invoice, mainView, cachedData, this.invoiceChecker);
           
            this.filesManager = new FilesManager(this.Invoice, cachedData, gridViewManager.Checker, loadedDocumentHandler);
            this.itemsGroupEditor = new ItemsGroupEditor(gridViewManager, cachedData);
            filterManager = new FilterManager(mainView);
            this.refreshInvoiceApprovalsCache();
            this.syncronizationManager.RefreshAll();
            Invoice.PropertyChanged += Invoice_PropertyChanged;
            if (Invoice != null && Invoice.HaveToBeWritten)
                {
                notifyTableChanged();
                }
            Console.WriteLine("loaded on: {0}", (DateTime.Now - from).TotalMilliseconds);
            this.gridViewManager.SetColumnsVisibility(this.Invoice.ExcelLoadingFormat);

            SetGrafHeader.Checked = this.Invoice.SetGrafHeader;
            }

        void syncronizationManager_OnSyncronized(DataRow dataRow, string columnName)
            {
            this.checkTotals(columnName);
            int rowIndex = dataRow.Table.Rows.IndexOf(dataRow);
            if (this.invoiceChecker != null)
                {
                this.invoiceChecker.CheckRow(rowIndex, true, string.Empty);
                }
            }

        private DatabaseObject item = null;
        public DatabaseObject Item
            {
            get { return item; }
            set
                {
                item = value;
                }
            }


        #region Enable/Disable editing document

        private void enableEditing()
            {
            setEditingState(true);
            }

        private void disableEditing()
            {
            setEditingState(false);
            }

        private void setEditingState(bool isEnabled)
            {
            this.GoodsControl.Enabled = isEnabled;
            this.ExcelLoadingFormat.Enabled = isEnabled;
            this.Contractor.Enabled = isEnabled;
            this.TradeMark.Enabled = isEnabled;
            this.ExcelFilePathSource.Enabled = isEnabled;
            this.NetWeightCalc.Enabled = isEnabled;
            this.GrossWeight.Enabled = isEnabled;
            this.NumberOfPlaces.Enabled = isEnabled;
            this.Currency.Enabled = isEnabled;
            this.CurrentNomenclature.Enabled = isEnabled;
            this.SelectedCellValue.Enabled = isEnabled;
            this.barBtnNextPage.Enabled = isEnabled;
            this.barBtnPreviousPage.Enabled = isEnabled;
            this.barBtnFilterUnresolved.Enabled = isEnabled;
            this.barBtnNextUnresolved.Enabled = isEnabled;
            }
        #endregion

        public Invoice Invoice
            {
            get { return (Invoice)Item; }
            }


        void btnCancel_ItemClick(object sender, ItemClickEventArgs e)
            {
            Close();
            }

        private void btnWrite_ItemClick(object sender, ItemClickEventArgs e)
            {
            Item.Write();
            }

        private void btnOk_ItemClick(object sender, ItemClickEventArgs e)
            {
            WritingResult result = Item.Write();
            if (result == WritingResult.Success)
                {
                Close();
                }
            }

        void Invoice_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
            if (e.PropertyName.Equals("SelectedCellValue") && !this.gridViewManager.IsByFocusChanged)
                {
                this.gridViewManager.setSelectedCellValue();
                notifyTableChanged();
                }
            }

        private void checkTotals(string columnName)
            {
            if (columnName.Equals(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.SUM_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.NUMBER_OF_PLACES_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.GROSS_WEIGHT_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME))
                {
                if (gridViewManager != null)
                    {
                    this.gridViewManager.RefreshTotals();
                    }
                }
            }

        private void notifyTableChanged()
            {
            gridViewManager.refreshErorsRowsState();
            this.Invoice.NotifyTableRowChanged(this.Invoice.Goods, this.Invoice.Graf31, null);
            this.gridViewManager.RefreshTotals();
            this.mainView.UpdateTotalSummary();
            }

        #region Values Distribution
        private void NetWeightCalc_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.OK)
                {
                distributeNetWeightsIfCan();
                }
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                distributeNetWeightsAnyWay();
                }
            }

        private void NetWeightCalc_KeyPress(object sender, KeyPressEventArgs e)
            {
            if (e.KeyChar == '\r')
                {
                distributeNetWeightsIfCan();
                }
            }

        private void distributeNetWeightsIfCan()
            {
            for (int i = 0; i < 3; i++)
                {
                syncronizationManager.DenySyncronization();
                double totalWeightBefore = this.Invoice.NetWeightTotal;
                double toUpdateValue = Invoice.NetWeightCalc;
                itemsGroupEditor.SetNetWeightIfCan(toUpdateValue);
                syncronizationManager.AllowSyncronization();
                syncronizationManager.RefreshAll();
                gridViewManager.RefreshChecking();
                gridViewManager.RefreshTotals();
                notifyTableChanged();
                double totalWeightAfter = this.Invoice.NetWeightTotal;
                Invoice.NetWeightCalc = Math.Round((totalWeightBefore + toUpdateValue) - totalWeightAfter, 3);
                }
            }

        private void distributeNetWeightsAnyWay()
            {
            for (int i = 0; i < 3; i++)
                {
                syncronizationManager.DenySyncronization();
                double totalWeightBefore = this.Invoice.NetWeightTotal;
                double toUpdateValue = Invoice.NetWeightCalc;
                itemsGroupEditor.SetNetWeightAnyWay(Invoice.NetWeightCalc);
                syncronizationManager.AllowSyncronization();
                syncronizationManager.RefreshAll();
                gridViewManager.RefreshChecking();
                gridViewManager.RefreshTotals();

                notifyTableChanged();
                double totalWeightAfter = this.Invoice.NetWeightTotal;
                Invoice.NetWeightCalc = Math.Round((totalWeightBefore + toUpdateValue) - totalWeightAfter, 3);
                }
            }


        private void GrossWeight_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.OK)
                {
                distributeGrossWeightByNetWeights();
                }
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Close)
                {
                itemsGroupEditor.ClearGrossWeight();
                gridViewManager.RefreshChecking();
                notifyTableChanged();
                }
            }

        private void GrossWeight_KeyPress(object sender, KeyPressEventArgs e)
            {
            if (e.KeyChar == '\r')
                {
                distributeGrossWeightByNetWeights();
                }
            }

        private void distributeGrossWeightByNetWeights()
            {
            for (int i = 0; i < 3; i++)
                {
                syncronizationManager.DenySyncronization();
                double totalWeightBefore = this.Invoice.GrossWeightTotal;
                double toUpdateValue = Invoice.GrossWeight;
                itemsGroupEditor.SetGrossWeight(toUpdateValue);
                syncronizationManager.AllowSyncronization();
                gridViewManager.RefreshChecking();
                gridViewManager.RefreshTotals();
                notifyTableChanged();
                double totalWeightAfter = this.Invoice.GrossWeightTotal;
                Invoice.GrossWeight = Math.Round((totalWeightBefore + toUpdateValue) - totalWeightAfter, 3);
                }
            }

        private void NumberOfPlaces_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.OK)
                {
                distributeNumberOfPlaces();
                }
            }


        private void NumberOfPlaces_KeyPress(object sender, KeyPressEventArgs e)
            {
            if (e.KeyChar == '\r')
                {
                distributeNumberOfPlaces();
                }
            }

        private void distributeNumberOfPlaces()
            {
            itemsGroupEditor.SetPlaces(Invoice.NumberOfPlaces);
            notifyTableChanged();
            }

        private void Summ_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.OK)
                {
                distributeSumm();
                }
            }

        private void summArrangeBtn_KeyPress(object sender, KeyPressEventArgs e)
            {
            if (e.KeyChar == '\r')
                {
                distributeSumm();
                }
            }

        private void distributeSumm()
            {
            try
                {
                double beforeTotalPrice = 0, afterTotalPrice = 0;
                string editValueStr = summArrangeBtn.EditValue.ToString();
                double editSumm = 0;
                if (double.TryParse(editValueStr, out editSumm))
                    {
                    syncronizationManager.DenySyncronization();
                    //-----------------------------------------
                    foreach (DataRow dataRow in this.Invoice.Goods.Rows)
                        {
                        string priceStr = dataRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME, string.Empty);
                        string countStr = dataRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME, string.Empty);
                        int count = 0;
                        double priceValue = 0;
                        if (double.TryParse(priceStr, out priceValue) && int.TryParse(countStr, out count))
                            {
                            beforeTotalPrice += priceValue * count;
                            }
                        }
                    //----------------------------------------
                    double summToEdit = this.gridViewManager.SummTotalCurrent;//this.Invoice.SumTotal
                    itemsGroupEditor.SetSumm(editSumm - summToEdit);
                    //----------------------------------------
                    foreach (DataRow dataRow in this.Invoice.Goods.Rows)
                        {
                        string priceStr = dataRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME, string.Empty);
                        string countStr = dataRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME, string.Empty);
                        int count = 0;
                        double priceValue = 0;
                        if (double.TryParse(priceStr, out priceValue) && int.TryParse(countStr, out count))
                            {
                            afterTotalPrice += priceValue * count;
                            }
                        }


                    //----------------------------------------
                    syncronizationManager.AllowSyncronization();
                    syncronizationManager.RefreshAll();
                    gridViewManager.RefreshChecking();
                    gridViewManager.RefreshTotals();
                    //this.invoiceChecker.CheckTable(this.isDocumentLoaded);
                    notifyTableChanged();
                    }
                }
            catch
                {
                }
            }

        #endregion

        #region ItemsGroupsEditing
        private void ReplaceStr_KeyPress(object sender, KeyPressEventArgs e)
            {
            if (e.KeyChar == '\r')
                {
                string selectedColumn = mainView.FocusedColumn.FieldName;
                freezeSelectedRows();
                int rowIndex = this.gridViewManager.FocusedRowIndex;  //this.filteredRowsSource.getSourceRow(mainView.FocusedRowHandle);
                if (rowIndex < 0)
                    {
                    return;
                    }
                string selectedValue = this.Invoice.Goods.Rows[rowIndex][selectedColumn].ToString();
                itemsGroupEditor.SetCurrentReplacement(this.Invoice.ReplaceStr, selectedColumn);
                notifyTableChanged();
                ReplaceStr.Text = string.Empty;
                }
            }

        private void freezeSelectedRows()
            {
            List<DataRow> displayingRowsCache = this.gridViewManager.DisplayingRows.ToList();
            foreach (DataRow row in this.Invoice.Goods.Rows)
                {
                row[Invoice.FILTER_COLUMN_NAME] = 0;
                }
            foreach (DataRow row in displayingRowsCache)
                {
                row[Invoice.FILTER_COLUMN_NAME] = 1;
                }
            this.mainView.ActiveFilter.Clear();
            GridColumn columnToFilter = null;
            try
                {
                columnToFilter = mainView.Columns[Invoice.FILTER_COLUMN_NAME];
                }
            catch { }
            if (columnToFilter != null)
                {
                mainView.ActiveFilter.Add(columnToFilter, new ColumnFilterInfo(string.Format("[{0}] = '{1}'", Invoice.FILTER_COLUMN_NAME, 1)));
                }
            }
        #endregion

        #region Documents processing

        private void unloadProcessedBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            string appendix = "_обработан.xls";
            string loadPath = this.ExcelFilePathSource.Text;
            string savePath = getSubFileName(appendix, loadPath);
            if (!string.IsNullOrEmpty(savePath) && this.filesManager.TrySaveProcessedDocument(savePath))
                {
                Process.Start(savePath);
                }
            }

        private string getSubFileName(string appendix, string loadPath)
            {
            string savePath = "";
            if (string.IsNullOrEmpty(loadPath) || !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(loadPath)))
                {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Files (.xls)|*.xls";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                    savePath = sfd.FileName;
                    }
                else
                    {
                    return null;
                    }
                }
            else
                {
                savePath = string.Concat(loadPath.Replace(".xls", ""), appendix);
                if (File.Exists(savePath))
                    {
                    try
                        {
                        File.Delete(savePath);
                        }
                    catch (IOException ioException)
                        {
                        "Закройте файл, открытый для обработки новых позиций/ошибок".AlertBox();
                        savePath = string.Empty;
                        }
                    catch (Exception e)
                        {
                        savePath = string.Empty;
                        }
                    }
                }
            return savePath;
            }

        private void ExcelFilePath_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {
            switch (e.Button.Kind)
                {
                case (DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph): openProcessingDocument(); break;
                case (DevExpress.XtraEditors.Controls.ButtonPredefines.OK): loadDocumentToProcess(); break;
                case (DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis): selectPathToLoadUnprocessed(); break;
                default: break;
                }
            }

        private void selectPathToLoadUnprocessed()
            {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files (.xls)|*.xls";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                ExcelFilePathSource.Text = ofd.FileName;
                }
            }

        private void openProcessingDocument()
            {
            if (!string.IsNullOrEmpty(ExcelFilePathSource.Text) && File.Exists(ExcelFilePathSource.Text))
                {
                Process.Start(new ProcessStartInfo(ExcelFilePathSource.Text));
                }
            else
                {
                "Файл не найден".AlertBox();
                }
            }

        private void loadDocumentToProcess()
            {
            refreshExcelLoadFormat();
            this.gridViewManager.SetColumnsVisibility(this.Invoice.ExcelLoadingFormat);
            string filePath = ExcelFilePathSource.Text;
            lastUnloadUnprocessedFileName = getUnprocessedUnloadFileName(filePath);
            syncronizationManager.DenySyncronization();
            if (this.filesManager.TryProcessNewDocument(filePath, lastUnloadUnprocessedFileName))
                {
                gridViewManager.SetLoaded();
                //    isDocumentLoaded = true;
                disableEditing();
                }
            else
                {
#if DEBUG
                gridViewManager.SetLoaded();
                //   isDocumentLoaded = true;
#endif
                lastUnloadUnprocessedFileName = null;
                syncronizationManager.AllowSyncronization();
                this.gridViewManager.RefreshChecking();
                //   invoiceChecker.CheckTable(isDocumentLoaded);
                }
            syncronizationManager.AllowSyncronization();
            notifyTableChanged();
            setLoadingInfo(this.filesManager.ToEditUnloadedCount);
            this.mainView.UpdateTotalSummary();
            this.refreshInvoiceApprovalsCache();
            }


        private void newExcelItemsUnloadBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            refreshExcelLoadFormat();
            lastUnloadUnprocessedFileName = getUnprocessedUnloadFileName(this.ExcelFilePathSource.Text);
            if (string.IsNullOrEmpty(lastUnloadUnprocessedFileName))
                {
                return;
                }
            if (this.filesManager.TrySaveErrorsToEdit(lastUnloadUnprocessedFileName))
                {
                disableEditing();
                }
            setLoadingInfo(filesManager.ToEditUnloadedCount);
            }

        private void allExcelItemsUnloadBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            refreshExcelLoadFormat();
            lastUnloadUnprocessedFileName = getUnprocessedUnloadFileName(this.ExcelFilePathSource.Text);
            if (string.IsNullOrEmpty(lastUnloadUnprocessedFileName))
                {
                return;
                }
            if (this.filesManager.TrySaveAllToEdit(lastUnloadUnprocessedFileName))
                {
                disableEditing();
                }
            }

        private string getUnprocessedUnloadFileName(string targetFileName)
            {
            string appendix = "_необработанныйИновый.xls";
            string loadPath = targetFileName;
            return getSubFileName(appendix, loadPath);
            }

        private void setLoadingInfo(int countToFileUnloaded)
            {
            loadingInfo.Caption = "";
            int totalCount = this.Invoice.Goods.Rows.Count;
            if (totalCount <= 0)
                {
                return;
                }
            int loadedCount = this.gridViewManager.Checker.LoadedRowsCount;
            loadingInfo.Caption = string.Format("Строк с новыми позициями {0} ", totalCount - loadedCount);
           //  loadingInfo.Caption =string.Format("Загружено {0} из {1} строк. Выгружено в новый файл {2}.", loadedCount, totalCount, countToFileUnloaded);
            //
            }

        private void refreshExcelLoadFormat()
            {
            if (this.Invoice.ExcelLoadingFormat.Id != 0){
                ExcelLoadingFormat loadFormat = new ExcelLoadingFormat();
                loadFormat.Id = this.Invoice.ExcelLoadingFormat.Id;
                loadFormat.Read();
                this.Invoice.ExcelLoadingFormat = loadFormat;
                }
            }

        private void loadItemsToDocument_ItemClick(object sender, ItemClickEventArgs e)
            {
            refreshExcelLoadFormat();
            if (!string.IsNullOrEmpty(lastUnloadUnprocessedFileName))
                {
                loadEdited(lastUnloadUnprocessedFileName);
                }
            else
                {
                "Для этого документа нету выгруженного файла".AlertBox();
                }
            gridViewManager.RefreshChecking();
            this.refreshInvoiceApprovalsCache();
            lastUnloadUnprocessedFileName = string.Empty;
            }

        private void loadEdited(string editedFilePath)
            {
            syncronizationManager.DenySyncronization();
            this.filesManager.LoadProcessedFile(editedFilePath);
            enableEditing();
            setLoadingInfo(0);
            notifyTableChanged();
            syncronizationManager.AllowSyncronization();
            syncronizationManager.RefreshAll();
            this.refreshInvoiceApprovalsCache();
            }

        #endregion

        #region PagesNavigation
        private void barBtnPreviousPage_ItemClick(object sender, ItemClickEventArgs e)
            {
            this.barBtnPageIndex.Caption = this.gridViewManager.ShowPreviousPage();
            }

        private void barBtnNextPage_ItemClick(object sender, ItemClickEventArgs e)
            {
            this.barBtnPageIndex.Caption = this.gridViewManager.ShowNextPage();
            }

        #endregion

        #region ErrorsNavigation

        bool isFilterChanging = false;

        private void barBtnFilterUnresolved_ItemClick(object sender, ItemClickEventArgs e)
            {
            try
                {
                isFilterChanging = true;
                //    syncronizationManager.DenySyncronization();
                this.gridViewManager.SwitchErrorsOrNewItemsFilter();
                //    syncronizationManager.AllowSyncronization();
                }
            finally
                {
                isFilterChanging = false;
                }
            }

        private void barBtnNextUnresolved_ItemClick(object sender, ItemClickEventArgs e)
            {
            this.gridViewManager.SelectNextErrorCell();
            }
        #endregion

        #region ManageFilters

        private void manageFiltersBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            manageFilters();
            }

        private void manageFilters()
            {
            this.gridViewManager.canChange = false;
            try
                {
                var somePoint = this.PointToScreen(new Point(0, 0));
                var parent = this.MdiParent;
                int parentLocationX = parent == null ? 0 : parent.Location.X;
                int parentLocationY = parent == null ? 0 : parent.Location.Y;
                Point position = new Point(somePoint.X + 10, somePoint.Y + 252);
                filterManager.EditFilters(mainView.FocusedColumn, position);

                foreach (var itemLink in this.GoodsButtonsBar.ItemLinks)
                    {
                    BarButtonItemLink buttonLink = itemLink as BarButtonItemLink;
                    if (buttonLink != null)
                        {
                        var button = buttonLink.Item;
                        if (button != null && button.Alignment != BarItemLinkAlignment.Right)
                            {
                            button.Enabled = true;
                            }
                        }
                    }
                }
            finally
                {
                this.gridViewManager.canChange = true;
                }
            }
        #endregion


        private void refreshGroupingBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            syncronizationManager.DenySyncronization();
            cachedData.RefreshCache();
            this.loadedDocumentHandler.RefreshTableState();
            cachedData.RefreshCache();
            syncronizationManager.AllowSyncronization();
            gridViewManager.RefreshChecking();
            }

        private void clearAllFiltersBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            this.mainView.ActiveFilter.Clear();
            this.barBtnFilterUnresolved.Checked = false;
            foreach (GridColumn column in mainView.Columns)
                {
                column.AppearanceHeader.Font = new Font(column.AppearanceHeader.Font, FontStyle.Regular);
                }
            }

        private void checkNetWeightBarBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            this.loadedDocumentHandler.MakeNetWeightReplacement();
            gridViewManager.RefreshChecking();
            notifyTableChanged();
            }

        private void SetGrafHeader_CheckedChanged(object sender, ItemClickEventArgs e)
            {
            this.Invoice.SetGrafHeader = SetGrafHeader.Checked;
            }

        }
    }