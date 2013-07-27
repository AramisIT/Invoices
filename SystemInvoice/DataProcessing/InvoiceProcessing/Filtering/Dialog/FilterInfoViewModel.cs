using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using SystemInvoice.MVVM;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog
    {
    /// <summary>
    /// Модель представления, которая описывает фильтры по колонкам.
    /// </summary>
    public class FilterInfoViewModel : ViewModelBase
        {
        private List<FilterInfoViewModelItem> itemsToSetFilter = null;
        private CollectionViewSource items = null;
        private bool isGlobalFilterChanging = false;

        public FilterInfoViewModel(FilterInfoModel filterInfo)
            {
            items = new CollectionViewSource();
            itemsToSetFilter = new List<FilterInfoViewModelItem>();
            foreach (string itemToFilter in filterInfo.AllItems)
                {
                bool isFiltered = filterInfo.FilteredItems.Contains(itemToFilter);
                var itemToFilterVM = new FilterInfoViewModelItem(itemToFilter, isFiltered);
                itemToFilterVM.PropertyChanged += itemToFilterVM_PropertyChanged;
                itemsToSetFilter.Add(itemToFilterVM);
                }
            this.FilterAll = filterInfo.FilterAll;
            items.Source = itemsToSetFilter;
            //сортируем по неотфильтрованым значениям а затем по алфавиту
            items.SortDescriptions.Add(new SortDescription("IsFiltered", ListSortDirection.Descending));
            items.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));
            }

        void itemToFilterVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
            if (e.PropertyName.Equals("IsFiltered"))
                {
                if (!isGlobalFilterChanging && !((FilterInfoViewModelItem)sender).IsFiltered)
                    {
                    isGlobalFilterChanging = true;
                    this.FilterAll = false;
                    isGlobalFilterChanging = false;
                    }
                }
            }

        public ICollectionView Items
            {
            get { return items.View; }
            }

        /// <summary>
        /// Выбор фильтрации для всех значений или сброс фильтров для всех значений
        /// </summary>
        private bool vm_FilterAll = false;
        public bool FilterAll
            {
            get
                {
                return vm_FilterAll;
                }
            set
                {
                if (vm_FilterAll != value)
                    {
                    vm_FilterAll = value;
                    if (!isGlobalFilterChanging)
                        {
                        isGlobalFilterChanging = true;
                        foreach (FilterInfoViewModelItem filterInfoViewModelItem in itemsToSetFilter)
                            {
                            filterInfoViewModelItem.IsFiltered = value;
                            }
                        isGlobalFilterChanging = false;
                        }
                    RaisePropertyChanged("FilterAll");
                    }
                }
            }

        /// <summary>
        /// Возвращает текущее состояние отфильтрованного набора значений
        /// </summary>
        public FilterInfoModel GetFilterModel()
            {
            if (itemsToSetFilter == null)
                {
                return null;
                }
            FilterInfoModel model = new FilterInfoModel();
            model.FilteredItems = new HashSet<string>();
            model.AllItems = new HashSet<string>();
            model.FilterAll = this.FilterAll;
            foreach (FilterInfoViewModelItem filterInfoViewModelItem in itemsToSetFilter)
                {
                if (filterInfoViewModelItem.IsFiltered)
                    {
                    model.FilteredItems.Add(filterInfoViewModelItem.Value);
                    }
                model.AllItems.Add(filterInfoViewModelItem.Value);
                }
            return model;
            }
        }
    }
