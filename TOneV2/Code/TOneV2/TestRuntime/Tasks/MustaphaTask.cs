using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class MustaphaTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Host Started");

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            int switchID = 8;
            Console.ReadKey();
            TriggerProcessImportCDRProcess(switchID);
            
            
            //TriggerProcessStoreCDRsInDB(switchID);

            //TriggerProcessRawCDRsProcess(switchID);
             //TriggerBillingCDRsProcess(switchID);
            //TriggerStoreInvalidCDRsInDBProcess(switchID);
            //TriggerStoreMainCDRsInDBProcess(switchID);

          //  GenerateStatisticsProcess(switchID);
            
           // SaveStatisticsToDBProcess(switchID);
            
            
          //  GenerateDailyStatisticsProcess(switchID);
           // SaveDailyStatisticsToDBProcess(switchID);
        }

        private static void SaveDailyStatisticsToDBProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "SaveDailyStatisticsToDBProcess",
                InputArguments = new TOne.CDRProcess.Arguments.SaveDailyStatisticsToDBProcessInput
                {
                    SwitchID = switchID
                }
            });
        }


        private static void GenerateDailyStatisticsProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "GenerateDailyStatisticsProcess",
                InputArguments = new TOne.CDRProcess.Arguments.GenerateDailyStatisticsProcessInput
                {
                    SwitchID = switchID
                }
            });
        }

        private static void SaveStatisticsToDBProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "SaveStatisticsToDBProcess",
                InputArguments = new TOne.CDRProcess.Arguments.SaveStatisticsToDBProcessInput
                {
                    SwitchID = switchID
                }
            });
        }

        private static void TriggerProcessImportCDRProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ImportCDRProcess",
                InputArguments = new TOne.CDRProcess.Arguments.ImportCDRProcessInput
                {
                    SwitchID = switchID
                }
            });
        }

        private static void TriggerProcessStoreCDRsInDB(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "StoreCDRsInDBProcess",
                InputArguments = new TOne.CDRProcess.Arguments.StoreCDRsInDBProcessInput
                {
                    SwitchID = switchID
                }
            });
        }

        private static void TriggerProcessRawCDRsProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "RawCDRsProcess",
                InputArguments = new TOne.CDRProcess.Arguments.RawCDRsProcessInput
                {
                    SwitchID = switchID,
                    CacheManagerId = new Guid() 
                }
            });

        }

        private static void TriggerBillingCDRsProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "BillingCDRsProcess",
                InputArguments = new TOne.CDRProcess.Arguments.BillingCDRsProcessInput
                {
                    SwitchID = switchID,
                    CacheManagerId = new Guid()
                }
            });
        }

        private static void TriggerStoreInvalidCDRsInDBProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "StoreInvalidCDRsInDBProcess",
                InputArguments = new TOne.CDRProcess.Arguments.StoreInvalidCDRsInDBProcessInput
                {
                    SwitchID = switchID
                }
            });
        }

        private static void TriggerStoreMainCDRsInDBProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "StoreMainCDRsInDBProcess",
                InputArguments = new TOne.CDRProcess.Arguments.StoreMainCDRsInDBProcessInput
                {
                    SwitchID = switchID
                }
            });
        }

        private static void GenerateStatisticsProcess(int switchID)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "GenerateStatisticsProcess",
                InputArguments = new TOne.CDRProcess.Arguments.GenerateStatisticsProcessInput
                {
                    SwitchID = switchID
                }
            });
        }

        //private static void TriggerStoreMainCDRsInDBProcess(int SwitchID)
        //{
        //    TOne.CDRProcess.Arguments.StoreMainCDRsInDBProcessInput inputArguments = new TOne.CDRProcess.Arguments.StoreMainCDRsInDBProcessInput { SwitchID = SwitchID };
        //    CreateProcessInput input = new CreateProcessInput
        //    {
        //        ProcessName = "StoreMainCDRsInDBProcess",
        //        InputArguments = inputArguments
        //    };
        //    ProcessManager processManager = new ProcessManager();
        //    processManager.CreateNewProcess(input);
        //}


        //private static void TriggerProcessCDRGenerationProcess(int SwitchID)
        //{
        //    TOne.CDRProcess.Arguments.CDRGenerationProcessInput CDRProcessInputArguments = new TOne.CDRProcess.Arguments.CDRGenerationProcessInput { SwitchID = SwitchID };
        //    CreateProcessInput input = new CreateProcessInput
        //    {
        //        ProcessName = "CDRGenerationProcess",
        //        InputArguments = CDRProcessInputArguments
        //    };
        //    ProcessManager processManager = new ProcessManager();
        //    processManager.CreateNewProcess(input);
        //}

        //private static void TriggerProcessUpdateBillingPricingProcess(int SwitchID)
        //{
        //    TOne.CDRProcess.Arguments.UpdateBillingPricingProcessInput UpdateBillingPricingProcessInputArguments = new TOne.CDRProcess.Arguments.UpdateBillingPricingProcessInput { SwitchID = SwitchID };
        //    CreateProcessInput input = new CreateProcessInput
        //    {
        //        ProcessName = "UpdateBillingPricingProcess",
        //        InputArguments = UpdateBillingPricingProcessInputArguments
        //    };
        //    ProcessManager processManager = new ProcessManager();
        //    processManager.CreateNewProcess(input);
        //}

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
