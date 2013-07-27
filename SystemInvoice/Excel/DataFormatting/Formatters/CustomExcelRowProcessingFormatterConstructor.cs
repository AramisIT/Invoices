using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public class CustomExcelRowProcessingFormatterConstructor : IFormatterConstructor
        {
        private Func<Row, object> processingFunc = null;

        public CustomExcelRowProcessingFormatterConstructor( Func<Row, object> processingFunc )
            {
            this.processingFunc = processingFunc;
            }

        public IDataFormatter Create( string expression, Type targetType, Func<string, IDataFormatter> formattersResolver )
            {
            CustomExcelRowProcessingFormatter dataFormatter = new CustomExcelRowProcessingFormatter( this.processingFunc );
            return dataFormatter;
            }
        }
    }
