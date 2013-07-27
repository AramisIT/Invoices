using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Ошибка о незагруженной строке. При наличии ошибки в колонке ROW_IS_INVALID_FAKE_COLUMN_NAME, вся строка выделяется как строка с новой номенклатурой
    /// </summary>
    public class RowCheckError : CellError
        {
        public const string ROW_IS_INVALID_FAKE_COLUMN_NAME = "ROWISNOTVALID";
        }
    }
