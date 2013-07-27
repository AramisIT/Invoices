using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public class SimpleIndexFormatterConstructor : IFormatterConstructor
        {
        public IDataFormatter Create( string parameter, Type targetType, Func<string, IDataFormatter> formattersResolver )//, IGeneratorInfo info )
            {
            int excelColumnIndex = 0;
            if (int.TryParse( parameter, out excelColumnIndex ))
                {
                return new SimpleIndexFormatter( targetType, excelColumnIndex );
                }
            return null;
            }
        }
    }
