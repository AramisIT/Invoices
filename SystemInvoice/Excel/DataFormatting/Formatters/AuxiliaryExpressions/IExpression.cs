using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;

namespace SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions
    {
    internal interface IExpression
        {
        string GetValue( Row row );
        }
    }
