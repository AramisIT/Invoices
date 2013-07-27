using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting.Formatters.AuxiliaryExpressions
    {
    internal class DateIndexExprssion : IExpression
        {
        private int index = 0;

        public DateIndexExprssion( int index )
            {
            this.index = index - 1;
            }

        public string GetValue( AramisWpfComponents.Excel.Row row )
            {
            //очень плохой и "грязный" прием, дабы не вылетал эксепшн при попытке получить дату из текстовой или другой ячейки
            try
                {
                if (row[index].CELL.CellType == NPOI.SS.UserModel.CellType.NUMERIC)
                    {
                    return row[index].CELL.DateCellValue.ToShortDateString();
                    }
                }
            catch
                {
                }
            return row[index].Value.ToString();
            }
        }
    }
