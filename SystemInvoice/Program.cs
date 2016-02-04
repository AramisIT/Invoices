using Aramis;
using Aramis.Platform;
using AramisDesktopUserInterface;
using System;
using Catalogs;

namespace SystemInvoice
    {
    static class Program
        {
        static bool runOrTest = true;

        [STAThread, AramisSystem(DefaultLanguage = Language.MultiLanguage)]
        static void Main(string[] args)
            {
            if (runOrTest)
                {
                SystemAramis.SystemStart(args, new DesktopUserInterfaceEngine(typeof(MainForm), typeof(Users)));
                }
            else
                {
                ComponentTests test = new ComponentTests();
                test.CheckIndexedCache();//.testSplitting();
                }
            }
        }
    }