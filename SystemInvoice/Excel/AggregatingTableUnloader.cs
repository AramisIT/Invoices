using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;

namespace SystemInvoice.Excel
    {
    public class AggregatingTableUnloader : TableUnloader
        {
        private bool unloadAggregateRow;
        private string aggregateRowColor = "";

        public AggregatingTableUnloader( string aggregateRowColor, bool unloadAggregateRow )
            {
            this.aggregateRowColor = aggregateRowColor;
            this.unloadAggregateRow = unloadAggregateRow;
            }

        protected override int GetRowsCount()
            {
            int baseCount = base.GetRowsCount();
            return baseCount;
            }

        protected override ExcelStyle GetCurrentObjectStyle( string propertyName )
            {
            if (this.isCurrentRowAggregate()&&!string.IsNullOrEmpty(aggregateRowColor))
                {
                return this.getAggregateRowStyle();
                }
            return base.GetCurrentObjectStyle( propertyName );
            }

        private ExcelStyle getAggregateRowStyle()
            {
            return stylesStore.GetStyle( aggregateRowColor );
            }

        private bool isCurrentRowAggregate()
            {
            return false;// currentRowIndex == (base.GetRowsCount() - 1);
            }
        }
    }
