using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions
    {
    internal class IndexExpression : IExpression
        {
        private int index = 0;

        public IndexExpression(int index)
            {
            this.index = index - 1;
            }

        public string GetValue(AramisWpfComponents.Excel.Row row)
            {
            return row[index].Value.ToString();
            }
        }
    }
