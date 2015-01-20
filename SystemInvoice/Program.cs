using Aramis;
using Aramis.Platform;
using AramisDesktopUserInterface;
using System;

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
                SystemAramis.SystemStart(args, new DesktopUserInterfaceEngine(typeof(MainForm)));
                }
            else
                {
                ComponentTests test = new ComponentTests();
                test.CheckIndexedCache();//.testSplitting();
                }
            }
        }
    }