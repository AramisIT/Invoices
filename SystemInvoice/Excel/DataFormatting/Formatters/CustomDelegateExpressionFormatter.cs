using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;
using SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public class CustomDelegateExpressionFormatter : IDataFormatter
        {
        ICustomExpressionHandler processingHandler = null;
        AuxiliaryExpressionsCollection expressions = null;

        internal CustomDelegateExpressionFormatter( AuxiliaryExpressionsCollection expressions, ICustomExpressionHandler processingHandler )
            {
            this.processingHandler = processingHandler;
            this.expressions = expressions;
            }

        public object Format( Row row )
            {
            int expressionsCount = expressions.Count;
            string[] parameters = new string[expressionsCount];
            for (int i = 0; i < expressionsCount; i++)
                {
                parameters[i] = expressions[i].GetValue( row );
                }
            object processedVal = processingHandler.ProcessRow( parameters );
            if (processedVal is string)
                {
                return ((string)processedVal).Trim();
                }
            return processedVal??string.Empty;
            }
        }
    }
