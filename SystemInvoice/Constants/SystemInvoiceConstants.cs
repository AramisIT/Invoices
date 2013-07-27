using System;
using System.Drawing;

namespace Catalogs
    {
    public class SystemInvoiceConstants : SystemConsts
        {
        /// <summary>
        /// Колличество дней которое добавляется к текущей дате, все разрешительные которые заканчиваются раньше итоговой даты отображаются красным
        /// </summary>
        public static string AlarmForApprovalBecomeFailDays
            {
            get
                {
                lock (locker)
                    {
                    return z_AlarmForApprovalBecomeFailDays;
                    }
                }
            set
                {
                lock (locker)
                    {
                    if (z_AlarmForApprovalBecomeFailDays != value)
                        {
                        z_AlarmForApprovalBecomeFailDays = value;
                        NotifyPropertyChanged( "AlarmForApprovalBecomeFailDays" );
                        }
                    }
                }
            }
        private static string z_AlarmForApprovalBecomeFailDays = "0";

        }
    }
