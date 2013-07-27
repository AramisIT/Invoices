using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public class CustomExcelRowProcessingFormatter : IDataFormatter
        {
        private Func<Row, object> processingFunc = null;

        public CustomExcelRowProcessingFormatter( Func<Row, object> processingFunc )
            {
            this.processingFunc = processingFunc;
            }

        public object Format(Row row )
            {
            if (processingFunc == null)
                {
                return null;
                }
            return processingFunc( row );
            }
        }
    }
