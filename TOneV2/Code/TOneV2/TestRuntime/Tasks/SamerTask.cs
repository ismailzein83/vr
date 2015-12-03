using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRuntime.ExecutionFlows;
using TOne.WhS.CDRProcessing.Entities;
using TOne.WhS.CDRProcessing.QueueActivators;
using TOne.WhS.CodePreparation.BP.Arguments;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    class SamerTask : ITask
    {
        public void Execute()
        {
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            //var tree = Vanrise.Common.Serializer.Serialize(queueFlowTree);
          //  var myflow = AllFlows.GetImportCDRFlow();
            //var tree1 = Vanrise.Common.Serializer.Serialize("test");
            //QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
            //var queuesByStages = executionFlowManager.GetQueuesByStages(2);
            //CDR cdr = new CDR
            //{
            //    ID=1,
            //    Name="test"
            //};
            //while (true)
            //{
            //    //Console.ReadKey();
            //    queuesByStages["Store CDR Raws"].Queue.EnqueueObject(cdr);
            //}
            





            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput
            //    {
            //        RepricingDay = DateTime.Parse("2014-03-01")
            //    }
            //});

            

            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new CodePreparationProcessInput{
            //        EffectiveDate=DateTime.Now,
            //        FileId=10,
            //        SellingNumberPlanId=1
            //    }
                
            //});

        }
    }
}
