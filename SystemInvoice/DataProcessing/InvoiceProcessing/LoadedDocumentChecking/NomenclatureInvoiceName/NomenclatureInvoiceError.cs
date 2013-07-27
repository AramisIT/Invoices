using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureInvoiceName
    {
    class NomenclatureInvoiceError : CompareWithDBCellError
        {
        public NomenclatureInvoiceError( string inDocumentValue, string inDbValue,string columnName )
            : base( inDocumentValue, inDbValue,columnName )
            {
            }

        public NomenclatureInvoiceError()
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
            if (string.IsNullOrEmpty( InDocumentValue ))
                {
                throw new CannotWriteToDBException( "Наименование инвойса не может быть пустым" );
                }
            nomenclature.NameInvoice = InDocumentValue;
            nomenclature.Write();
            }
        }
    }
