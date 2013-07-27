using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog
    {
    /// <summary>
    /// Хранит данные о настройках фильтров для колонки
    /// </summary>
    public class FilterInfoModel
        {
        /// <summary>
        /// Все значения в колонке таблицы
        /// </summary>
        public HashSet<string> AllItems { get; set; }
        /// <summary>
        /// Отфильтрованные значения в колонке таблицы
        /// </summary>
        public HashSet<string> FilteredItems { get; set; }
        /// <summary>
        /// Выбраны ли все значения
        /// </summary>
        public bool FilterAll { get; set; }
        }
    }
