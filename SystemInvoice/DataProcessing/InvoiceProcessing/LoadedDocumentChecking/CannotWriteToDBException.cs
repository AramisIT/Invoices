using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Исключение вызванное некоректностью данных которые записываются в БД
    /// </summary>
    public class CannotWriteToDBException : Exception
        {
        public CannotWriteToDBException( string message )
            : base( message )
            {
            }
        }
    }
