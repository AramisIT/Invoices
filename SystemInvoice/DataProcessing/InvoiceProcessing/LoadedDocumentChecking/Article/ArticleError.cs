using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Article
    {
    class ArticleError : CompareWithDBCellError
        {
        public ArticleError( string inDocumentValue, string inDBValue, string columnName )
            : base( inDocumentValue, inDBValue, columnName )
            {
            CanCopyFromDBOnly = true;
            }

        public ArticleError()
            : base()
            {
            CanCopyFromDBOnly = true;
            }
        }
    }
