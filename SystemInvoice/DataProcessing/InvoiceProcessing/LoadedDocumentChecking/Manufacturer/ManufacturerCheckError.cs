using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using Aramis.Core;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Manufacturer
    {
    class ManufacturerCheckError: CompareWithDBCellError
        {        
        public ManufacturerCheckError( string inDocumentValue, string inDBValue,string columnName )
            : base( inDocumentValue, inDBValue,columnName )
            {
            }

        public ManufacturerCheckError()
            : base()
            {
            }

        public override void SetCurrentDBValueAsInCell( System.Data.DataRow row, Cache.SystemInvoiceDBCache dbCache )
            {
            long inDocumentsManufacturer = dbCache.ManufacturerCacheObjectsStore.GetManufcaturerId( InDocumentValue );
            Nomenclature nomenclature = readNomenclature( row );
            if (nomenclature == null)
                {
                return;
                }
            if (inDocumentsManufacturer == 0)//попытка записать в базу несуществующее значение
                {
                throw new CannotWriteToDBException( string.Format( "Производитель {0} отсутствует в справочнике производителей.", InDocumentValue ) );
                }
            var newManufacturer = A.New<IManufacturer>();
            newManufacturer.Id = inDocumentsManufacturer;
            newManufacturer.Read();
            nomenclature.Manufacturer = newManufacturer;
            nomenclature.Write();
            }
        }
    }
