using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public class SumExpressionDataFormatterConstructor : IFormatterConstructor
        {
        public const string DateExpressionConstant = "d:";
        int callingDeepness = 0;
        public IDataFormatter Create( string parameter, Type targetType, Func<string, IDataFormatter> formattersResolver )
            {
            try
                {
                if (callingDeepness > 10)
                    {
                    return null;
                    }
                callingDeepness++;
                string constExpr = string.Format( @"""{0}""+", DateExpressionConstant );
                if (parameter.StartsWith( constExpr ))
                    {
                    targetType = typeof( DateTime );
                    parameter = parameter.Replace( constExpr, "" );
                    }
                string[] content = parameter.Split( new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries );                
                AuxiliaryExpressionsCollection expressions = (new ExpressionBuilder()).CreateExpressions( targetType, formattersResolver, content );
                if (expressions.Count > 0)
                    {
                    return new SumExpressionDataFormatter( expressions );
                    }
                return null;
                }
            finally
                {
                callingDeepness = 0;
                }
            }      
        }
    }
