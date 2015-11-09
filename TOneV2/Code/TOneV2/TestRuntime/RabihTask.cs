using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime
{
    public class RabihTask : ITask
    {
        public void Execute()
        {

            //IServiceDataManager datamanager = BEDataManagerFactory.GetDataManager<IServiceDataManager>();


            //string result = datamanager.GetServicesDisplayList(15);
            //return;

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Console.WriteLine("Hello from Rabih!");

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();


            //Console.ReadKey();
            //host.Stop();
            //Console.ReadKey();
            //BusinessProcessRuntime.Current.TerminatePendingProcesses();
            //Timer timer = new Timer(1000);
            //timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            //timer.Start();

            //////System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
            //////    {
            //////        for (DateTime d = DateTime.Parse(ConfigurationManager.AppSettings["RepricingFrom"]); d <= DateTime.Parse(ConfigurationManager.AppSettings["RepricingTo"]); d = d.AddDays(1))
            //////        {
            //////            TriggerProcess(d);
            //////            System.Threading.Thread.Sleep(30000);
            //////        }
            //////    });
            //////t.Start();
            //char key;
            //Console.WriteLine("Enter 'c' for Build Complete Route Build or 'p' for partial");
            //do
            //{

            //    key = Console.ReadKey().KeyChar;
            //    if (key == 'p')
            //        RunPartialRouteBuild();
            //    else if (key == 'c')
            //        RunCompleteRouteBuild();
            //    else
            //        Console.WriteLine("Enter 'c' for Build Complete Route Build or 'p' for partial");
            //} while (key != 'p' || key != 'c');




            RunCompleteRouteBuild();
            //RunPartialRouteBuild();
        }

        private static void RunCompleteRouteBuild()
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                //InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
                //{
                //    EffectiveTime = DateTime.Now,
                //    IsFuture = false,
                //    IsLcrOnly = false
                //}
                InputArguments = new TOne.WhS.Routing.BP.Arguments.RoutingProcessInput
                {
                    EffectiveTime = DateTime.Now,
                    IsFuture = false
                }
            });
        }

        private static void RunPartialRouteBuild()
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.LCRProcess.Arguments.DifferentialRoutingProcessInput()

            });
        }
    }
}
