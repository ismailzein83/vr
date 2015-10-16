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

namespace Vanrise.Fzero.DevRuntime
{
    class Program
    {
        static void Main(string[] args)
        {

            //string cdrFlow = AllFlows.GetCDRFlow();
            //string stagingCDRFlow = AllFlows.GetStagingCDRFlow();

            if(ConfigurationManager.AppSettings["IsRuntimeService"] == "true")
            {
                BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
                QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
                SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 5) };

                var runtimeServices = new List<RuntimeService>();

                runtimeServices.Add(queueActivationService);

                runtimeServices.Add(bpService);

                runtimeServices.Add(schedulerService);

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
