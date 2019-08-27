﻿using System;
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
                DateTime dt = new DateTime();

                if (args.Length > 2)
                {                    
                    DateTime.TryParse(args[2], out dt);
                    if (dt > DateTime.MinValue)
                    {
                        currentDateShort = args[2];
                    }
                }

                switch (args[0])
                {
                    case "DBs":
                        Common.GenerateDBStructure(currentDate, currentDateShort, new List<string>(), null, args[1]);
                        break;
                    case "GenerateLocalDB":
                        Common.GenerateLocalDB();
                        break;
                    case "GRPSQL":
                        Common.GroupSQLPostScriptFiles(currentDateShort, false, null, args[1], args[2]);
                        break;
                    case "GRPSQLOverridden":
                        Common.GroupSQLPostScriptFiles(currentDateShort, true, null, args[1], args[2]);
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
                    case "CheckPathLength":
                        Common.CheckPathLength(currentDateShort, Common.CheckPathLengthSourcePath, Common.CheckPathLengthOutputPath, args[1], int.Parse(args[2]));
                        break;
                    case "GRPCheckPathLength":
                        Common.GroupCheckPathLengthFiles(currentDateShort, null);
                        break;
                    default:
                        Console.WriteLine("Invalid argument: {0} or {1}", args[0], args[1]);
                        break;
                }
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
