using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;

namespace SystemInvoice.Utils
    {
    public interface IExcelRow
        {
        /// <summary>
        /// Returns trimmed string
        /// </summary>
        /// <param name="cellIndex">cell index</param>
        /// <returns>value string or empty string</returns>
        string GetString(int cellIndex);

        /// <summary>
        /// Returns date 
        /// </summary>
        /// <param name="cellIndex">cell index</param>
        /// <returns>Date or 01.01.0001</returns>
        DateTime GetDate(int cellIndex);

        /// <summary>
        /// Returns long value
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns>cell value or 0</returns>
        long GetLong(int cellIndex);

        /// <summary>
        /// Returns double value
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns>cell value or 0.0</returns>
        double GetDouble(int cellIndex);
        }

    public interface IExcelSheet
        {
        int RowsCount { get; }
        int ColumnsCount { get; }

        IExcelRow this[int rowIndex] { get; }

        string Name { get; }
        }

    public interface IExcelReader : IDisposable
        {
        bool OpenFile(string fileName);

        int SheetsCount { get; }
        IExcelSheet this[int sheetIndex] { get; }
        }
    }
