using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions
    {
    internal class ReferenceExpression : IExpression
        {
        private IDataFormatter formatter = null;

        public ReferenceExpression( IDataFormatter formatter )
            {
            this.formatter = formatter;
            }
        public string GetValue( AramisWpfComponents.Excel.Row row )
            {
            if (formatter != null)
                {
//                try
//                    {
                    return formatter.Format( row ).ToString();
//                    }
//                catch
//                    {
//                    return string.Empty;
//                    }
                }
            return string.Empty;
            }
        }
    }
