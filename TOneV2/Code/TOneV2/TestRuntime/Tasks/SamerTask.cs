﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRuntime.ExecutionFlows;
using TOne.WhS.CodePreparation.BP.Arguments;
using TOne.WhS.Invoice.Business;
using TOne.WhS.Invoice.Business.Extensions;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    class SamerTask : ITask
    {
        public void Execute()
        {

            //VREventHandler eventHandler = new VREventHandler
            //{
            //   Name = "InvoiceCarrierAccountStatusChanged",
            //   BED = new DateTime(2001,10,10),
            //   EED = null,
            //   VREventHandlerId = Guid.NewGuid(),
            //   Settings = new VREventHandlerSettings
            //   {
            //       ExtendedSettings = new InvoiceCarrierAccountStatusChangedHandler()
            //   }
            //};
            //var test5 = Vanrise.Common.Serializer.Serialize(eventHandler);
            //Vanrise.Common.Serializer.Serialize(eventHandler);
          //  System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            //BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            //QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };

            //var runtimeServices = new List<RuntimeService>();
            //runtimeServices.Add(queueActivationService);

            //runtimeServices.Add(bpService);

            //runtimeServices.Add(schedulerService);

            //RuntimeHost host = new RuntimeHost(runtimeServices);
            //host.Start();

            ////var tree = Vanrise.Common.Serializer.Serialize(queueFlowTree);
            //var myflow = AllFlows.GetImportCDRFlow();
            //var tree1 = Vanrise.Common.Serializer.Serialize("test");

            //AnalyticDimensionConfig AnalyticDimensionConfig = new AnalyticDimensionConfig()
            //{
            //    FieldType = new FieldTextType(),
            //    //GroupByColumns = new List<string>() { "ant.SaleZoneID", "salz.Name" },
            //    IdColumn = "ISNULL(ant.SaleZoneID,'N/A')",
            //    JoinConfigNames = new List<string>() { "SaleZoneJoin" },
            //    NameColumn = "salz.Name"

            //};
            //AnalyticDimensionConfig AnalyticDimensionConfig1 = new AnalyticDimensionConfig()
            //{
            //    FieldType = new FieldTextType(),
            //    //GroupByColumns = new List<string>() { "ant.SupplierZoneID", "suppz.Name" },
            //    IdColumn = "ISNULL(ant.SupplierZoneID,'N/A')",
            //    JoinConfigNames = new List<string>() { "SupplierZoneJoin" },
            //    NameColumn = "suppz.Name"

            //};
            //var test = Vanrise.Common.Serializer.Serialize(AnalyticDimensionConfig);
            //var test1 = Vanrise.Common.Serializer.Serialize(AnalyticDimensionConfig1);

            var runtimeServices = new List<Vanrise.Runtime.RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
           
            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);
            runtimeServices.Add(queueActivationRuntimeService);
            runtimeServices.Add(bpRegulatorService);
            runtimeServices.Add(queueRegulatorService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);
            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();



            //AnalyticMeasure AnalyticMeasure = new AnalyticMeasure()
            //{
            //    AnalyticMeasureConfigId = 2,
            //    Config = new AnalyticMeasureConfig()
            //    {
            //        JoinConfigNames = null,
            //        //GetSQLExpressionMethod = "",
            //        //SQLExpression = "Sum(ant.DeliveredAttempts)",
            //        //SummaryFunction = AnalyticSummaryFunction.Sum
            //    }
            //};
            //var test5 = Vanrise.Common.Serializer.Serialize(AnalyticMeasure);
            //Vanrise.Common.Serializer.Serialize(AnalyticMeasure);
               
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
