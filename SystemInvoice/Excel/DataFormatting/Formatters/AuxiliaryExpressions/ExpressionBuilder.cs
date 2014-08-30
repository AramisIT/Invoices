using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions
    {
    internal class ExpressionBuilder
        {
        public AuxiliaryExpressionsCollection CreateExpressions(Type targetType, Func<string, IDataFormatter> formattersResolver, string[] expressionItems)
            {
            AuxiliaryExpressionsCollection expressions = new AuxiliaryExpressionsCollection();
            foreach (string contentItem in expressionItems.Select(itm => itm.Trim()))
                {
                IExpression expression = null;
                if (!tryCreateConstantExpression(contentItem, out expression) &&
                    !tryCreateIndexExpression(targetType, contentItem, out expression)
                    )
                    {
                    expression = createReferenceOrConstantExpression(formattersResolver, contentItem);
                    }
                expressions.Add(expression);
                continue;
                }
            return expressions;
            }

        private IExpression createReferenceOrConstantExpression(Func<string, IDataFormatter> formattersResolver, string parameterName)
            {
            IExpression expression = null;
            IDataFormatter additionalFormatter = null;
            if (formattersResolver != null && (additionalFormatter = formattersResolver(parameterName)) != null)
                {
                expression = new ReferenceExpression(additionalFormatter);
                }
            else
                {
                expression = new ConstantExpression(parameterName);
                }
            return expression;
            }

        private bool tryCreateIndexExpression(Type targetType, string contentItem, out IExpression expression)
            {
            expression = null;
            int columnIndex = 0;
            if (int.TryParse(contentItem, out columnIndex))
                {
                if (targetType == typeof(DateTime))
                    {
                    expression = new DateIndexExprssion(columnIndex);
                    }
                else
                    {
                    expression = new IndexExpression(columnIndex);
                    }
                return true;
                }
            return false;
            }

        private bool tryCreateConstantExpression(string contentItem, out IExpression expression)
            {
            expression = null;
            if (contentItem.StartsWith(@"""") && contentItem.EndsWith(@""""))
                {
                if (contentItem.Length <= 2)
                    {
                    expression = new ConstantExpression("");
                    return true;
                    }
                string constStr = contentItem.Substring(1, contentItem.Length - 2);
                expression = new ConstantExpression(constStr);
                return true;
                }
            return false;
            }
        }
    }
