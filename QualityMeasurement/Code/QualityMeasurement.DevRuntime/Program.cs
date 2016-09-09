using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanrise.Runtime;

namespace QM.Runtime
{
    class Program
    {
        static void Main(string[] args)
        {
            if (ConfigurationManager.AppSettings["IsRuntimeService"] == "true")
            {
                
                SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 5) };
                Vanrise.Caching.Runtime.CachingDistributorRuntimeService cachingDistributorRuntimeService = new Vanrise.Caching.Runtime.CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
                
                var runtimeServices = new List<RuntimeService>();

                runtimeServices.Add(schedulerService);
                runtimeServices.Add(cachingDistributorRuntimeService);

                RuntimeHost host = new RuntimeHost(runtimeServices);
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
