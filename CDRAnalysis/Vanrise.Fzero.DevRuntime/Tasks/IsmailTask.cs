using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Integration.Mappers;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            //TestLoadCDRs();
            //return;
            //Console.ReadKey();
            Console.WriteLine("Ismail Task started");
           
            var runtimeServices = new List<RuntimeService>();
           
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorService);
            //DataGroupingDistributorRuntimeService dataGroupingDistributorService = new DataGroupingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(dataGroupingDistributorService);
            //DataGroupingExecutorRuntimeService dataGroupingExecutorService = new DataGroupingExecutorRuntimeService { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(dataGroupingExecutorService);
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);
            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 5) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            
         

            runtimeServices.Add(bpService);

            runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

        



            //BPClient bpClient = new BPClient();
            //int strategyId = int.Parse(Console.ReadLine());
            //Console.WriteLine("Strategy: {0}", strategyId);
            //Console.WriteLine("Press any key to start...");
            //Console.ReadKey();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
            //    {
            //        FromDate = DateTime.Parse("2014-01-01"),
            //        ToDate = DateTime.Parse("2014-01-02"),
            //        StrategyIds = new List<int> { strategyId},//22, 23, 26 },//, 27, 28, 29 },//3, 13, 14, 15, 16 },//hourly
            //        //StrategyIds = new List<int> { 2, 4, 5, 6, 7, 8, 9, 10, 11, 12 },//daily
            //    }
            //});
 
            //Console.ReadKey();
        }

        private static void TestLoadCDRs()
        {
            while (true)
            {
                DateTime current = DateTime.Now;
                //List<Vanrise.Fzero.CDRImport.Entities.CDR> cdrs = new List<CDRImport.Entities.CDR>();
                //for (int i = 0; i < 10000; i++)
                //{
                //    cdrs.Add(new Vanrise.Fzero.CDRImport.Entities.CDR
                //    {
                //        ConnectDateTime = current,
                //        MSISDN = i.ToString()
                //    });
                //    current = current.AddSeconds(-1);
                //}
                DateTime start = DateTime.Now;
                Vanrise.Fzero.CDRImport.Data.ICDRDataManager dataManager = Vanrise.Fzero.CDRImport.Data.CDRDataManagerFactory.GetDataManager<Vanrise.Fzero.CDRImport.Data.ICDRDataManager>();
                int count = 0;
                int index = 0;
                dataManager.LoadCDR("120", DateTime.Today.AddDays(-10), DateTime.Today.AddDays(1), null, (cdr) =>
                {
                    count++;
                    index++;
                    if (index == 1000)
                    {
                        Console.WriteLine("{0} loaded", count);
                        index = 0;
                    }
                });
                //dataManager.SaveCDRsToDB(cdrs);
                Console.WriteLine("DONE in {0}", (DateTime.Now - start));
                System.Threading.Thread.Sleep(2000);
            }
            Console.ReadKey();
            return;
            //    Vanrise.Fzero.CDRImport.Data.SQL.PartitionedCDRDataManagerFactory.GetCDRDataManager<Vanrise.Fzero.CDRImport.Data.SQL.PartitionedCDRDataManager>(DateTime.Now);
            //Vanrise.Fzero.CDRImport.Data.SQL.PartitionedCDRDataManagerFactory.GetCDRDataManager<Vanrise.Fzero.CDRImport.Data.SQL.PartitionedCDRDataManager>(DateTime.Now.AddDays(2));

            //Vanrise.Fzero.CDRImport.Data.SQL.PartitionedCDRDataManagerFactory.GetCDRDataManager<Vanrise.Fzero.CDRImport.Data.SQL.PartitionedCDRDataManager>(DateTime.Now.AddDays(3));













        }
    }

}




