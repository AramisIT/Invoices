using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;
using System.Data;

namespace SystemInvoice.Excel.DataFormatting.Formatters
    {
    public class SimpleIndexFormatter : IDataFormatter
        {
        int columnIndex = 0;
        Type dataType = null;
        Func<Row, object> typeConverter = null;

        public SimpleIndexFormatter( Type dataType, int columnIndex )
            {
            this.columnIndex = columnIndex - 1;
            this.dataType = dataType;
            }

        public object Format( Row row )
            {
            if (typeConverter == null)
                {
                createTypeConverter( row );
                }
            else
                {
                return typeConverter( row );
                }
            if (typeConverter != null)
                {
                return typeConverter( row );
                }
            return DBNull.Value;
            }

        private void createTypeConverter( Row row )
            {
            object value = row[columnIndex].Value;
            if (value == null)
                {
                return;
                }
            //Type valueType = value.GetType();
            //if (valueType == dataType)
            //    {
            //    typeConverter = ( rowItem ) => rowItem[columnIndex].Value;
            //    return;
            //    }
            else if (dataType == typeof( int ))
                {
                createNumericConverter<int>();
                return;
                }
            else if (dataType == typeof( long ))
                {
                createNumericConverter<long>();
                return;
                }
            else if (dataType == typeof( short ))
                {
                createNumericConverter<short>();
                return;
                }
            else if (dataType == typeof( double ))
                {
                createNumericConverter<double>();
                return;
                }
            else if (dataType == typeof( DateTime ))
                {
                createDateTimeConverter();
                return;
                }
            else if (dataType == typeof( string ))
                {
                typeConverter = ( rowItem ) =>
                    {
                        Type valueType = rowItem[columnIndex].Value.GetType();
                        if (valueType == dataType)
                            {
                            return rowItem[columnIndex].Value;
                            }
                        return rowItem[columnIndex].Value.ToString();
                    };
                return;
                }
            typeConverter = ( rowItem ) => rowItem[columnIndex].Value;
            }

        private void createDateTimeConverter()
            {
            typeConverter = ( rowItem ) =>
            {
                string strVal = rowItem[columnIndex].Value.ToString();
                DateTime dateTimeRes;
                if (DateTime.TryParse( strVal, out dateTimeRes ))
                    {
                    return dateTimeRes;
                    }
                return null;
            };
            }

        private void createNumericConverter<T>()
            {
            typeConverter = ( rowItem ) =>
            {
                Type valueType = rowItem[columnIndex].Value.GetType();
                if (valueType == dataType)
                    {
                    return rowItem[columnIndex].Value;
                    }
                string strVal = rowItem[columnIndex].Value.ToString();
                double numericVal = 0;
                if (double.TryParse( strVal, out numericVal ))
                    {
                    dynamic dynamicVal = numericVal;
                    return (T)dynamicVal;
                    }
                return null;
            };
            }
        }
    }
