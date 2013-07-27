using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureOriginalName
    {
    class NomenclatureOriginalError : CompareWithDBCellError
        {
        public NomenclatureOriginalError( string inDocumentValue, string inDbValue, string columnName )
            : base( inDocumentValue, inDbValue, columnName )
            {
            }

        public NomenclatureOriginalError()
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
            nomenclature.NameOriginal = InDocumentValue;
            nomenclature.Write();
            }
        }
    }
