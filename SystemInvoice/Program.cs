using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using Aramis.Platform;
using System;
using System.IO;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing;

namespace SystemInvoice
    {
    static class Program
        {
        static bool runOrTest = true;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
            {
            //args = new string[] { "Deploy", "sqlpath=localhost;InvoiceDeplTest2;skipStart" };
            // SubGroupOfGoods Deploy sqlpath=localhost;InvoiceDeplTest;skipStart
            //ColumnName name = (ColumnName)Enum.Parse( typeof( ColumnName ), "Price" );
            //int nameIndex = (int)name;
            //ColumnName name2 = (ColumnName
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault( false );

            //DevExpress.Skins.SkinManager.EnableFormSkins();
            //DevExpress.UserSkins.BonusSkins.Register();
            //UserLookAndFeel.Default.SetSkinStyle( "DevExpress Style" );

            //Application.Run( new MainForm() );
            //  args = new string[1];
            //args = new string[] { "-setupr" };
            //    MainForm.LoadedForm += MainForm_LoadedForm;
            //    "Begin to start".AlertBox();
            if (runOrTest)
                {
                SystemAramis.SystemStart(args, typeof(MainForm));
                //SystemAramis.SystemStart(args,typeof(PlatformTest.AramisMainWindow));
                }
            else
                {
                ComponentTests test = new ComponentTests();
                test.CheckIndexedCache();//.testSplitting();
                }
            //   Console.ReadLine();
            }

        static void MainForm_LoadedForm()
            {
            test();
            }

        private static void test()
            {
            //ExcelLoadingFormat format = new  ExcelLoadingFormat();
            //format.Read(2);
            //string fileToLoadPath = @"E:\VSProjects\SystemInvoice\ExcelLoad\Honda_processed.xls";
            //InvoiceProcessedFilesLoader loader = new InvoiceProcessedFilesLoader();
            //loader.ProcessNewItems( fileToLoadPath, format );
            }
        }
    }