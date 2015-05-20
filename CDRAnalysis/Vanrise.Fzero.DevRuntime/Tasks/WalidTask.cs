using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Fzero.FraudAnalysis.Data.MySQL;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class WalidTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            //PersistentQueue<ImportedCDRBatch> queue =  Vanrise.Fzero.CDRImport.Business.CDRQueueFactory.GetImportedCDRQueue("DS1");

            //String fileName = @"C:\Users\mustafa.hawi\Desktop\x.Dat";

          //  queue.Enqueue(new ImportedCDRBatch()
           // {
                //file =File.ReadAllBytes(fileName)
          //  });



            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "CDRImportProcess",
            //    InputArguments = new Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput
            //    {
            //    }
            //});





            //BPClient bpClient2 = new BPClient();
            //bpClient2.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "SaveCDRToDBProcess",
            //    InputArguments = new Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput
            //    {
            //    }
            //});


            List<int> StrategyIds = new List<int>();
            StrategyIds.Add(1);

            BPClient bpClient3 = new BPClient();

            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIds,
                    FromDate = new DateTime(2015, 3, 13, 23, 0, 0),
                    ToDate = new DateTime(2015, 3, 13, 23, 59, 59),
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });


            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIds,
                    FromDate = new DateTime(2015, 3, 14, 0, 0, 0),
                    ToDate = new DateTime(2015, 3, 14, 0, 59, 59),
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });



            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIds,
                    FromDate = new DateTime(2015, 3, 14, 1, 0, 0),
                    ToDate = new DateTime(2015, 3, 14, 1, 59, 59),
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });

            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIds,
                    FromDate = new DateTime(2015, 3, 14, 2, 0, 0),
                    ToDate = new DateTime(2015, 3, 14, 2, 59, 59),
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });



            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIds,
                    FromDate = new DateTime(2015, 3, 14, 3, 0, 0),//2015-03-13 23:01:12.000
                    ToDate = new DateTime(2015, 3, 14, 3, 59, 59),//2015-03-14 06:11:42.000
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });

            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIds,
                    FromDate = new DateTime(2015, 3, 14, 4, 0, 0),//2015-03-13 23:01:12.000
                    ToDate = new DateTime(2015, 3, 14, 4, 59, 59),//2015-03-14 06:11:42.000
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });

            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIds,
                    FromDate = new DateTime(2015, 3, 14, 5, 0, 0),//2015-03-13 23:01:12.000
                    ToDate = new DateTime(2015, 3, 14, 5, 59, 59),//2015-03-14 06:11:42.000
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });


           









            List<int> StrategyIdsDaily = new List<int>();
            StrategyIdsDaily.Add(35);


            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIdsDaily,
                    FromDate = new DateTime(2015, 3, 13, 23, 0, 0),//2015-03-13 23:01:12.000
                    ToDate = new DateTime(2015, 3, 13, 23, 59, 59),//2015-03-14 06:11:42.000
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Day
                }
            });


            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyIds = StrategyIdsDaily,
                    FromDate = new DateTime(2015, 3, 14, 0, 0, 0),//2015-03-13 23:01:12.000
                    ToDate = new DateTime(2015, 3, 14, 5, 59, 59),//2015-03-14 06:11:42.000
                    PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Day
                }
            });


          



            Console.WriteLine("END");
            Console.ReadKey();
        }

    }
}
