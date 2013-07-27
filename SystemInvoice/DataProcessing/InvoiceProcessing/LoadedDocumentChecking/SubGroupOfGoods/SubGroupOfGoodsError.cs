using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.SubGroupOfGoods
    {
    class SubGroupOfGoodsError : CompareWithDBCellError
        {
        public SubGroupOfGoodsError( string inDocumentValue, string inDbValue,string columnName )
            : base( inDocumentValue, inDbValue,columnName )
            {
            }

        public SubGroupOfGoodsError()
            : base()
            {
            }

        public override void SetCurrentDBValueAsInCell( DataRow row, SystemInvoiceDBCache dbCache )
            {
            string newGroupName = row.TryGetColumnValue<string>(Documents.InvoiceColumnNames.GroupOfGoods.ToString(),"").Trim();
            string newSubGroupName = row.TryGetColumnValue<string>(Documents.InvoiceColumnNames.SubGroupOfGoods.ToString(),"").Trim();
            string newSubGroupCode = row.TryGetColumnValue<string>( Documents.InvoiceColumnNames.GroupCode.ToString(), "" ).Trim();
            long newGroupId = dbCache.GetSubGroupId( newGroupName, newSubGroupName, newSubGroupCode );
            Nomenclature nomenclature = readNomenclature( row );
            if (nomenclature == null)
                {
                return;
                }
            if (newGroupId == 0)
                {
                throw new CannotWriteToDBException( "Подгруппа с таким значением имени/кода/группы не найдена" );
                }
            Catalogs.SubGroupOfGoods newSubGroup = new Catalogs.SubGroupOfGoods();
            newSubGroup.Id = newGroupId;
            nomenclature.SubGroupOfGoods = newSubGroup;
            nomenclature.Write();
            }
        }
    }
