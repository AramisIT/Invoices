using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions
    {
    internal class ConstantExpression : IExpression
        {
        private string constantValue = "";

        public ConstantExpression( string constantValue )
            {
            this.constantValue = constantValue;
            }

        public string GetValue( AramisWpfComponents.Excel.Row row )
            {
            return constantValue;
            }
        }
    }
