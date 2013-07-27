using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.RDChecking
    {
    public class RDCheckError : NotificationError
        {
        public RDCheckError(string error)
            : base(error)
            {

            }
        }
    }
