using System.Collections.Generic;
using System.Data;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.RowsGrouping
    {
    /// <summary>
    /// Используется для сортировки и дальнейшей группировки строк в табличной части инвойса
    /// </summary>
    class GoodsRowsComparer : IComparer<DataRow>, IEqualityComparer<DataRow>
        {

        public bool Equals( DataRow x, DataRow y )
            {
            return x[ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME] )
                && x[ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME] )
                && x[ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME] ) 
                && x[ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME] )
                && x[ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME] ) 
                && x[ProcessingConsts.ColumnNames.INVOICE_NUMBER_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.INVOICE_NUMBER_COLUMN_NAME] )
                && x[ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME] ) 
                && x[ProcessingConsts.ColumnNames.SUB_GROUP_OF_GOODS_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.SUB_GROUP_OF_GOODS_COLUMN_NAME] )
                && x[ProcessingConsts.ColumnNames.GROUP_CODE_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.GROUP_CODE_COLUMN_NAME] )
                && x[ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME].Equals( y[ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME] );
            }

        public int GetHashCode( DataRow obj )
            {
            return string.Concat(obj[ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME], obj[ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME],
                obj[ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME], obj[ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME],
                obj[ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME], obj[ProcessingConsts.ColumnNames.INVOICE_NUMBER_COLUMN_NAME],
                obj[ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME], obj[ProcessingConsts.ColumnNames.SUB_GROUP_OF_GOODS_COLUMN_NAME],
                obj[ProcessingConsts.ColumnNames.GROUP_CODE_COLUMN_NAME], obj[ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME]).GetHashCode();
            }

        public int Compare( DataRow x, DataRow y )
            {
            int compareResult = 0;
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.INVOICE_NUMBER_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.SUB_GROUP_OF_GOODS_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            return compareRows(x, y, ProcessingConsts.ColumnNames.GROUP_CODE_COLUMN_NAME);
            }

        private int compareRows( DataRow first, DataRow second, string columnName )
            {
            int compareResult = ((string)first[columnName]).CompareTo( (string)second[columnName] );
            return compareResult;
            }
        }
      
    }
