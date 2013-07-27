using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;
using SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public class SumExpressionDataFormatter : IDataFormatter
        {
        AuxiliaryExpressionsCollection expressions = null;

        internal SumExpressionDataFormatter( AuxiliaryExpressionsCollection expressions )
            {
            this.expressions = expressions;
            }

        public object Format( Row row )
            {            
            return string.Concat( expressions.Select( expr => expr.GetValue( row ) ) );
            }
        }
    }
