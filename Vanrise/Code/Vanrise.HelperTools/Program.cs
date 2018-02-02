using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vanrise.HelperTools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length > 1)
            {
                string currentDate = DateTime.Now.ToString("yyyMMddHHmm");
                string currentDateShort = DateTime.Now.ToString("yyyMMdd");

                switch (args[0])
                {
                    case "DBs":
                        Common.GenerateDBStructure(currentDate, currentDateShort, new List<string>(), null, args[1]);
                        break;
                    case "GRPSQL":
                        Common.GroupSQLPostScriptFiles(currentDateShort, false, null, args[1]);
                        break;
                    case "GRPSQLOverridden":
                        Common.GroupSQLPostScriptFiles(currentDateShort, true, null, args[1]);
                        break;
                    case "CompressJS":
                        Common.CompressJSFiles(currentDateShort, "Javascripts", null, args[1]);
                        Common.CompressJSFiles(currentDateShort, "Modules", null, args[1]);
                        break;
                    case "GRPJS":
                        Common.GroupJSFiles(currentDateShort, "Javascripts", false, null, args[1]);
                        Common.GroupJSFiles(currentDateShort, "Modules", false, null, args[1]);
                        break;
                    case "GRPJSOverridden":
                        Common.GroupJSFiles(currentDateShort, "Modules", true, null, args[1]);
                        break;
                    case "Enumerations":
                        Common.GenerateEnumerationsScript(Common.BinPath, currentDateShort, false, null, args[1]);
                        break;
                    default:
                        Console.WriteLine("Invalid argument: {0} or {1}", args[0], args[1]);
                        break;
                }

                //if (args[0] == "DBs")
                //{
                //    Common.GenerateDBStructure(currentDate, currentDateShort, new List<string>(), null);
                //}
                //if (args[0] == "GRPSQL")
                //{
                //    Common.GroupSQLPostScriptFiles(currentDateShort, false, null);
                //}

                //if (args[0] == "GRPSQLOverridden")
                //{
                //    Common.GroupSQLPostScriptFiles(currentDateShort, true, null);
                //}

                //if (args[0] == "CompressJS")
                //{
                //    Common.CompressJSFiles(currentDateShort, "Javascripts", null);
                //    Common.CompressJSFiles(currentDateShort, "Modules", null);
                //}

                //if (args[0] == "GRPJS")
                //{
                //    Common.GroupJSFiles(currentDateShort, "Javascripts", false, null);
                //    Common.GroupJSFiles(currentDateShort, "Modules", false, null);
                //}
                //if (args[0] == "GRPJSOverridden")
                //{
                //    Common.GroupJSFiles(currentDateShort, "Modules", true, null);
                //}
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
