using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog
    {
    public partial class FilterDialogForm : Form
        {
        private bool canChange = true;
        private FilterInfoViewModel filterInfoViewModel = null;
        private Dictionary<FilterInfoViewModelItem, CheckedListBoxItem> filteredDict = new Dictionary<FilterInfoViewModelItem, CheckedListBoxItem>();
        private Dictionary<CheckedListBoxItem, FilterInfoViewModelItem> reversedDict = new Dictionary<CheckedListBoxItem, FilterInfoViewModelItem>();
        public FilterDialogForm(FilterInfoModel filterInfoModel, Point position)
            {
            this.filterInfoViewModel = new FilterInfoViewModel(filterInfoModel);
            this.Load += FilterDialogForm_Load;
            this.Top = position.Y; //(screenHeight - Height) / 2;
            this.Left = position.X;// (screenWidth - Width) / 2;
            InitializeComponent();
            }

        public FilterInfoModel CurrentFilterInfo
            {
            get { return filterInfoViewModel == null ? null : filterInfoViewModel.GetFilterModel(); }
            }

        public static FilterInfoModel ShowDialog(FilterInfoModel filterInfo, Point position)
            {
            FilterDialogForm form = new FilterDialogForm(filterInfo, position);
            if (form.ShowDialog() == DialogResult.OK)
                {
                return form.CurrentFilterInfo;
                }
            return filterInfo;
            }


        void FilterDialogForm_Load(object sender, EventArgs e)
            {
            this.checkAll.CheckStateChanged += checkAll_CheckStateChanged;
            initState();
            }

        void checkAll_CheckStateChanged(object sender, EventArgs e)
            {

            }

        private void okButton_Click(object sender, EventArgs e)
            {

            }

        private void cancelButton_Click(object sender, EventArgs e)
            {

            }

        private void initState()
            {
            canChange = false;
            if (this.filterInfoViewModel.FilterAll)
                {
                this.checkAll.CheckState = CheckState.Checked;
                }
            else
                {
                this.checkAll.CheckState = CheckState.Unchecked;
                }
            foreach (FilterInfoViewModelItem filterInfoViewModelItem in this.filterInfoViewModel.Items)
                {
                CheckedListBoxItem newItem = new CheckedListBoxItem();
                newItem.CheckState = filterInfoViewModelItem.IsFiltered ? CheckState.Checked : CheckState.Unchecked;
                newItem.Description = filterInfoViewModelItem.Value;
                this.checkedListBox.Items.Add(newItem);
                filteredDict.Add(filterInfoViewModelItem, newItem);
                reversedDict.Add(newItem, filterInfoViewModelItem);
                }
            canChange = true;
            }

        private void RefreshState()
            {

            }

        }
    }
