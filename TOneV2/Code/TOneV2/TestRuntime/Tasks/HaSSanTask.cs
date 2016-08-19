//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;
//using Vanrise.BusinessProcess;
//using Vanrise.BusinessProcess.Business;
//using Vanrise.BusinessProcess.Entities;

//namespace TestRuntime
//{
//    public class HaSSanTask : ITask
//    {
//        public void Execute()
//        {

//            var config = new BPConfiguration { MaxConcurrentWorkflows = 20 };
//            var ser = Vanrise.Common.Serializer.Serialize(config);
//            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


//            //Console.WriteLine("Host Started");
//            //BusinessProcessRuntime.Current.TerminatePendingProcesses();

//            //Timer timer = new Timer(1000);
//            //timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
//            //timer.Start();
//            //int switchID = 66;
//            //System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
//            //{
//            //    TriggerProcessImportCDRProcess(switchID);
//            //    // System.Threading.Thread.Sleep(30000);
//            //});
//            //t.Start();


         
//            BusinessProcessRuntime.Current.TerminatePendingProcesses();
//            Timer timer = new Timer(1000);
//            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
//            timer.Start();

//            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
//                {
//                    for (DateTime d = DateTime.Parse(ConfigurationManager.AppSettings["RepricingFrom"]); d <= DateTime.Parse(ConfigurationManager.AppSettings["RepricingTo"]); d = d.AddDays(1))
//                    {
//                        TriggerProcess(d);
//                        System.Threading.Thread.Sleep(30000);
//                    }
//                });
//            t.Start();


//            Console.ReadKey();
//        }

//        private static void TriggerProcess(DateTime date)
//        {
//            TOne.CDRProcess.Arguments.DailyRepricingProcessInput inputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput { RepricingDay = date };
//            CreateProcessInput input = new CreateProcessInput
//            {
//                InputArguments = inputArguments
//            };
//            BPInstanceManager processManager = new BPInstanceManager();
//            processManager.CreateNewProcess(input);
//        }


//        private static void TriggerProcessImportCDRProcess(int SwitchID)
//        {
//            TOne.CDRProcess.Arguments.ImportCDRProcessInput inputArguments = new TOne.CDRProcess.Arguments.ImportCDRProcessInput { SwitchID = SwitchID };
//            CreateProcessInput input = new CreateProcessInput
//            {
//                InputArguments = inputArguments
//            };
//            BPInstanceManager processManager = new BPInstanceManager();
//            processManager.CreateNewProcess(input);

//        }

//        private static void TriggerProcess(int SwitchID)
//        {
//            TOne.CDRProcess.Arguments.CDRImportProcessInput inputArguments = new TOne.CDRProcess.Arguments.CDRImportProcessInput { SwitchID = SwitchID };
//            CreateProcessInput input = new CreateProcessInput
//            {
//                InputArguments = inputArguments
//            };
//            BPInstanceManager processManager = new BPInstanceManager();
//            processManager.CreateNewProcess(input);

//            TOne.CDRProcess.Arguments.CDRGenerationProcessInput CDRProcessInputArguments = new TOne.CDRProcess.Arguments.CDRGenerationProcessInput { SwitchID = SwitchID };
//            CreateProcessInput inputCDRProcess = new CreateProcessInput
//            {
//                InputArguments = CDRProcessInputArguments
//            };
//            BPInstanceManager processManager1 = new BPInstanceManager();
//            processManager1.CreateNewProcess(inputCDRProcess);
//        }



//        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
//        {
//            System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());

//        }

//        static bool _isRunning;
//        static object _lockObj = new object();
//        static void timer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            lock (_lockObj)
//            {
//                if (_isRunning)
//                    return;
//                _isRunning = true;
//            }
//            try
//            {
//                //BusinessProcessRuntime.Current.LoadAndExecutePendings();

//                BusinessProcessRuntime.Current.ExecutePendings();
//                BusinessProcessRuntime.Current.TriggerPendingEvents();
//            }
//            finally
//            {
//                lock (_lockObj)
//                {
//                    _isRunning = false;
//                }
//            }
//        }
//    }


//}










