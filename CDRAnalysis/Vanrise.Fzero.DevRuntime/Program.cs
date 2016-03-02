using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Fzero.CDRImport.Business.ExecutionFlows;
using Vanrise.Integration.Business;

namespace Vanrise.Fzero.DevRuntime
{
    class Program
    {
        static void Main(string[] args)
        {            
            if(ConfigurationManager.AppSettings["IsRuntimeService"] == "true")
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
