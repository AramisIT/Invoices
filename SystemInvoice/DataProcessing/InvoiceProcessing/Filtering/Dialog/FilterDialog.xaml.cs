using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Filtering.Dialog
    {
    /// <summary>
    /// Interaction logic for FilterDialog.xaml
    /// </summary>
    public partial class FilterDialog : UserControl
        {
        public FilterDialog(FilterInfoViewModel dataContext)
            {
            InitializeComponent();
            this.DataContext = dataContext;
            }

        private void Button_Click_1(object sender, RoutedEventArgs e)
            {
            Window.GetWindow(this).DialogResult = true;
            }

        private void Button_Click_2(object sender, RoutedEventArgs e)
            {
            Window.GetWindow(this).DialogResult = false;
            }
        }
    }
