using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureDeclarationName
    {
    class NomenclatureDeclartionError : CompareWithDBCellError
        {
        public NomenclatureDeclartionError( string inDocumentValue, string inDbValue,string columnName )
            : base( inDocumentValue, inDbValue,columnName )
            {
            }

        public NomenclatureDeclartionError()
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
            if (string.IsNullOrEmpty(InDocumentValue))
                {
                throw new CannotWriteToDBException("Наименование декларации не может быть пустым");
                }
            nomenclature.NameDecl = InDocumentValue;
            nomenclature.Write();
            }
        }
    }
