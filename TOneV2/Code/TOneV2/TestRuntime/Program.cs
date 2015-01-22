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

namespace TestRuntime
{
    class Program
    {
        static void Main(string[] args)
        {
            MainForm f = new MainForm();
            f.ShowDialog();
            Console.ReadKey();
            

            var config = new BPConfiguration { MaxConcurrentWorkflows = 20 };
            var ser = Vanrise.Common.Serializer.Serialize(config);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
          
            Console.WriteLine("Host Started");
            BusinessProcessRuntime.Current.TerminatePendingProcesses();

            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            //////System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
            //////    {
            //////        for (DateTime d = DateTime.Parse(ConfigurationManager.AppSettings["RepricingFrom"]); d <= DateTime.Parse(ConfigurationManager.AppSettings["RepricingTo"]); d = d.AddDays(1))
            //////        {
            //////            TriggerProcess(d);
            //////            System.Threading.Thread.Sleep(30000);
            //////        }
            //////    });
            //////t.Start();

            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "UpdateCodeZoneMatchProcess",
                InputArguments = new TOne.LCRProcess.Arguments.UpdateCodeZoneMatchProcessInput
                {
                    IsFuture = false,
                    CodeEffectiveOn = DateTime.Today,
                    GetChangedCodeGroupsOnly = false
                }
            });
            //////////BusinessProcessRuntime.Current.CreateNewProcess<TOne.LCRProcess.UpdateCodeZoneMatchProcess>(new CreateProcessInput { InputArguments = new TOne.LCRProcess.UpdateCodeZoneMatchProcessInput { IsFuture = true } });

            //processManager.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "UpdateZoneRateProcess"
            //});

            //TriggerProcess(DateTime.Parse("10/15/2013"));
            //TriggerProcess(DateTime.Parse("09/16/2014"));
            //TriggerProcess(DateTime.Parse("07/07/2014"));
            //System.Threading.Thread.Sleep(5000);
            //TriggerProcess(DateTime.Parse("07/29/2014"));
            //System.Threading.Thread.Sleep(5000);
            //TriggerProcess(DateTime.Parse("07/28/2014"));

            //CDRManager cdrManager = new CDRManager();
            //cdrManager.LoadCDRRange(DateTime.Parse("8/1/2014"), DateTime.Parse("1/1/2015"), 2000, (cdrs) =>
            //    {
            //        RepricingProcess process = new RepricingProcess { CDRs = cdrs };
            //        BusinessProcessRuntime.Current.CreateNewProcess(process);
            //    });



            Console.ReadKey();
        }
        
        private static void TriggerProcess(DateTime date)
        {
            TOne.CDRProcess.Arguments.DailyRepricingProcessInput inputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput { RepricingDay = date };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "DailyRepricingProcess",
                InputArguments = inputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());

        }

        static bool _isRunning;
        static object _lockObj = new object();
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }
            try
            {
                //BusinessProcessRuntime.Current.LoadAndExecutePendings();

                BusinessProcessRuntime.Current.ExecutePendings();
                BusinessProcessRuntime.Current.TriggerPendingEvents();
            }
            finally
            {
                lock (_lockObj)
                {
                    _isRunning = false;
                }
            }
        }
    }
}
