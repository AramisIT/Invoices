using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using Aramis.DatabaseConnector;
using Aramis.Core;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Files
    {
    /// <summary>
    /// Выгружает обработанный файл
    /// </summary>
    public class ProcessedDocumentUnloader : DocumentUnloaderBase
        {

        public ProcessedDocumentUnloader( Invoice invoice )
            : base( invoice )
            {
            base.Unloader = new AggregatingTableUnloader( ProcessingConsts.EXCEL_UNLOAD_AGGREGATE_ROW_COLOR, true );
            }

        protected override string UnloadIndexColumnName
            {
            get { return ProcessingConsts.EXCEL_UNLOAD_PROCESSED_COLUMN_NUMBER_NAME; }
            }

        public void SaveAllItems( string fileName )
            {
            base.SaveTable( fileName, this.Invoice.Goods, 1 );
            }
        }
    }
