using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Ошибка - уведомление, которая предназначена для информационного вывода, не учитывается при подсчете ошибок.
    /// </summary>
    public class NotificationError : CellError
        {
        public string NotificationMessage { get; private set; }

        public NotificationError(string notificationMessage)
            {
            this.NotificationMessage = notificationMessage;
            }
        }
    }
