using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using SystemInvoice.DataProcessing.InvoiceProcessing.Filtering;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Filtering
    {
    /// <summary>
    /// Осуществляет фильтрацию по определенным значениям в колонках табличной части инвойса
    /// </summary>
    public class FilterManager
        {
        private GridView gridView = null;
        public FilterManager(GridView gridView)
            {
            this.gridView = gridView;
            }
        /// <summary>
        /// Редактирует фильтры по значениям для колонки таблицы
        /// </summary>
        /// <param name="focusedColumn">Колонка</param>
        /// <param name="position">Расположение</param>
        public void EditFilters(GridColumn focusedColumn, Point position)
            {
            if (focusedColumn == null || gridView == null || gridView.DataSource == null)
                {
                return;
                }
            DataTable sourceTable = (gridView.DataSource as DataView).Table;
            HashSet<string> allValues = getUniqueValues(sourceTable, focusedColumn.FieldName);
            HashSet<string> filteredValues = getFilteredValues(focusedColumn);
            FilterInfoModel filterInfoModel = new FilterInfoModel();
            if (filteredValues.Count == 0)
                {
                filterInfoModel.FilterAll = true;
                }
            filterInfoModel.AllItems = allValues;
            filterInfoModel.FilteredItems = filteredValues;
            var filterInfoModelNew = FilterDialogWindow.ShowDialog(filterInfoModel, position);//FilterDialogForm.ShowDialog(filterInfoModel, position); //
            if (filterInfoModel != filterInfoModelNew)
                {
                this.setFilters(filterInfoModelNew, focusedColumn);
                }
            }

        private void setFilters(FilterInfoModel filterInfoModel, GridColumn focusedColumn)
            {
            gridView.ActiveFilter.Remove(focusedColumn);
            if (filterInfoModel.FilterAll)
                {
                focusedColumn.AppearanceHeader.Font = new Font(focusedColumn.AppearanceHeader.Font, FontStyle.Regular);
                return;
                }
            if (filterInfoModel.FilteredItems.Count == 0)
                {
                return;
                }
            string filterStr = string.Format("[{0}] = '{1}'", focusedColumn.FieldName, filterInfoModel.FilteredItems.First());
            StringBuilder builder = new StringBuilder();
            foreach (string filteredItem in filterInfoModel.FilteredItems.Skip(1))
                {
                builder.Append(string.Format(" OR [{0}] = '{1}'", focusedColumn.FieldName, filteredItem));
                }
            filterStr = filterStr + builder.ToString();
            gridView.ActiveFilter.Add(focusedColumn,
                                      new ColumnFilterInfo(filterStr));
            if (!string.IsNullOrEmpty(filterStr))
                {
                focusedColumn.AppearanceHeader.Font = new Font(focusedColumn.AppearanceHeader.Font, FontStyle.Bold);
                }
            }
        /// <summary>
        /// Возвращает список значений по которым отфильтрована колонка 
        /// </summary>
        /// <param name="focusedColumn">колонка содержащая фильтруемые значения</param>
        private HashSet<string> getFilteredValues(GridColumn focusedColumn)
            {
            if (focusedColumn == null)
                {
                return null;
                }
            string columnFocused = focusedColumn.FieldName;
            HashSet<string> filteredValues = new HashSet<string>();
            string filterStr = gridView.ActiveFilterString;
            string[] parts = filterStr.Split(new string[] { "And", "Or", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
                {
                string clearPart = part;
                if (clearPart.StartsWith(@""""))
                    {
                    clearPart = clearPart.Substring(1);
                    }
                if (clearPart.EndsWith(@""""))
                    {
                    clearPart = clearPart.Substring(0, clearPart.Length - 1);
                    }
                clearPart = clearPart.Trim();
                if (string.IsNullOrEmpty(clearPart))
                    {
                    continue;
                    }
                string[] subParts = clearPart.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (subParts.Length != 2)
                    {
                    continue;
                    }
                string[] columnNameSplitted = subParts[0].Trim().Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                string[] valueSplitted = subParts[1].Trim().Split(new string[] { "'" }, StringSplitOptions.RemoveEmptyEntries);
                if (columnNameSplitted.Length != 1 || valueSplitted.Length != 1)
                    {
                    continue;
                    }
                string columnName = columnNameSplitted[0];
                string value = valueSplitted[0];
                if (columnFocused.Equals(columnName))
                    {
                    filteredValues.Add(value);
                    }
                }
            return filteredValues;
            }

        /// <summary>
        /// Возвращает уникальные значения для колонки таблицы
        /// </summary>
        private HashSet<string> getUniqueValues(DataTable dataTable, string columnName)
            {
            if (!dataTable.Columns.Contains(columnName))
                {
                return null;
                }
            HashSet<string> values = new HashSet<string>();
            foreach (DataRow row in dataTable.Rows)
                {
                string value = row[columnName].ToString();
                values.Add(value);
                }
            return values;
            }
        }
    }
