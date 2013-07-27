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

namespace SystemInvoice.DataProcessing.CatalogsProcessing.NotificationWindow
    {
    /// <summary>
    /// Interaction logic for UploadingNotificationControl.xaml
    /// </summary>
    public partial class UploadingNotificationControl : UserControl
        {
        NotificationViewModel notificationVM = null;
        public UploadingNotificationControl( NotificationViewModel notificationVM )
            {
            this.notificationVM = notificationVM;
            InitializeComponent();
            this.Loaded += UploadingNotificationControl_Loaded;
            }

        void UploadingNotificationControl_Loaded( object sender, RoutedEventArgs e )
            {
            this.DataContext = notificationVM;
            }
        }
    }
