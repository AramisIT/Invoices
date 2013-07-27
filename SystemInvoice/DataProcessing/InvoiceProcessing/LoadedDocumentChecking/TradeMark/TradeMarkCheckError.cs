using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.TradeMark
    {
    class TradeMarkCheckError : CompareWithDBCellError
        {
        public TradeMarkCheckError( string inDocumentValue, string inDBValue, string columnName )
            : base( inDocumentValue, inDBValue, columnName )
            {
            CanCopyFromDBOnly = true;
            }

        public TradeMarkCheckError()
            : base()
            {
            CanCopyFromDBOnly = true;
            }
        }
    }
