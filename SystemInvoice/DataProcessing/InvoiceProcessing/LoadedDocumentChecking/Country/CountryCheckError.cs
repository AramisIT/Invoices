using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Country
    {
    class CountryCheckError : CompareWithDBCellError
        {
        public CountryCheckError( string inDocumentValue, string inDBValue, string columnName )
            : base( inDocumentValue, inDBValue, columnName )
            {
            }

        public CountryCheckError()
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
            long countryID = dbCache.CountryCahceObjectsStore.GetIdForCountryShortName( InDocumentValue );
            if (countryID == 0)//попытка записать в базу несуществующее значение
                {
                throw new CannotWriteToDBException( string.Format( @"В справочнике ""Страны"" отсутствует страна, которой соответствует код ""{0}""", InDocumentValue ) );
                }
            Catalogs.Country country = new Catalogs.Country();
            country.Id = countryID;
            nomenclature.Country = country;
            nomenclature.Write();
            }
        }
    }
