using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using System.Timers;
using System.Configuration;
using System.Collections.Concurrent;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess.Client;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime
{
    class Program
    {
        static void Main(string[] args)
        {
            if (ConfigurationManager.AppSettings["IsRuntimeService"] == "true")
            {
                RuntimeHost host = new RuntimeHost(args);
                host.Start();
                Console.ReadKey();
            }
            else
            {
                MainForm f = new MainForm();
                f.ShowDialog();
                Console.ReadKey();
                return;
            }
        }
    }
}
