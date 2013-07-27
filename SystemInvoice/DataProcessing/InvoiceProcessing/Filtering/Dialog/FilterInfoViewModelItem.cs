using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.MVVM;


namespace SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog
    {
    /// <summary>
    /// Модель представления описывающая фильтруемый елемент
    /// </summary>
    public class FilterInfoViewModelItem : ViewModelBase
        {
        public FilterInfoViewModelItem(string value, bool isFiltered)
            {
            this.vm_Value = value;
            this.vm_IsFiltered = isFiltered;
            }
        /// <summary>
        /// Значение
        /// </summary>
        private string vm_Value = string.Empty;
        public string Value
            {
            get
                {
                return vm_Value;
                }
            set
                {
                if (vm_Value != value)
                    {
                    vm_Value = value;
                    RaisePropertyChanged("Value");
                    }
                }
            }
        /// <summary>
        /// Является ли значение выбранным для фильтрации
        /// </summary>
        private bool vm_IsFiltered = false;
        public bool IsFiltered
            {
            get
                {
                return vm_IsFiltered;
                }
            set
                {
                if (vm_IsFiltered != value)
                    {
                    vm_IsFiltered = value;
                    RaisePropertyChanged("IsFiltered");
                    }
                }
            }

        }
    }
