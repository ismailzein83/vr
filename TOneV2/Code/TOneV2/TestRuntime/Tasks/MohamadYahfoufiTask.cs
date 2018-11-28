using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime
{
    public class MohamadYahfoufiTask : ITask
    {
        public void TestRDB()
        {
            //  SaleCodeManager salecode = new SaleCodeManager();

            SaleCodeQueryByZone query = new SaleCodeQueryByZone
            {
                EffectiveOn = DateTime.Now,
                ZoneId = 104270
            };


            SaleCodeQuery saleCodeQuery = new SaleCodeQuery
            {
                SellingNumberPlanId = 16,

                ZonesIds = new List<long>()
                {
                    104270,104271,19210,1238,1245,16146,11759
                },

                Code= "961",
                EffectiveOn= DateTime.Today,
                GetEffectiveAfter=true

            };  

            ISaleCodeDataManager manager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();


            var codeGroupIds = new List<int>()
            {
                961,962
            };

            var getByCodeGroups = manager.GetSaleCodesByCodeGroups(codeGroupIds);
          //  var q = manager.GetSaleCodesByCodeId(codes);
         // var ew = manager.GetSaleCodesByPrefix("961",DateTime.Today,true,)
            var t = manager.GetFilteredSaleCodes(saleCodeQuery);

            var a= manager.GetSaleCodesEffectiveAfter(16,DateTime.Today, null);

            var x = manager.GetParentsByPlan(16,"52010");
            //var saleCodesByCountry = manager.GetSaleCodesByCountry(213, DateTime.Now);
            var y = manager.GetSaleCodesByZoneName(16, "Mexico - Test", DateTime.Now);
            var saleCodes = manager.GetSaleCodes(DateTime.Now); //383214 by RDB.. to be Checked by sql

            //var saleCodesbyZoneID = manager.GetSaleCodesByZoneID(104270, DateTime.Now);//263953

            //var allSaleCodes = manager.GetAllSaleCodes();//Count = 389095
            //var codesByCode= manager.GetSaleCodesByCode("961");//Count = 1887

            //var codesByZone = manager.GetSaleCodesByZone(query); //263953
        }
        public void Execute()
        {
            TestRDB();
            return;
            #region Runtime

            //var Swapdeals = new SwapDealManager().GetSwapDealsEffectiveAfterDate(new DateTime(2017, 07, 01));
            //foreach (var dealDefinition in Swapdeals)
            //{
            //    var settings = (SwapDealSettings)dealDefinition.Settings;
            //    DealGetRoutingZoneGroupsContext context = new DealGetRoutingZoneGroupsContext();
            //    settings.GetRoutingZoneGroups(context);
            //}

            //var volumes = new VolCommitmentDealManager().GetCachedVolCommitmentDeals();
            //foreach (var volume in volumes)
            //{
            //    var settings = (VolCommitmentDealSettings)volume.Settings;
            //    DealGetRoutingZoneGroupsContext context = new DealGetRoutingZoneGroupsContext();
            //    settings.GetRoutingZoneGroups(context);
            //}
            //return;

            //var runtimeServices = new List<RuntimeService>();

            //BusinessProcessService bpService = new BusinessProcessService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bpService);

            //BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bpRegulatorRuntimeService);

            //SchedulerService schedulerService = new SchedulerService { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            //RuntimeHost host = new RuntimeHost(runtimeServices);
            //host.Start();
            //Console.ReadKey();


            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            //Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            //DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(dataGroupingExecutorRuntimeService);

            //DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
            #endregion
        }
    }
}


