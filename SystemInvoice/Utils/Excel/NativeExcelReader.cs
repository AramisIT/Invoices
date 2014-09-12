using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace SystemInvoice.Utils.Excel
    {
    public class NativeExcelRow : IExcelRow
        {
        private Range range;
        private int rowNumber;
        private int columnsCount;

        public NativeExcelRow(Range range, int rowIndex, int columnsCount)
            {
            this.range = range;
            this.rowNumber = rowIndex + 1;
            this.columnsCount = columnsCount;
            }

        public string GetString(int cellIndex)
            {
            var value = getValue(cellIndex);
            if (value == null) return string.Empty;

            return value.ToString();
            }

        private object getValue(int cellIndex)
            {
            if (cellIndex > columnsCount) return null;
            return (range[rowNumber, cellIndex + 1] as Range).Value2;
            }

        public DateTime GetDate(int cellIndex)
            {
            var value = getValue(cellIndex);
            if (value == null) return DateTime.MinValue;
            if (value is DateTime) return (DateTime)value;

            if (!(value is string))
                {
                TimeSpan datefromexcel = new TimeSpan(Convert.ToInt32(value) - 2, 0, 0, 0);
                DateTime inputdate = new DateTime(1900, 1, 1).Add(datefromexcel);
                return inputdate;
                }
            var strValue = value.ToString();
            try
                {
                if (strValue.Contains('/'))
                    {
                    var parts = strValue.Split('/');
                    return new DateTime(2000 + parts[2].ToInt32(), parts[0].ToInt32(), parts[1].ToInt32());
                    }
                else
                    {
                    return strValue.ConvertToDateTime("dd.MM.yyyy").Date;
                    }
                }
            catch
                {
                return DateTime.MinValue;
                }
            }

        public long GetLong(int cellIndex)
            {
            var value = getValue(cellIndex);
            if (value == null) return 0L;
            if (value is long) return (long)value;

            return (long)Math.Round(value.ToString().ConvertToDouble(), 0);
            }

        public double GetDouble(int cellIndex)
            {
            var value = getValue(cellIndex);
            if (value == null) return 0.0;
            if (value is double) return (double)value;

            return value.ToString().ConvertToDouble();
            }
        }

    public class NativeExcelSheet : IExcelSheet
        {
        private Worksheet worksheet;

        public NativeExcelSheet(Worksheet worksheet)
            {
            this.worksheet = worksheet;
            var range = worksheet.UsedRange;
            RowsCount = range.Rows.Count;
            ColumnsCount = range.Columns.Count;
            this.Name = worksheet.Name;
            rows = new Dictionary<int, NativeExcelRow>();
            }

        public int RowsCount { get; private set; }

        private Dictionary<int, NativeExcelRow> rows;
        public IExcelRow this[int rowIndex]
            {
            get
                {
                if (rowIndex > RowsCount) return null;

                NativeExcelRow row;
                if (!rows.TryGetValue(rowIndex, out row))
                    {
                    row = new NativeExcelRow(worksheet.UsedRange.Cells, rowIndex, ColumnsCount);
                    rows.Add(rowIndex, row);
                    }
                return row;
                }
            }

        public string Name { get; private set; }

        public int ColumnsCount { get; private set; }
        }

    public class NativeExcelReader : IExcelReader
        {
        private Application application;
        private Workbook book;

        public bool OpenFile(string fileName)
            {
            application = new Application();
            book = application.Workbooks.Open(fileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            SheetsCount = book.Worksheets.Count;
            sheets = new Dictionary<int, NativeExcelSheet>();
            return true;
            }

        public int SheetsCount { get; private set; }

        private Dictionary<int, NativeExcelSheet> sheets;

        public IExcelSheet this[int sheetIndex]
            {
            get
                {
                if (sheetIndex >= SheetsCount) return null;

                NativeExcelSheet nativeExcelSheet;
                if (!sheets.TryGetValue(sheetIndex, out nativeExcelSheet))
                    {
                    nativeExcelSheet = new NativeExcelSheet((Worksheet)book.Worksheets.get_Item(sheetIndex + 1));
                    sheets.Add(sheetIndex, nativeExcelSheet);
                    }
                return nativeExcelSheet;
                }
            }

        private bool isDisposed;

        public void Dispose()
            {
            if (isDisposed) return;

            book.Close(true, null, null);
            application.Quit();

            isDisposed = true;
            }
        }

    }
