using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog
    {
    /// <summary>
    /// Окно отображающее текущие фильтры
    /// </summary>
    public class FilterDialogWindow : Window
        {
        private FilterInfoViewModel filterInfoViewModel = null;

        public FilterDialogWindow(FilterInfoModel currentFilterData, Point position)
            {
            filterInfoViewModel = new FilterInfoViewModel(currentFilterData);
            this.Content = new FilterDialog(filterInfoViewModel);
            this.Width = 250;
            this.Height = 300;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.WindowStyle = WindowStyle.None;
            this.Top = position.Y; //(screenHeight - Height) / 2;
            this.Left = position.X;// (screenWidth - Width) / 2;
            }

        public FilterInfoModel CurrentFilterInfo
            {
            get { return filterInfoViewModel == null ? null : filterInfoViewModel.GetFilterModel(); }
            }

        /// <summary>
        /// Отображает диалоговое окно для настройки фильтра в колонках
        /// </summary>
        /// <param name="filterInfo">Данные о текущем фильтре в колонке</param>
        /// <param name="position">Точка на екране, в которой должно отображатся окно</param>
        /// <returns>Данные после изменения фильтров в диалоговом окне </returns>
        public static FilterInfoModel ShowDialog(FilterInfoModel filterInfo, Point position)
            {
            FilterDialogWindow window = new FilterDialogWindow(filterInfo,position);
            bool? dialogResult = window.ShowDialog();
            if (dialogResult != null && dialogResult.Value)
                {
                return window.CurrentFilterInfo;
                }
            return filterInfo;
            }
        }
    }
