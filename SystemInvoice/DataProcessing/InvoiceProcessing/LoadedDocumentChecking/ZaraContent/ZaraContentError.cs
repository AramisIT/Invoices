using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.ZaraContent
    {
    public class ZaraContentError : CompareWithDBCellError
        {
        public ZaraContentError( string inDocumentValue, string inDBValue, string columnName )
            : base( inDocumentValue, inDBValue, columnName )
            {
            CanCopyFromDBOnly = true;
            }

        public ZaraContentError()
            : base()
            {
            CanCopyFromDBOnly = true;
            }
        }
    }
