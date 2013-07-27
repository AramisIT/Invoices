using System.Collections.Generic;
using System.Data;
using SystemInvoice.Documents;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.UIInteraction
    {
    /// <summary>
    /// Управляет отображением всех строк/строк только с ошибками, обеспечивает доступ к строкам в таблице данных через указатель на строку в табличном контроле
    /// </summary>
    public class FilteredRowsSource
        {
        private bool useErrorsFiltering = false;
        private Dictionary<int, int> sourceToDisplayDict = new Dictionary<int, int>();
        private Dictionary<int, int> displayToSourceDict = new Dictionary<int, int>();
        private List<DataRow> visibleRows = new List<DataRow>();

        private Invoice Invoice = null;
        private GridView mainView = null;

        public FilteredRowsSource(Invoice invoice, GridView mainView)
            {
            this.mainView = mainView;
            this.Invoice = invoice;
            }

        public void SwitchNewRowsFilterState()
            {
            var filterColumn = mainView.Columns["ShowRow"];
            mainView.ActiveFilter.Remove(filterColumn);
            if (useErrorsFiltering = !useErrorsFiltering)
                {
                switchShowNewRowsFilterOn();
                }
            }
        
        /// <summary>
        /// Обновляет данные об отображаемых строках и соответствии отображаемых строк строкам с данными
        /// </summary>
        public void refreshState()
            {
            sourceToDisplayDict.Clear();
            displayToSourceDict.Clear();
            visibleRows.Clear();
            for (int displayRow = 0; displayRow < mainView.DataRowCount; displayRow++)
                {
                int sourceRow = mainView.GetDataSourceRowIndex(displayRow);
                sourceToDisplayDict.Add(sourceRow, displayRow);
                displayToSourceDict.Add(displayRow, sourceRow);
                visibleRows.Add(this.Invoice.Goods.Rows[sourceRow]);
                }
            }

        /// <summary>
        /// Возвращет список строк данных которые отображаются в таблице 
        /// Имеется в виду не отображаются при позиционировании скролом - а отображаются при установке различных фильтров - то есть все строки Грида при использовании фильтров
        /// </summary>
        public IList<DataRow> DisplayingRows
            {
            get
                {
                return visibleRows;
                }
            }

        /// <summary>
        /// Возвращает индекс отображаемой строки в дата гриде
        /// </summary>
        /// <param name="sourceRow">Индекс строки в таблице - источнике данных</param>
        public int getDisplayRow(int sourceRow)
            {
            int displayRow = -1;
            if (sourceToDisplayDict.TryGetValue(sourceRow, out displayRow))
                {
                return displayRow;
                }
            return sourceRow;
            }
        /// <summary>
        /// Возвращает индекс строки в источнике данных
        /// </summary>
        /// <param name="displayRow">Индекс отображаемой строки</param>
        public int getSourceRow(int displayRow)
            {
            int sourceRow = -1;
            if (displayToSourceDict.TryGetValue(displayRow, out sourceRow))
                {
                return sourceRow;
                }
            return displayRow;
            }

        private void switchShowNewRowsFilterOn()
            {
            GridColumn columnToFilter = null;
            try
                {
                columnToFilter = mainView.Columns["ShowRow"];
                }
            catch { }
            if (columnToFilter != null)
                {
                mainView.ActiveFilter.Add(columnToFilter, new ColumnFilterInfo(string.Format("[{0}] = '{1}'", "ShowRow", 0)));
                }
            }
        }
    }
