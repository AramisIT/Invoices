using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Windows.Forms;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.UIInteraction
    {
    /// <summary>
    /// Управляет контекстным меню для исправления ошибок
    /// </summary>
    public class ResolveErrorsContextMenuManager
        {
        /// <summary>
        /// Вызывается при разрешении ошибки с помощью контекстного меню
        /// </summary>
        public event Action OnErrorResolved = null;

        private GridView mainView = null;
        private InvoiceChecker invoiceChecker = null;

        #region Графические компоненты
        ContextMenu showDownMenu = null;
        MenuItem descriptionCellValueItem = new MenuItem();
        MenuItem setCurrentCellValueItem = new MenuItem();
        MenuItem setDatabaseValueItem = new MenuItem();
        MenuItem setCurrentCellValueForAllSameItems = new MenuItem();
        MenuItem setDatabaseValueForAllSameItems = new MenuItem();
        MenuItem setCurrentCellValueForAllItems = new MenuItem();
        MenuItem setDataBaseValueForAllItems = new MenuItem();
        MenuItem delimeterItem = new MenuItem(); 
        #endregion

        private FilteredRowsSource filteredRowsSource = null;
        private CellError currentError = null;
        private bool isDocumentLoaded = false;

        public ResolveErrorsContextMenuManager(GridView mainView, InvoiceChecker invoiceChecker, FilteredRowsSource filteredRowsSource)
            {
            this.mainView = mainView;
            this.invoiceChecker = invoiceChecker;
            this.filteredRowsSource = filteredRowsSource;
            initContextMenu();
            }

        private void raiseErrorResolved()
            {
            if (OnErrorResolved != null)
                {
                OnErrorResolved();
                }
            }


        private void initContextMenu()
            {
            delimeterItem.Text = "-";
            descriptionCellValueItem.Enabled = false;
            showDownMenu = new ContextMenu(
                new[] 
                { 
                descriptionCellValueItem,
                setCurrentCellValueItem,
                setCurrentCellValueForAllSameItems,
                setDatabaseValueItem,
                setDatabaseValueForAllSameItems,
                delimeterItem,
                setCurrentCellValueForAllItems,
                setDataBaseValueForAllItems
                }
            );
            setCurrentCellValueItem.Click += setCurrentCellValueItem_Click;
            setCurrentCellValueForAllSameItems.Click += setCurrentCellValueForAllSameItems_Click;
            setDatabaseValueItem.Click += setDatabaseValueItem_Click;
            setDatabaseValueForAllSameItems.Click += setDatabaseValueForAllSameItems_Click;
            setCurrentCellValueForAllItems.Click += setCurrentCellValueForAllItems_Click;
            setDataBaseValueForAllItems.Click += setDataBaseValueForAllItems_Click;
            this.mainView.GridControl.ContextMenu = showDownMenu;
            }

        /// <summary>
        /// Обновляет контекстное меню для определенной ошибки
        /// </summary>
        /// <param name="error">Ошибка</param>
        /// <param name="currentNotification">Уведомление которое может отображатся вместо ошибки</param>
        public void RefreshMenu(CellError error, string currentNotification)
            {
            this.currentError = error;
            this.mainView.GridControl.ContextMenu = null;
            if (error == null && string.IsNullOrEmpty(currentNotification))
                {
                return;
                }
            CompareWithDBCellError compareError = error as CompareWithDBCellError;
            this.mainView.GridControl.ContextMenu = showDownMenu;
            if (compareError != null)
                {
                this.setMenuItemsText(compareError.InDocumentValue, compareError.InDBValue, compareError.CanCopyFromDBOnly, currentNotification, compareError.ErrorDescription);
                }
            else
                {
                this.setMenuItemsText(string.Empty, string.Empty, false, currentNotification, string.Empty);
                }
            }

        public void SetIsLoaded()
            {
            isDocumentLoaded = true;
            }

        void setDataBaseValueForAllItems_Click(object sender, EventArgs e)
            {
            this.invoiceChecker.CopyToColumnFromDB(currentError, filteredRowsSource.getSourceRow(mainView.FocusedRowHandle), isDocumentLoaded);
            raiseErrorResolved();
            }

        void setCurrentCellValueForAllItems_Click(object sender, EventArgs e)
            {
            this.invoiceChecker.CopyToDataBaseFromCurrentColumn(currentError, filteredRowsSource.getSourceRow(mainView.FocusedRowHandle), isDocumentLoaded);
            raiseErrorResolved();
            }

        void setDatabaseValueForAllSameItems_Click(object sender, EventArgs e)
            {
            this.invoiceChecker.CopyToSameCellsFromDB(currentError, filteredRowsSource.getSourceRow(mainView.FocusedRowHandle), isDocumentLoaded);
            raiseErrorResolved();
            }

        void setDatabaseValueItem_Click(object sender, EventArgs e)
            {
            this.invoiceChecker.CopyToCellFromDataBase(currentError, filteredRowsSource.getSourceRow(mainView.FocusedRowHandle), isDocumentLoaded);
            raiseErrorResolved();
            }

        void setCurrentCellValueForAllSameItems_Click(object sender, EventArgs e)
            {
            this.invoiceChecker.CopyToDataBaseFromSameCells(currentError, filteredRowsSource.getSourceRow(mainView.FocusedRowHandle), isDocumentLoaded);
            raiseErrorResolved();
            }

        void setCurrentCellValueItem_Click(object sender, EventArgs e)
            {
            this.invoiceChecker.CopyToDataBaseFromCurrentCell(currentError, filteredRowsSource.getSourceRow(mainView.FocusedRowHandle), isDocumentLoaded);
            raiseErrorResolved();
            }
        /// <summary>
        /// Устанавливает текст в элементах меню
        /// </summary>
        /// <param name="valueInCell">Текстовое значение в ячейке для которой устанавливается меню</param>
        /// <param name="valueInDatabase">Значение в базе которое устанавливается для ячейки</param>
        /// <param name="showCopyFromDBOnly">Убрать элементы контекстного меню для установки данных в базу</param>
        /// <param name="currentNotification">Уведомление которое може по умолчанию отображатся вместо текста для обычных ошибок для которых нельзя изменить значения в базе или ячейке</param>
        /// <param name="errorDescription">Текст шапки меню.</param>
        private void setMenuItemsText(string valueInCell, string valueInDatabase, bool showCopyFromDBOnly, string currentNotification, string errorDescription)
            {
            //либо берем описание ошибки, которое отображается в первой строки из параметра, либо если не задано - формируем сами
            string errorToShow = !string.IsNullOrEmpty(errorDescription) ? errorDescription : string.Format(@"""{0}"" а должно быть ""{1}""", valueInCell, valueInDatabase);
            setCurrentCellValueItem.Visible = false;
            setDatabaseValueItem.Visible = false;
            setCurrentCellValueForAllSameItems.Visible = false;
            setDatabaseValueForAllSameItems.Visible = false;
            setCurrentCellValueForAllItems.Visible = false;
            setDataBaseValueForAllItems.Visible = false;
            delimeterItem.Visible = false;
            if (string.IsNullOrEmpty(valueInCell) && string.IsNullOrEmpty(valueInDatabase))
                {
                string notification = string.IsNullOrEmpty(currentNotification)
                                          ? "Не найдено соответствующего элемента справочника."
                                          : currentNotification;
                descriptionCellValueItem.Text = notification;
                return;
                }
            if (!showCopyFromDBOnly)
                {
                setCurrentCellValueItem.Visible = true;
                setCurrentCellValueForAllSameItems.Visible = true;
                setCurrentCellValueForAllItems.Visible = true;
                }
            setDatabaseValueItem.Visible = true;
            setDatabaseValueForAllSameItems.Visible = true;
            setDataBaseValueForAllItems.Visible = true;
            delimeterItem.Visible = true;
            descriptionCellValueItem.Text = errorToShow;
            setCurrentCellValueItem.Text = string.Format(@"принять ""{0}"" для этой ячейки", valueInCell);
            setDatabaseValueItem.Text = string.Format(@"принять ""{0}"" для этой ячейки", valueInDatabase);
            setCurrentCellValueForAllSameItems.Text = string.Format(@"принять ""{0}"" для всех ячейеек у которых ""{0}""", valueInCell);
            setDatabaseValueForAllSameItems.Text = string.Format(@"принять ""{0}"" для всех ячейеек у которых ""{1}""", valueInDatabase, valueInCell);
            setCurrentCellValueForAllItems.Text = "принять по всем как есть";
            setDataBaseValueForAllItems.Text = "принять по всем как в базе";
            }
        }
    }
