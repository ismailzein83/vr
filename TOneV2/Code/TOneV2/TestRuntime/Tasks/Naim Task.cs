using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.MainExtensions;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using System.Linq;
using Vanrise.Common;
using Vanrise.BusinessProcess.MainExtensions.BPTaskTypes;

namespace TestRuntime.Tasks
{

    public class NaimTask : ITask
    {
        public void Execute()
        {
            var analyticManager = new Vanrise.Analytic.Business.AnalyticManager();
            var analyticQuery = new Vanrise.Analytic.Entities.AnalyticQuery
            {
                TableId = new Guid("58dd0497-498d-40f2-8687-08f8356c63cc"),
                FromTime = DateTime.Parse("2010-01-01"),
                DimensionFields = new List<string> { "Customer", "Supplier" },
                MeasureFields = new List<string> { "CostNet" },
                WithSummary = true
            };
            Vanrise.Analytic.Entities.AnalyticRecord summaryRecord;
            List<Vanrise.Analytic.Entities.AnalyticResultSubTable> resultSubTables;
            var rslt = analyticManager.GetAllFilteredRecords(analyticQuery,false,false, out summaryRecord, out resultSubTables);
            string serializedResultSubTables = Serializer.Serialize(resultSubTables);
            string serializedRslt = Serializer.Serialize(rslt);

            //List<AnalyticCustomRecord> customRecords = new List<AnalyticCustomRecord>();
            //foreach (var record in rslt)
            //{
            //    var customRecord = new AnalyticCustomRecord
            //    {
            //        CountCDRs = (int)record.MeasureValues["CountCDRs"].Value,
            //        TotalDuration = (decimal)record.MeasureValues["TotalDuration"].Value,
            //        CalculatedCountCDRs = record.SubTables[0].MeasureValues.Sum(itm => (int)itm["CountCDRs"].Value),
            //        CalculatedTotalDuration = record.SubTables[0].MeasureValues.Sum(itm => (decimal)itm["TotalDuration"].Value)
            //    };
            //    if (customRecord.CountCDRs != customRecord.CalculatedCountCDRs || customRecord.TotalDuration - customRecord.CalculatedTotalDuration > 0.000000000001M)
            //        throw new Exception("Invalid SubTables Measures");
            //    customRecords.Add(customRecord);
            //}
            //string serializedCustomRecords = Serializer.Serialize(customRecords);

            //if (summaryRecord != null)
            //{
            //    string serializedSummary = Serializer.Serialize(summaryRecord);

            //    List<AnalyticCustomRecord> customVerticalRecords = new List<AnalyticCustomRecord>();
            //    int colIndex = 0;
            //    foreach (var subTableSummaryMeasures in summaryRecord.SubTables[0].MeasureValues)
            //    {
            //        var customRecord = new AnalyticCustomRecord
            //        {
            //            CountCDRs = (int)subTableSummaryMeasures["CountCDRs"].Value,
            //            TotalDuration = (decimal)subTableSummaryMeasures["TotalDuration"].Value,
            //            CalculatedCountCDRs = rslt.Sum(record => (int)record.SubTables[0].MeasureValues[colIndex]["CountCDRs"].Value),
            //            CalculatedTotalDuration = rslt.Sum(record => (decimal)record.SubTables[0].MeasureValues[colIndex]["TotalDuration"].Value),
            //        };
            //        if (customRecord.CountCDRs != customRecord.CalculatedCountCDRs || customRecord.TotalDuration - customRecord.CalculatedTotalDuration > 0.000000000001M)
            //            throw new Exception("Invalid SubTables Measures");
            //        customVerticalRecords.Add(customRecord);
            //        colIndex++;
            //    }
            //    string serializedCustomVerticalRecords = Serializer.Serialize(customVerticalRecords);
            //}



            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            //QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(queueRegulatorService);

            //QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(queueActivationService);

            //SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(summaryQueueActivationService);

            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            //Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bigDataService);

            //Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            //CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingRuntimeService);

            //CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingDistributorRuntimeService);

            //DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(dataGroupingExecutorRuntimeService);

            //DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }
        Action<int> BuildAction(ref int A, ref int B)
        {
            int a = A;
            int b = B;

            a++;
            b++;

            Action<int> action = i =>
            {
                a++;
                b++;
            };

            A = a;
            B = b;
            return action;
        }

    }
}