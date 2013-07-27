using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NetWeight
    {
    class NetWeightError : CompareWithDBCellError
        {
        private string countColumnName = Documents.InvoiceColumnNames.Count.ToString();
        private string netWeightColumnName = Documents.InvoiceColumnNames.NetWeight.ToString();

        public NetWeightError( string inDocumentValue, string inDbValue, string columnName )
            : base( inDocumentValue, inDbValue, columnName )
            {
            }

        public NetWeightError()
            : base()
            {
            }

        public override void SetCurrentDBValueAsInCell( DataRow row, SystemInvoiceDBCache dbCache )
            {
            Nomenclature nomenclature = readNomenclature( row );
            if (nomenclature == null)
                {
                return;
                }
            string currentCountValue = row.TrySafeGetColumnValue<string>( countColumnName, "" );
            int currentCount;
            double currentValue;
            if (!double.TryParse( InDocumentValue, out currentValue ) ||
                !int.TryParse( currentCountValue, out currentCount ))
                {
                return;
                }
            double totalValue = 0;
            if (ColumnName.Equals( netWeightColumnName ))
                {
                totalValue = currentValue;
                }
            else
                {
                totalValue = currentValue * currentCount;
                }
            if (nomenclature.NetWeightFrom * currentCount > totalValue)
                {
                nomenclature.NetWeightFrom = Math.Round( Math.Floor( 1000 * totalValue / currentCount ) / 1000, 3 );
                }
            if (nomenclature.NetWeightTo * currentCount < totalValue)
                {
                nomenclature.NetWeightTo = Math.Round( Math.Ceiling( 1000 * totalValue / currentCount ) / 1000, 3 );
                }
            nomenclature.Write();
            }
        }
    }
