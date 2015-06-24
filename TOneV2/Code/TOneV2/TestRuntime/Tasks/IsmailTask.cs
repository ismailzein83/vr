using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TOne.Business;
using TOne.Entities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            ////TOne.LCR.Data.SQL.CodeDataManager codeDataManager = new TOne.LCR.Data.SQL.CodeDataManager();
            ////TOne.LCR.Data.SQL.CodeMatchDataManager codeMatchDataManager = new TOne.LCR.Data.SQL.CodeMatchDataManager();
            ////List<SupplierCodeInfo> suppliersCodeInfo = codeDataManager.GetActiveSupplierCodeInfo(DateTime.Today, DateTime.Today);
            ////List<string> distinctCodes = codeDataManager.GetDistinctCodes(false);
            ////codeMatchDataManager.FillCodeMatchesFromCodes(new CodeList(distinctCodes), suppliersCodeInfo, DateTime.Today);
            ////Console.ReadKey();
            ////return;
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);
            
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

            BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "RoutingProcess",
            //    InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
            //    {
            //        DivideProcessIntoSubProcesses = true,
            //        EffectiveTime = DateTime.Now,
            //        IsFuture = false,
            //        IsLcrOnly = false
            //    }
            //});

            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput
                {
                    RepricingDay = DateTime.Parse("2014-07-07")//,
                   // DivideProcessIntoSubProcesses = true
                }
            });

            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "RoutingProcess",
            //    InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
            //    {
            //        EffectiveTime = DateTime.Now,
            //        IsFuture = true
            //    }
            //});

            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "UpdateCodeZoneMatchProcess",
            //    InputArguments = new TOne.LCRProcess.Arguments.UpdateCodeZoneMatchProcessInput
            //    {
            //        IsFuture = false,
            //        CodeEffectiveOn = DateTime.Now
            //    }
            //});

            //processManager.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "UpdateZoneRateProcess",
            //    InputArguments = new TOne.LCRProcess.Arguments.UpdateZoneRateProcessInput
            //    {
            //        IsFuture = false,
            //        ForSupplier = true,
            //        RateEffectiveOn = DateTime.Now
            //    }
            //});
        }

        //static bool _isRunning;
        //static object _lockObj = new object();
        //static void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    lock (_lockObj)
        //    {
        //        if (_isRunning)
        //            return;
        //        _isRunning = true;
        //    }
        //    try
        //    {
        //        //BusinessProcessRuntime.Current.LoadAndExecutePendings();

        //        BusinessProcessRuntime.Current.ExecutePendings();
        //        BusinessProcessRuntime.Current.TriggerPendingEvents();
        //    }
        //    finally
        //    {
        //        lock (_lockObj)
        //        {
        //            _isRunning = false;
        //        }
        //    }
        //}

        private static void TriggerProcess(DateTime date)
        {
            TOne.CDRProcess.Arguments.DailyRepricingProcessInput inputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput { RepricingDay = date };
            CreateProcessInput input = new CreateProcessInput
            {
                InputArguments = inputArguments
            };
            BPClient processManager = new BPClient();
            processManager.CreateNewProcess(input);
        }
    }
}
