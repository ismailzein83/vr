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



            BPClient bpClient3 = new BPClient();
            bpClient3.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "ExecuteStrategyProcess",
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    StrategyId = 1,
                    FromDate =    new DateTime(2014,12,1),
                    ToDate = new DateTime(2014,12,2),
                    PeriodId = (int) Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
                }
            });


            //NumberProfileDataManager x = new NumberProfileDataManager();
            //x.LoadNormalCDR(DateTime.Parse("2015-03-10 04:00:00"), DateTime.Parse("2015-03-20 06:00:00"), (int) Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Day);




            Console.WriteLine("END");
            Console.ReadKey();
        }

    }
}
