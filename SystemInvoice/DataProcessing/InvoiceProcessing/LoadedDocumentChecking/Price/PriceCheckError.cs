using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Price
    {
    public class PriceCheckError : CompareWithDBCellError
        {
        private bool isComodityError = false;

        public PriceCheckError(string inDocumentValue, string inDbValue, string columnName, bool isComodityError)
            : base(inDocumentValue, inDbValue, columnName)
            {
            this.isComodityError = isComodityError;
            }

        public PriceCheckError()
            : base()
            {
            }

        public override void SetCurrentDBValueAsInCell(DataRow row, SystemInvoiceDBCache dbCache)
            {
            Nomenclature nomenclature = readNomenclature(row);
            if (nomenclature == null)
                {
                return;
                }
            // PriceChecker.RefreshSummValue( row, InDocumentValue );
            double inDocumentPriceValue = getPrice(InDocumentValue);
            nomenclature.Price = inDocumentPriceValue;
            nomenclature.Write();
            }

        //public override void SetCurrentErrorCellAsInDB( DataRow row, SystemInvoiceDBCache dbCache )
        //    {
        //    if (row.Table.Columns.Contains( ColumnName ))
        //        {
        //        row[ColumnName] = InDBValue;
        //        }
        //    PriceChecker.RefreshSummValue( row, InDBValue );
        //    }

        private double getPrice(string valueToConvert)
            {
            double doubleValue;
            if (!double.TryParse(valueToConvert, out doubleValue))
                {
                throw new CannotWriteToDBException("Неверный формат");
                }
            return doubleValue;
            }

        protected override string FormattErrorMessage(string inDocVal, string inDBVal)
            {
            if (!isComodityError)
                {
                return base.FormattErrorMessage(inDocVal, inDBVal);
                }
            return string.Format("Минимальное значение для биржевой цены встречавшееся ранее - {0}", InDBValue);
            }

        }
    }
