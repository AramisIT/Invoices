using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking;
using SystemInvoice.DataProcessing.InvoiceProcessing.UIInteraction;
using SystemInvoice.Documents;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {

    public interface IEditableRowsSource
        {
        IList<DataRow> DisplayingRows { get; }
        }
    /// <summary>
    /// Обрабатывает события пользовательскоги интерфейса Грида, также выполняет отображения и исправлени ошибок (через контекстное меню).
    /// </summary>
    public class GridViewManager : IEditableRowsSource
        {
        public bool IsByFocusChanged = false;
        public bool canChange = true;
        private bool isDocumentLoaded = false;
        private int lastFocusedRow = -1;
        private string lastFocusedColumn = "";
        private ColorConverter colorConverter = new ColorConverter();
        private Invoice Invoice = null;
        private GridView mainView = null;
        private InvoiceChecker invoiceChecker = null;
        private SystemInvoiceDBCache cachedData = null;
        private FilteredRowsSource filteredRowsSource = null;
        private GridViewEventManager eventManager = null;
        private ResolveErrorsContextMenuManager resolveErrorsContextMenuManager = null;
        private CustomsCodeUpdater customsCodeUpdater = null;
        private TotalsManager totalsManager = null;
        private GridViewColumnsVisualStateManager columnsVisualStateManager = null;

        public GridViewManager(Invoice invoice, GridView mainView, SystemInvoiceDBCache cachedData, InvoiceChecker invoiceChecker)
            {
            this.invoiceChecker = invoiceChecker;
            this.columnsVisualStateManager = new GridViewColumnsVisualStateManager(mainView);
            columnsVisualStateManager.RefreshColumns(invoice.ExcelLoadingFormat);

            eventManager = new GridViewEventManager(mainView);
            eventManager.OnGridViewTabPressed += eventManager_OnGridViewTabPressed;
            eventManager.OnSelectedCellNotByTabChanged += eventManager_OnSelectedCellNotByTabChanged;

            this.cachedData = cachedData;

            this.mainView = mainView;
            this.mainView.RowCellStyle += mainView_RowCellStyle;
            this.mainView.EndGrouping += mainView_EndGrouping;
            this.mainView.EndSorting += mainView_EndSorting;
            this.mainView.ColumnFilterChanged += mainView_ColumnFilterChanged;
            this.mainView.MouseDown += mainView_MouseDown;

            this.Invoice = invoice;
            this.invoiceChecker.ErrorsCountChanged += invoiceChecker_ErrorsCountChanged;
            this.invoiceChecker.CheckTable(false);

            this.filteredRowsSource = new FilteredRowsSource(invoice, mainView); //filteredRowsSource;
            this.totalsManager = new TotalsManager(invoice, mainView, this);
            this.customsCodeUpdater = new CustomsCodeUpdater(cachedData, invoice, mainView, filteredRowsSource);

            this.resolveErrorsContextMenuManager = new ResolveErrorsContextMenuManager(mainView, invoiceChecker, filteredRowsSource, showItemFormForCurrentCell);
            this.resolveErrorsContextMenuManager.OnErrorResolved += resolveErrorsContextMenuManager_OnErrorResolved;

            this.mainView.CellValueChanging += mainView_CellValueChanging;
           }

        /// <summary>
        /// Обновляет итоговые ячейки после каждого изменения значений в колонках по которым рассчитываются итоги
        /// </summary>
        void mainView_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
            {
            string columnName = e.Column.FieldName;
            if (columnName.Equals(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME) ||
              columnName.Equals(ProcessingConsts.ColumnNames.SUM_COLUMN_NAME) ||
              columnName.Equals(ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME) ||
              columnName.Equals(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME) ||
              columnName.Equals(ProcessingConsts.ColumnNames.NUMBER_OF_PLACES_COLUMN_NAME) ||
              columnName.Equals(ProcessingConsts.ColumnNames.GROSS_WEIGHT_COLUMN_NAME) ||
                columnName.Equals(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME))
                {
                DataRow targetRow = Invoice.Goods.Rows[this.filteredRowsSource.getSourceRow(e.RowHandle)];
                if (Invoice.Contractor.SynchronizeQuantityAndPlacesQuantity
                   && columnName.Equals(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME))
                    {
                    targetRow[ProcessingConsts.ColumnNames.NUMBER_OF_PLACES_COLUMN_NAME] = e.Value.ToInt64(true).ToString();
                    }
                targetRow[columnName] = e.Value;
                this.RefreshTotals();
                this.mainView.UpdateTotalSummary();
                }
            }

        /// <summary>
        /// Включает/выключает фильтр по новым елементам номенклатуры/строкам с ошибками
        /// </summary>
        public void SwitchErrorsOrNewItemsFilter()
            {
            this.filteredRowsSource.SwitchNewRowsFilterState();
            }

        /// <summary>
        /// Возвращает список строк данных которые соостветствуют отфильтрованным строкам отображаемым в гриде
        /// </summary>
        public IList<DataRow> DisplayingRows
            {
            get { return filteredRowsSource.DisplayingRows; }
            }

        #region Открытие таможенного кода/номенклатуры

        /// <summary>
        /// Открывает номенклатуру или таможенный код (если клик на таможенном коде)
        /// </summary>
        void mainView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
            {
            if (e.Button == MouseButtons.Right)
                {
                GridHitInfo hitInfo = mainView.CalcHitInfo(e.X, e.Y);
                if (hitInfo.RowHandle >= 0)
                    {
                    mainView.FocusedColumn = hitInfo.Column;
                    mainView.FocusedRowHandle = hitInfo.RowHandle;
                    if (mainView.GridControl.ContextMenu == null)
                        {
                        showItemFormForCurrentCell();
                        }
                    }
                }
            }

        private void showItemFormForCurrentCell()
            {
            if (mainView.FocusedColumn != null &&
                mainView.FocusedColumn.FieldName.Equals("CustomsCodeIntern"))
                {
                customsCodeUpdater.UpdateCurrentRowCustomsCode();
                TransactionManager.TransactionManagerInstance.BeginBusinessTransaction();
                this.cachedData.NomenclatureCacheObjectsStore.Refresh();
                TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                this.RefreshChecking();
                //this.notifyTableChanged();
                return;
                }
            openSelectedNomenclature();
            }


        /// <summary>
        /// Открывает элемент справочника номенклатуры для текущей строки, если таковой существует
        /// </summary>
        private void openSelectedNomenclature()
            {
            if (mainView.FocusedRowHandle < 0)
                {
                return;
                }
            int rowSelected = filteredRowsSource.getSourceRow(mainView.FocusedRowHandle);
            if (rowSelected >= this.Invoice.Goods.Rows.Count || rowSelected < 0)
                {
                //return;
                }
            long nomenclatureId = this.Invoice.Goods.Rows[rowSelected].TrySafeGetColumnValue<long>("FoundedNomenclature", 0);
            if (nomenclatureId > 0)
                {
                Nomenclature nomenclature = new Nomenclature();
                nomenclature.Id = nomenclatureId;
                nomenclature.Read();
                Aramis.UI.UserInterface.Current.ShowItem(nomenclature);
                }
            }
        #endregion

        void resolveErrorsContextMenuManager_OnErrorResolved()
            {
            this.refreshErorsRowsState();
            }

        void eventManager_OnSelectedCellNotByTabChanged()
            {
            onCellSelect();
            }

        void eventManager_OnGridViewTabPressed()
            {
            SelectNextErrorCell();
            }


        public InvoiceChecker Checker
            {
            get { return this.invoiceChecker; }
            }


        public double SummTotalCurrent
            {
            get
                {
                return totalsManager.TotalsState.VisibleTotalPrice;
                }
            }


        /// <summary>
        /// Указывает системе обработки ошибок что документ который в данный момент редактируется является непосредственно загруженным из файла, а
        /// не загруженным из бызы
        /// </summary>
        public void SetLoaded()
            {
            isDocumentLoaded = true;
            resolveErrorsContextMenuManager.SetIsLoaded();
            }

        #region Управление ошибками

        /// <summary>
        /// Проверяет всю табличную часть на наличие ошибок и подкрашивает соответствующим цветом ячейки
        /// </summary>
        public void RefreshChecking()
            {
            this.invoiceChecker.CheckTable(isDocumentLoaded);
            }

        /// <summary>
        /// Обновляет значение в колонке указывающее на наличие ошибки в строке, или на то что строка является строкой с новым элементом номенклатуры
        /// </summary>
        public void refreshErorsRowsState()
            {
            for (int i = 0; i < Invoice.Goods.Rows.Count; i++)
                {
                DataRow row = Invoice.Goods.Rows[i];
                bool isRowValid = invoiceChecker.IsRowValid(i);
                int currentValue = row.TrySafeGetColumnValue<int>("ShowRow", -1);
                int haveToSetCellValue = isRowValid ? 1 : 0;
                if (haveToSetCellValue != currentValue)
                    {
                    row[Invoice.ShowRow] = haveToSetCellValue;
                    }
                }
            filteredRowsSource.refreshState();
            this.totalsManager.RefreshTotals();
            }
        #region Обновление отображения ошибок при изменении порядка строк в таблице (необходимо обновлить сопоставление отображаемых строк к строкам с данными).

        void mainView_EndSorting(object sender, EventArgs e)
            {
            this.invoiceChecker.CheckTable(isDocumentLoaded);
            this.refreshErorsRowsState();
            }

        void mainView_EndGrouping(object sender, EventArgs e)
            {
            this.invoiceChecker.CheckTable(isDocumentLoaded);
            this.refreshErorsRowsState();
            }

        void mainView_ColumnFilterChanged(object sender, EventArgs e)
            {
            this.RefreshChecking();
            this.refreshErorsRowsState();
            this.onCellSelect();
            }

        #endregion


        void invoiceChecker_ErrorsCountChanged()
            {
            setAllErors();
            }
        /// <summary>
        /// Устанавливает в ячейку значение из редактируемого поля и проверяет на наличие ошибок
        /// </summary>
        public void setSelectedCellValue()
            {
            string newValue = Invoice.SelectedCellValue;
            int rowIndex = filteredRowsSource.getSourceRow(mainView.FocusedRowHandle);
            string selectedColumn = mainView.FocusedColumn.FieldName;
            this.Invoice.Goods.Rows[rowIndex][selectedColumn] = newValue;
            invoiceChecker.CheckRow(rowIndex, this.isDocumentLoaded, selectedColumn);
            }

        /// <summary>
        /// меняет фокус на следующую ячейку с ошибкой
        /// </summary>
        public void SelectNextErrorCell()
            {
            if (invoiceChecker == null)
                {
                return;
                }
            if (canChange)
                {
                canChange = false;
                try
                    {
                    mainView.Focus();
                    List<string> currentColumnsOrder = this.getCurrentViewColumnsOrder();
                    CellErrorPosition errorPosition = invoiceChecker.GetNextError(currentColumnsOrder, lastFocusedRow, lastFocusedColumn); //currentError );
                    if (errorPosition != null)
                        {
                        mainView.FocusedRowHandle = filteredRowsSource.getDisplayRow(errorPosition.RowIndex);
                        mainView.FocusedColumn = mainView.Columns[errorPosition.ColumnName];
                        onCellSelect();
                        }
                    }
                finally
                    {
                    canChange = true;
                    }
                }
            }

        private List<string> getCurrentViewColumnsOrder()
            {
            List<string> columnsStr = new List<string>();
            List<GridColumn> columns = new List<GridColumn>();
            foreach (GridColumn column in mainView.Columns)
                {
                if (column.VisibleIndex == -1)
                    {
                    continue;
                    }
                columns.Add(column);
                }
            var list = columns.OrderBy(col => col.VisibleIndex).Select(col => col.FieldName).ToList();
            return list;
            }

        void setAllErors()
            {
            if (mainView.Columns.Count == 0)
                {
                return;
                }
            foreach (GridColumn column in mainView.Columns)
                {
                setColumnErrors(column.FieldName);
                }
            }

        void setColumnErrors(string columnName)
            {
            GridColumn gridColumn = mainView.Columns.ColumnByName(columnName);
            if (gridColumn == null)
                {
                return;
                }
            string newCaption = "";
            string currentCaption = gridColumn.Caption;
            string realCaption = currentCaption.Split(new string[] { ". (" }, StringSplitOptions.RemoveEmptyEntries)[0];
            int errorsCount = invoiceChecker.GetColumnErrors(columnName);
            if (errorsCount > 0)
                {
                newCaption = string.Format("{0}. ({1})", realCaption.Trim(), errorsCount);
                }
            else
                {
                newCaption = realCaption;
                }
            gridColumn.Caption = newCaption;
            }

        /// <summary>
        /// Проверяет ошибки для выбранной ячейки и формирует контекстное меню для их исправления,
        /// также связывает поле для редактирования значения ячеки с выбранной ячейкой
        /// </summary>
        public void onCellSelect()
            {
            if (invoiceChecker == null || mainView == null || Invoice == null)
                {
                return;
                }
            if (mainView.FocusedRowHandle < 0)
                {
                return;
                }
            invoiceChecker.CheckRow(lastFocusedRow, this.isDocumentLoaded, lastFocusedColumn);
            lastFocusedRow = filteredRowsSource.getSourceRow(mainView.FocusedRowHandle);
            if (mainView.FocusedColumn == null)
                {
                return;
                }
            lastFocusedColumn = mainView.FocusedColumn.FieldName;
            string customCaption = mainView.FocusedColumn.CustomizationCaption;
            DataRow row = Invoice.Goods.Rows[lastFocusedRow];
            if (row != null)
                {
                long nomenclatureID = (long)row[Invoice.FoundedNomenclature];
                if (nomenclatureID != 0)
                    {
                    var nomenclature = cachedData.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureID);
                    if (nomenclature != null)
                        {
                        Invoice.CurrentNomenclature = nomenclature.NameInvoice;
                        }
                    }
                else
                    {
                    Invoice.CurrentNomenclature = " ";
                    }
                string focusedColumnName = mainView.FocusedColumn.FieldName;
                if (Invoice.Goods.Columns.Contains(focusedColumnName))
                    {
                    string focusedValueStr = "";
                    object focusedValue = row[focusedColumnName];
                    if (focusedValue != null && focusedValue != DBNull.Value)
                        {
                        focusedValueStr = focusedValue.ToString();
                        }
                    else
                        {
                        focusedValue = " ";
                        }
                    IsByFocusChanged = true;
                    Invoice.SelectedCellValue = focusedValueStr;
                    IsByFocusChanged = false;
                    }
                }
            invoiceChecker.CheckRow(lastFocusedRow, this.isDocumentLoaded, lastFocusedColumn);
            var currentError = invoiceChecker.GetError(lastFocusedRow, lastFocusedColumn);
            string currentNotification = invoiceChecker.GetNotification(lastFocusedRow, lastFocusedColumn);
            this.resolveErrorsContextMenuManager.RefreshMenu(currentError, currentNotification,
                "CustomsCodeIntern".Equals(mainView.FocusedColumn.FieldName));
            refreshErorsRowsState();
            }

        void mainView_RowCellStyle(object sender, RowCellStyleEventArgs e)
            {
            if (invoiceChecker == null)
                {
                return;
                }
            int rowIndex = filteredRowsSource.getSourceRow(e.RowHandle);
            string columnName = e.Column.FieldName;
            string colorStr = invoiceChecker.GetCellCollor(rowIndex, columnName);
            if (string.IsNullOrEmpty(colorStr))
                {
                return;
                }
            object color = colorConverter.ConvertFromString(colorStr);
            if (color != null && color is Color)
                {
                e.Appearance.BackColor = (Color)color;
                }
            }

        #endregion

        /// <summary>
        /// Возвращает индекс строки в таблице с данными соответствующий отображаемой строке
        /// </summary>
        public int FocusedRowIndex
            {
            get { return this.filteredRowsSource.getSourceRow(mainView.FocusedRowHandle); }
            }

        /// <summary>
        /// Обновляет итоговые ячейки
        /// </summary>
        public void RefreshTotals()
            {
            this.totalsManager.RefreshTotals();
            }

        /// <summary>
        /// Обновляет видимость колонок таким образом, что бы отображались только колонки с ошибками
        /// </summary>
        public void SetColumnsVisibility(ExcelLoadingFormat excelLoadingFormat)
            {
            this.columnsVisualStateManager.SetColumnsVisibility(this.Invoice.ExcelLoadingFormat);
            }

        public string ShowPreviousPage()
            {
            if (mainView != null)
                {
                return this.columnsVisualStateManager.SelectPreviousPage();
                }
            return string.Empty;
            }

        public string ShowNextPage()
            {
            if (mainView != null)
                {
                return this.columnsVisualStateManager.SelectNextPage();
                }
            return string.Empty;
            }

        /// <summary>
        /// Удаляет текущую строку из таблицы - источника и грида
        /// </summary>
        //public void RemoveCurrentRow()
        //    {
        //    int focusedRow = this.mainView.FocusedRowHandle;
        //    if (focusedRow >= 0 && "Вы действительно хотите удалить текущую строку?".Ask())
        //        {

        //        Aramis.UI.WinFormsDevXpress.ItemFormTuner.RemoveSubtableRow(mainView.GetDataRow(mainView.FocusedRowHandle), this.Invoice);
        //        }
        //    }
        }
    }
