using System.Collections.Generic;
using System.Data;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.RowsGrouping
    {
    /// <summary>
    /// Используется для сортировки сгруппированных строк в табличной части инвойса, с тем что бы затем правильно отобразить шапку графы 31.
    /// </summary>
    public class ForHeaderGraf31ConstructComparer : IComparer<DataRow>
        {
        private const string S_SIZE_LETTERS = "s";
        private const string M_SIZE_LETTERS = "m";
        private const string L_SIZE_LETTERS = "l";
        private const string XS_SIZE_LETTERS = "xs";
        private const string XL_SIZE_LETTERS = "xl";
        private const string XXS_SIZE_LETTERS = "xxs";
        private const string XXL_SIZE_LETTERS = "xxl";
        private const string XXXS_SIZE_LETTERS = "xxxs";
        private const string XXXL_SIZE_LETTERS = "xxxl";
        private const int S_SIZE_COMPARE_CODE = 1001;
        private const int M_SIZE_COMPARE_CODE = 1002;
        private const int L_SIZE_COMPARE_CODE = 1003;
        private const int XS_SIZE_COMPARE_CODE = 1004;
        private const int XL_SIZE_COMPARE_CODE = 1005;
        private const int XXS_SIZE_COMPARE_CODE = 1006;
        private const int XXL_SIZE_COMPARE_CODE = 1007;
        private const int XXXS_SIZE_COMPARE_CODE = 1008;
        private const int XXXL_SIZE_COMPARE_CODE = 1009;

        public int Compare(DataRow x, DataRow y)
            {
            int compareResult = 0;
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }

            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareRows(x, y, ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            if ((compareResult = compareSizes(x, y, ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME)) != 0)
                {
                return compareResult;
                }
            return compareRows(x, y, ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME);
            }

        private int compareSizes(DataRow first, DataRow second, string sizeColumnName)
            {
            string xSizeStr = first.TrySafeGetColumnValue<string>(sizeColumnName, string.Empty);
            string ySizeStr = second.TrySafeGetColumnValue<string>(sizeColumnName, string.Empty);
            int intXsize = getIntSize(xSizeStr);
            int intYsize = getIntSize(ySizeStr);
            return intXsize.CompareTo((int)intYsize);
            }

        private int getIntSize(string sizeStr)
            {
            int sizeInt = 0;
            if (int.TryParse(sizeStr, out sizeInt))
                {
                return sizeInt;
                }
            sizeInt = getSizeFromLetter(sizeStr);
            return sizeInt;
            }

        private int getSizeFromLetter(string sizeStr)
            {
            switch (sizeStr.Trim().ToLower())
                {
                case (S_SIZE_LETTERS):
                    return S_SIZE_COMPARE_CODE;
                case (L_SIZE_LETTERS):
                    return L_SIZE_COMPARE_CODE;
                case (M_SIZE_LETTERS):
                    return M_SIZE_COMPARE_CODE;
                case (XS_SIZE_LETTERS):
                    return XS_SIZE_COMPARE_CODE;
                case (XL_SIZE_LETTERS):
                    return XL_SIZE_COMPARE_CODE;
                case (XXS_SIZE_LETTERS):
                    return XXS_SIZE_COMPARE_CODE;
                case (XXL_SIZE_LETTERS):
                    return XXL_SIZE_COMPARE_CODE;
                case (XXXS_SIZE_LETTERS):
                    return XXXS_SIZE_COMPARE_CODE;
                case (XXXL_SIZE_LETTERS):
                    return XXXL_SIZE_COMPARE_CODE;
                default:
                    return 0;
                }
            }

        private int compareRows(DataRow first, DataRow second, string columnName)
            {
            int compareResult = ((string)first[columnName]).CompareTo((string)second[columnName]);
            return compareResult;
            }
        }
    }
