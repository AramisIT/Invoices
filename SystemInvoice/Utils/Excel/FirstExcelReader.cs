using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Excel;

namespace SystemInvoice.Utils.Excel
    {
    public class FirstExcelRow : IExcelRow
        {
        private int columnsCount;
        private DataRow dataRow;

        public FirstExcelRow(DataRow dataRow, int columnsCount)
            {
            this.dataRow = dataRow;
            this.columnsCount = columnsCount;
            }

        public string GetString(int cellIndex)
            {
            var value = getValue(cellIndex);
            if (value == null) return string.Empty;

            return value.ToString().Trim();
            }

        private object getValue(int cellIndex)
            {
            if (cellIndex >= columnsCount) return null;
            return dataRow[cellIndex];
            }

        public DateTime GetDate(int cellIndex)
            {
            var value = getValue(cellIndex);
            if (value == null) return DateTime.MinValue;
            if (value is DateTime) return (DateTime)value;

            if (!(value is string))
                {
                var dec = value.ToDecimal();
                if (dec > Int32.MaxValue) return DateTime.MinValue;

                TimeSpan datefromexcel = new TimeSpan(Convert.ToInt32(dec) - 2, 0, 0, 0);
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

    public class FirstExcelSheet : IExcelSheet
        {
        public FirstExcelSheet(DataTable dataTable)
            {
            this.dataTable = dataTable;
            RowsCount = dataTable.Rows.Count;
            ColumnsCount = dataTable.Columns.Count;
            this.Name = dataTable.TableName;
            rows = new Dictionary<int, FirstExcelRow>();
            }

        public int RowsCount { get; private set; }

        private Dictionary<int, FirstExcelRow> rows;
        private DataTable dataTable;
        public IExcelRow this[int rowIndex]
            {
            get
                {
                if (rowIndex >= RowsCount) return null;

                FirstExcelRow row;
                if (!rows.TryGetValue(rowIndex, out row))
                    {
                    row = new FirstExcelRow(dataTable.Rows[rowIndex], ColumnsCount);
                    rows.Add(rowIndex, row);
                    }
                return row;
                }
            }

        public string Name { get; private set; }

        public int ColumnsCount { get; private set; }
        }

    public class FirstExcelReader : IExcelReader
        {
        private DataSet dataSet;

        public bool OpenFile(string fileName)
            {
            try
                {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                    var openXmlFileType = fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase);
                    var excelReader = openXmlFileType ?
                        ExcelReaderFactory.CreateOpenXmlReader(stream) :
                        ExcelReaderFactory.CreateBinaryReader(stream);

                    dataSet = excelReader.AsDataSet();
                    }
                }
            catch
                {
                return false;
                }

            if (dataSet == null) return false;

            SheetsCount = dataSet.Tables.Count;
            sheets = new Dictionary<int, FirstExcelSheet>();
            return true;
            }

        public int SheetsCount { get; private set; }

        private Dictionary<int, FirstExcelSheet> sheets;

        public IExcelSheet this[int sheetIndex]
            {
            get
                {
                if (sheetIndex >= SheetsCount) return null;

                FirstExcelSheet nativeExcelSheet;
                if (!sheets.TryGetValue(sheetIndex, out nativeExcelSheet))
                    {
                    nativeExcelSheet = new FirstExcelSheet(dataSet.Tables[sheetIndex]);
                    sheets.Add(sheetIndex, nativeExcelSheet);
                    }
                return nativeExcelSheet;
                }
            }

        private bool isDisposed;

        public void Dispose()
            {
            if (isDisposed) return;

            dataSet = null;

            isDisposed = true;
            }
        }

    }
