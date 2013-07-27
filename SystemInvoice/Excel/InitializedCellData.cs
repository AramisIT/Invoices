using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel
    {
    public class InitializedCellData
        {
        public readonly string Text;
        public readonly int RowSpan;
        public readonly int ColSpan;
        public readonly bool IsBold;

        public InitializedCellData( string text, int rowSpan, int colSpan, bool isBold )
            {
            this.Text = text;
            this.RowSpan = rowSpan;
            this.ColSpan = colSpan;
            this.IsBold = isBold;
            }
        }
    }
