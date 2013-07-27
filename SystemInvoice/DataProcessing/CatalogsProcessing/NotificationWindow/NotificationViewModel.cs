using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.MVVM;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.NotificationWindow
    {
    /// <summary>
    /// Модель предстваления диалогового окна отоброжающего загрузку файлов
    /// </summary>
    public class NotificationViewModel : ViewModelBase
        {
        private int vm_TotalCount;
        public int TotalCount
            {
            get
                {
                return vm_TotalCount;
                }
            set
                {
                if (vm_TotalCount != value)
                    {
                    vm_TotalCount = value;
                    RaisePropertyChanged( "TotalCount" );
                    }
                }
            }

        private int vm_Current;
        public int Current
            {
            get
                {
                return vm_Current;
                }
            set
                {
                if (vm_Current != value)
                    {
                    vm_Current = value;
                    RaisePropertyChanged( "Current" );
                    }
                }
            }
        }
    }
