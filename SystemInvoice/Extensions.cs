using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Aramis.Attributes;

namespace SystemInvoice
    {
    public static class Extensions
        {
        public static T TryGetColumnValue<T>(this DataRow row, string columnName, T defaultValue)
            {
            object objectValue = row[columnName];
            if (objectValue == DBNull.Value || objectValue == null)
                {
                return defaultValue;
                }
            return (T)objectValue;
            }

        public static T TrySafeGetColumnValue<T>(this DataRow row, string columnName, T defaultValue)
            {
            if (row == null || !row.Table.Columns.Contains(columnName))
                {
                return defaultValue;
                }
            object objectValue = row[columnName];
            if (objectValue == DBNull.Value || objectValue == null)
                {
                return defaultValue;
                }
            return (T)objectValue;
            }

        public static DataRow CopyRow(this DataTable table, DataRow rowToCopy)
            {
            DataRow rowToInsert = table.NewRow();
            DataTable rowOwner = rowToCopy.Table;
            foreach (DataColumn column in rowOwner.Columns)
                {
                rowToInsert[column.ColumnName] = rowToCopy[column.ColumnName];
                }
            table.Rows.Add(rowToInsert);
            return rowToInsert;
            }

        //public static DateTime EndOfYear( this DateTime dateTime )
        //    {
        //    return new DateTime( dateTime.Year, 1, 1 ).AddTicks( -1 ).AddYears( 1 );
        //    }

        }
    }
