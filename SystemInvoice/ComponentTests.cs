using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Excel;
using SystemInvoice.Excel.DataFormatting;
using SystemInvoice.Excel.DataFormatting.Formatters;
using AramisWpfComponents.Excel;
using SystemInvoice.DataProcessing;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.CustomCodesCache;
using SystemInvoice.DataProcessing.Cache.CountryCache;

namespace SystemInvoice
    {
    public class ComponentTests
        {
        public void TestLoad()
            {
            //string funcTypeKeyWord = "customProcessing";
            //string filePath = @"E:\VSProjects\SystemInvoice\ExcelLoad\Honda_processed.xls";
            //TableLoader loader = new TableLoader();
            //ExcelMapper mapper = new ExcelMapper();
            ////registerExpression
            //loader.RegisterFormatter( funcTypeKeyWord, new CustomDelegateExpressionFormatterConstructor( ( obj ) =>
            //    {
            //        return obj[1];
            //    } ), true );
            //mapper.TryAddExpression( "ColumnFirst", ExcelMapper.IndexKey, "1" );
            //mapper.TryAddExpression( "ColumnSecond", ExcelMapper.IndexKey, "2" );
            //mapper.TryAddExpression( "ColumnCustom", funcTypeKeyWord, "3,4" );
            //DataTable table = loader.Transform( mapper, filePath, 1 );
            }

        public void testStruct()
            {
            //TradeMarkInfo tmInfo1 = new TradeMarkInfo( "test", 1 );
            //TradeMarkInfo tmInfo2 = new TradeMarkInfo( "test", 2 );
            //TradeMarkInfo tmInfo3 = new TradeMarkInfo( "test", 1 );
            //Console.WriteLine( "0==1?{0}", tmInfo1.Equals( tmInfo2 ) );
            //Console.WriteLine( "1==2?{0}", tmInfo2.Equals( tmInfo3 ) );
            //Console.WriteLine( "0==2?{0}", tmInfo1.Equals( tmInfo3 ) );
            //Console.ReadLine();
            }

        public void NewItemsLoader()
            {
            HashSet<long> longSet = new HashSet<long>();
            longSet.Add( 0 );
            longSet.Add( 1 );
            longSet.Add( 2 );
            longSet.Add( 0 );
            longSet.Add( 1 );
            longSet.Add( 4 );
            //string filePath = @"E:\VSProjects\SystemInvoice\ExcelLoad\ZaraDress.xls";
            //Documents.Invoice invoice = new Documents.Invoice();
            //invoice.Read( 5 );
            //InvoiceProcessedFilesLoader processedLoader = new InvoiceProcessedFilesLoader( invoice );
            //processedLoader.ProcessNewItems( filePath );
            //Console.ReadLine();
            }

        public void TestSorting()
            {
            int res = 1 | -1;
            int res2 = 1 | -1;
            DataTable tableToTest = new DataTable();
            tableToTest.Columns.Add( "Col1" );
            tableToTest.Columns.Add( "Col2" );
            tableToTest.Columns.Add( "Col3" );
            tableToTest.Columns.Add( "Col4" );
            tableToTest.Rows.Add( "a", "b", "c", "d" );
            tableToTest.Rows.Add( "a", "b", "c", "d1" );
            tableToTest.Rows.Add( "a", "b", "c1", "d1" );
            tableToTest.Rows.Add( "a", "b", "c1", "d1" );
            tableToTest.Rows.Add( "a", "b1", "c", "d" );
            tableToTest.Rows.Add( "a", "b", "c1", "d" );
            tableToTest.Rows.Add( "a1", "b", "c", "d" );
            tableToTest.Rows.Add( "a1", "b", "c1", "d" );
            tableToTest.Rows.Add( "a", "b", "c1", "d" );
            tableToTest.Rows.Add( "a2", "b", "c", "d" );
            tableToTest.Rows.Add( "a", "b", "c2", "d" );
            tableToTest.Rows.Add( "a", "b2", "c", "d" );
            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in tableToTest.Rows)
                {
                rows.Add( row );
                }
            IEnumerable<DataRow> rowsSorted = rows.OrderBy( ( row ) => row["Col1"] ).OrderBy( ( row ) => row["Col2"] ).OrderBy( ( row ) => row["Col3"] ).ToList();
            DataTable copyTable = tableToTest.Clone();
            //  tableToTest.Rows.Clear();
            foreach (DataRow sorted in rowsSorted)
                {
                copyTable.CopyRow( sorted );
                //  tableToTest.Rows.Add( sorted );
                }
            Console.ReadLine();
            }

        public void testSplitting()
            {
            string testString = @"cababab ""a ""b ab c";
            string[] parts = testString.Split( new string[] { "c" }, StringSplitOptions.None );// SystemInvoice.Excel.ExcelHelper.DivideString( testString );//
            Console.ReadLine();
            }

        public void TestManufacturerInfo()
            {
            //ManufacturerInfo mInfoA = new ManufacturerInfo( "aaaaa", 1 );
            //ManufacturerInfo mInfoB = new ManufacturerInfo( "bbbbb", 2 );
            //ManufacturerInfo mInfoC = new ManufacturerInfo( "ccccc", 1 );
            //ManufacturerInfo mInfoD = new ManufacturerInfo( "aaaaa", 1 );
            //HashSet<ManufacturerInfo> manInfoSet = new HashSet<ManufacturerInfo>();
            //manInfoSet.Add( mInfoA );
            //bool containsB = manInfoSet.Contains( mInfoB );
            //bool containsC = manInfoSet.Contains( mInfoC );
            //bool containsD = manInfoSet.Contains( mInfoD );
            //bool equalsB = mInfoA.Equals( mInfoB );
            //bool equalsC = mInfoA.Equals( mInfoC );
            //bool equalsD = mInfoA.Equals( mInfoD );
            //int iterationsCount = 1000000;
            //bool finalResult = false;
            //manInfoSet.Add( mInfoB );
            //manInfoSet.Add( mInfoC );
            //DateTime fromTime = DateTime.Now;
            //for (int i = 0; i < iterationsCount; i++)
            //    {
            //    finalResult = manInfoSet.Contains( mInfoD );
            //    }
            //double millisecs = (DateTime.Now - fromTime).TotalMilliseconds;  
            }

        public void CheckCache()
            {
            SystemInvoiceDBCache dbCache = new SystemInvoiceDBCache(null);
            dbCache.CustomsCodesCacheStore.Refresh();
            var cheItem = dbCache.CustomsCodesCacheStore.GetCachedObject( 410001 );
            DataProcessing.Cache.CustomCodesCache.CustomsCodesCacheObject searchCache = new DataProcessing.Cache.CustomCodesCache.CustomsCodesCacheObject( cheItem.Code );
            long finalResult = 0;
            int iterationsCount = 1000000;
            DateTime fromTime = DateTime.Now;
            for (int j = 0; j < iterationsCount; j++)
                {
                //searchCache.SetCode( cheItem.Code );
              //  searchCache = new DataProcessing.Cache.CustomCodesCache.CustomsCodesCache( cheItem.Code );
                finalResult = dbCache.CustomsCodesCacheStore.GetCustomsCodeIdForCodeName(cheItem.Code);//.GetCachedObjectId( searchCache );//
                }
            double millisecs = (DateTime.Now - fromTime).TotalMilliseconds;
            Console.WriteLine( "duration: " + millisecs );
            Console.ReadLine();
            }

        public void CheckIndexedCache()
            {
            CountryCahceObjectsStore countryStore = new CountryCahceObjectsStore();
            TransactionManager.TransactionManagerInstance.BeginBusinessTransaction();
            countryStore.Refresh();
            long foundedID = countryStore.GetIdForCountryShortName( "UA" );
         //   long foundedRuID = countryStore.GetIdForCountryRuName( "Украина" );
            TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
            }

        }
    }
