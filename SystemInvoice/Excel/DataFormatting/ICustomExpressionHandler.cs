using System;
namespace SystemInvoice.Excel.DataFormatting
    {
    public interface ICustomExpressionHandler
        {
        object ProcessRow(params object[] parameters);
        }
    }
