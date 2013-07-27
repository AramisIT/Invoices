using System.Collections.Generic;
using System.Data;
using System.Linq;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.UIInteraction
    {
    /// <summary>
    /// Управляет отображением колонок в таблице (ширина, размещение)
    /// </summary>
    public class GridViewColumnsVisualStateManager
        {
        private GridView mainView = null;
        private GridControl GoodsControl = null;

        public GridViewColumnsVisualStateManager(GridView mainView)
            {
            this.mainView = mainView;
            this.GoodsControl = mainView.GridControl;
            }

        /// <summary>
        /// Устанавливает видимыми только те колонки, которые присутствуют в формате загрузки
        /// </summary>
        /// <param name="loadingFormat"></param>
        public void SetColumnsVisibility(ExcelLoadingFormat loadingFormat)
            {
            if (loadingFormat != null)
                {
                HashSet<string> visibleColumns = getVisibleColumns(loadingFormat);
                List<GridColumn> gridColumnsList = getGridColumnsList();
                setColumnsVisibleIfInSet(gridColumnsList, visibleColumns);
                }
            }

        /// <summary>
        /// Устанавливает видимость колонок
        /// </summary>
        /// <param name="gridColumnsList">Список всех колонок в таблице</param>
        /// <param name="visibleColumns">Список имен колонок которые нужно сделать видимыми</param>
        private void setColumnsVisibleIfInSet(List<GridColumn> gridColumnsList, HashSet<string> visibleColumns)
            {
            List<GridColumn> newColumns = new List<GridColumn>();
            foreach (GridColumn gridColumn in gridColumnsList.OrderBy(col => col.AbsoluteIndex))
                {
                gridColumn.VisibleIndex = gridColumn.AbsoluteIndex;
                if (gridColumn.VisibleIndex == -1)
                    {
                    newColumns.Add(gridColumn);
                    }
                }
            foreach (GridColumn column in newColumns)
                {
                column.VisibleIndex = column.AbsoluteIndex;
                column.Visible = visibleColumns.Contains(column.FieldName);
                }
            foreach (GridColumn column in gridColumnsList.Except(newColumns).OrderByDescending(col => col.VisibleIndex))
                {
                if (column.FieldName.Equals("LineNumber"))
                    {
                    continue;
                    }
                column.Visible = visibleColumns.Contains(column.FieldName);
                }
            }


        private List<GridColumn> getGridColumnsList()
            {
            List<GridColumn> gridColumnsList = new List<GridColumn>();
            foreach (GridColumn column in mainView.Columns)
                {
                gridColumnsList.Add(column);
                }
            return gridColumnsList;
            }

        private HashSet<string> getVisibleColumns(ExcelLoadingFormat loadingFormat)
            {
            HashSet<string> visibleColumns = new HashSet<string>();
            foreach (DataRow mappingRow in loadingFormat.ColumnsMappings.Rows)
                {
                object mapping = mappingRow[loadingFormat.ColumnName];
                if (mapping is int)
                    {
                    InvoiceColumnNames columnNameInv = (InvoiceColumnNames) ((int) mapping);
                    string columnName = columnNameInv.ToString();
                    if (columnName.Equals(ProcessingConsts.ColumnNames.SIZE_ORIGINAL_COLUMN_NAME))
                        {
                        continue;
                        }
                    visibleColumns.Add(columnName);
                    }
                }
            return visibleColumns;
            }

        /// <summary>
        /// Обновляет состояние таблицы - видимость колонок и их ширину
        /// </summary>
        public void RefreshColumns(ExcelLoadingFormat loadingFormat)
            {
            if (mainView != null)
                {
                SetColumnsVisibility(loadingFormat);
                mainView.OptionsView.ColumnAutoWidth = false;
                mainView.ScrollStyle = ScrollStyleFlags.LiveHorzScroll;
                mainView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
                //Выставляем ширину всех колонок 125 пикселей, в принципе она должна выставлятся сама на основании настроек пользователя, после того как он настроит и сохранит доку
                //мент с таблицей, но проблема в том, что при отображении/скрывании колонки - все эти настройки сбрасываются
                foreach (GridColumn column in mainView.Columns)
                    {
                    string fieldName = column.FieldName;
                    column.Width = this.getColumnWidth(fieldName);
                    }
                }
            }

        private int getColumnWidth(string fieldName)
            {
            if (fieldName.Equals(ProcessingConsts.ColumnNames.GRAF31_COLUMN_NAME))
                {
                return 350;
                }
            if (fieldName.Equals(ProcessingConsts.ColumnNames.CUSTOM_CODE_EXTERNAL_COLUMN_NAME) ||
                fieldName.Equals(ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME))
                {
                return 75;
                }
            if (fieldName.Equals(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME))
                {
                return 80;
                }
            if (fieldName.Equals(ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME))
                {
                return 100;
                }
            return 125;
            }


        private int getTotalPagesCount()
            {
            int totalWidth = 0;
            foreach (GridColumn column in mainView.Columns)
                {
                if (column.Visible)
                    {
                    totalWidth += column.Width;
                    }
                }
            int totalPagesCount = (int)(totalWidth / this.GoodsControl.Width) + ((totalWidth % GoodsControl.Width > 0) ? 1 : 0);
            return totalPagesCount;
            }

        private int getCurrentPageIndex()
            {
            double totalWidth = GoodsControl.Width;
            double scrollOffset = mainView.LeftCoord;
            int currentPageIndex = (int)(scrollOffset / totalWidth) + ((scrollOffset % totalWidth) > 0 ? 1 : 0) + 1;
            return currentPageIndex;
            }

        private string setPageIndex(int currentPageIndex)
            {
            double calculatedOffset = GoodsControl.Width * (currentPageIndex - 1);
            mainView.LeftCoord = (int)calculatedOffset;
            return string.Format("страница {0} из {1}", currentPageIndex, getTotalPagesCount());
            }
        /// <summary>
        /// Отображает следующую "страницу" по горизонтали
        /// </summary>
        public string SelectPreviousPage()
            {
            int currentPageIndex = getCurrentPageIndex();
            if (currentPageIndex > 1)
                {
                currentPageIndex--;
                }
            return this.setPageIndex(currentPageIndex);
            }

        /// <summary>
        /// Отображает предыдущую "страницу" по горизонтали
        /// </summary>
        public string SelectNextPage()
            {
            int currentPageIndex = getCurrentPageIndex();
            int totalPagesCount = getTotalPagesCount();
            if (currentPageIndex < totalPagesCount)
                {
                currentPageIndex++;
                }
            return this.setPageIndex(currentPageIndex);
            }
        }
    }
