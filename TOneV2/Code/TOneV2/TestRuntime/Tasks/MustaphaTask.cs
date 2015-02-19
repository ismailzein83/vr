using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace TestRuntime.Tasks
{
    public class MustaphaTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Host Started");
            BusinessProcessRuntime.Current.TerminatePendingProcesses();

            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            int switchID = 14;

            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
            {
                TriggerProcessRawCDRsProcess(switchID);
            });
            t.Start();

            //while (true)
            //{
                

            //    System.Threading.Thread.Sleep(5000);

            //    System.Threading.Tasks.Task t2 = new System.Threading.Tasks.Task(() =>
            //    {
            //        TriggerProcess2(switchID);
            //    });
            //    t2.Start();

            //    System.Threading.Thread.Sleep(5000);

            //    System.Threading.Tasks.Task t3 = new System.Threading.Tasks.Task(() =>
            //    {
            //        TriggerProcess3(switchID);
            //    });
            //    t3.Start();

            //}

        }


        //

        private static void TriggerProcessRawCDRsProcess(int SwitchID)
        {
            TOne.CDRProcess.Arguments.RawCDRsProcessInput inputArguments = new TOne.CDRProcess.Arguments.RawCDRsProcessInput { SwitchID = SwitchID, CacheManagerId = new Guid() };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "RawCDRsProcess",
                InputArguments = inputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);
        }


        private static void TriggerProcessStoreCDRsInDB(int SwitchID)
        {
            TOne.CDRProcess.Arguments.StoreCDRsInDBProcessInput inputArguments = new TOne.CDRProcess.Arguments.StoreCDRsInDBProcessInput { SwitchID = SwitchID };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "StoreCDRsInDBProcess",
                InputArguments = inputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);

        }

        private static void TriggerProcessImportCDRProcess(int SwitchID)
        {
            TOne.CDRProcess.Arguments.ImportCDRProcessInput inputArguments = new TOne.CDRProcess.Arguments.ImportCDRProcessInput { SwitchID = SwitchID };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "ImportCDRProcess",
                InputArguments = inputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);

        }

        //private static void TriggerProcessCDRImportProcess(int SwitchID)
        //{
        //    TOne.CDRProcess.Arguments.CDRImportProcessInput inputArguments = new TOne.CDRProcess.Arguments.CDRImportProcessInput { SwitchID = SwitchID };
        //    CreateProcessInput input = new CreateProcessInput
        //    {
        //        ProcessName = "CDRImportProcess",
        //        InputArguments = inputArguments
        //    };
        //    ProcessManager processManager = new ProcessManager();
        //    processManager.CreateNewProcess(input);

        //}

        private static void TriggerProcessCDRGenerationProcess(int SwitchID)
        {
            TOne.CDRProcess.Arguments.CDRGenerationProcessInput CDRProcessInputArguments = new TOne.CDRProcess.Arguments.CDRGenerationProcessInput { SwitchID = SwitchID };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "CDRGenerationProcess",
                InputArguments = CDRProcessInputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);
        }

        private static void TriggerProcessUpdateBillingPricingProcess(int SwitchID)
        {
            TOne.CDRProcess.Arguments.UpdateBillingPricingProcessInput UpdateBillingPricingProcessInputArguments = new TOne.CDRProcess.Arguments.UpdateBillingPricingProcessInput { SwitchID = SwitchID };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "UpdateBillingPricingProcess",
                InputArguments = UpdateBillingPricingProcessInputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);
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
