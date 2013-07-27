using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public delegate object CustomExpressionDelegate( params object[] parameters );
    public class CustomDelegateExpressionFormatterConstructor : IFormatterConstructor
        {
        ICustomExpressionHandler processingHandler = null;

        public CustomDelegateExpressionFormatterConstructor( ICustomExpressionHandler processingFunc )
            {
            this.processingHandler = processingFunc;
            }

        public IDataFormatter Create( string parameter, Type targetType, Func<string, IDataFormatter> formattersResolver )
            {
            if (processingHandler == null)
                {
                return null;
                }
            string[] parameters = parameter.Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries );
            if (parameters.Length == 0)
                {
                return null;
                }
            for (int i = 0; i < parameters.Length; i++)
                {
                string currentParameter = parameters[i];
                if (SystemInvoice.Documents.Invoice.InvoiceColumnNamesTranslated.ContainsKey( currentParameter ))
                    {
                    parameters[i] = SystemInvoice.Documents.Invoice.InvoiceColumnNamesTranslated[currentParameter];
                    }
                }
            AuxiliaryExpressionsCollection expressionsCollection = (new ExpressionBuilder()).CreateExpressions( targetType, formattersResolver, parameters );
            return new CustomDelegateExpressionFormatter( expressionsCollection, processingHandler );
            }
        }
    }
