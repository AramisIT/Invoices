using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.NotificationWindow
    {
    /// <summary>
    /// Диалоговое окно, отображающее процес загрузки
    /// </summary>
    public class UploadNotificationWindow : System.Windows.Window
        {
        private NotificationViewModel notifyVM = new NotificationViewModel();
        public UploadNotificationWindow()
            {
            this.Content = new UploadingNotificationControl( this.notifyVM );
            this.Width = 500;
            this.Height = 80;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Top = (screenHeight - Height) / 2;
            this.Left = (screenWidth - Width) / 2;
            }

        public int TotalCount
            {
            get { return notifyVM.TotalCount; }
            set { notifyVM.TotalCount = value; }
            }

        public int Current
            {
            get { return notifyVM.Current; }
            set
                {
                notifyVM.Current = value;
                }
            }

        static UploadNotificationWindow notificationWindow = null;

        public static UploadNotificationWindow Window
            {
            get { return notificationWindow; }
            }

        public static void ShowWindow()
            {
            var waitForWindowShowHandle = new AutoResetEvent( false );
            //Создаем STA - поток для отображения WPF - окна
            Thread thread = new Thread( new ThreadStart( () =>
            {
                notificationWindow = new UploadNotificationWindow();
                notificationWindow.Loaded += ( o, s ) => { waitForWindowShowHandle.Set(); };
                notificationWindow.ShowDialog();
            } ) );
            thread.SetApartmentState( ApartmentState.STA );
            thread.Start();
            waitForWindowShowHandle.WaitOne();
            }

        public static void CloseWindow()
            {
            if (notificationWindow != null)
                {
                notificationWindow.Dispatcher.Invoke( new Action( () =>
                    {
                        notificationWindow.Close();
                    } ) );
                notificationWindow = null;
                }
            }
        }
    }
