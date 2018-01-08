using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;

namespace SOM.Runtime
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
