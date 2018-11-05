using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime.Tasks
{
    public class AnthonyTask : ITask
    {
        public void Execute()
        {
            //List<EricssonConvertedRoute> convertedRoutes = new List<EricssonConvertedRoute>();
            //EricssonConvertedRoute r01 = new EricssonConvertedRoute() { BO = "1", Code = "1", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r01);
            //AddRoutes("1", convertedRoutes);
            //AddRoutes("2", convertedRoutes);
            //AddRoutes("3", convertedRoutes);
            //AddRoutes("3", convertedRoutes);

            //List<ConvertedRoute> routes = TOne.WhS.RouteSync.Business.Helper.CompressRoutesWithCodes(convertedRoutes, (context) => { return new EricssonConvertedRoute() { BO = context.Customer, Code = context.Code, RCNumber = int.Parse(context.RouteOptionIdentifier), RouteType = EricssonRouteType.BNumber }; });
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

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingExecutorRuntimeService);

            DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }

        //private static void AddRoutes(string bo, List<EricssonConvertedRoute> convertedRoutes)
        //{
        //    EricssonConvertedRoute r09 = new EricssonConvertedRoute() { BO = bo, Code = "9", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r09);

        //    //EricssonConvertedRoute r91 = new EricssonConvertedRoute() { BO = bo, Code = "91", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r91);
        //    EricssonConvertedRoute r92 = new EricssonConvertedRoute() { BO = bo, Code = "92", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r92);
        //    EricssonConvertedRoute r96 = new EricssonConvertedRoute() { BO = bo, Code = "96", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r96);

        //    EricssonConvertedRoute r910 = new EricssonConvertedRoute() { BO = bo, Code = "910", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r910);
        //    EricssonConvertedRoute r911 = new EricssonConvertedRoute() { BO = bo, Code = "911", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r911);
        //    EricssonConvertedRoute r912 = new EricssonConvertedRoute() { BO = bo, Code = "912", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r912);
        //    EricssonConvertedRoute r913 = new EricssonConvertedRoute() { BO = bo, Code = "913", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r913);
        //    EricssonConvertedRoute r914 = new EricssonConvertedRoute() { BO = bo, Code = "914", RouteType = EricssonRouteType.BNumber, RCNumber = 5 }; convertedRoutes.Add(r914);
        //    EricssonConvertedRoute r915 = new EricssonConvertedRoute() { BO = bo, Code = "915", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r915);
        //    EricssonConvertedRoute r916 = new EricssonConvertedRoute() { BO = bo, Code = "916", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r916);
        //    EricssonConvertedRoute r917 = new EricssonConvertedRoute() { BO = bo, Code = "917", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r917);
        //    EricssonConvertedRoute r918 = new EricssonConvertedRoute() { BO = bo, Code = "918", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r918);
        //    EricssonConvertedRoute r919 = new EricssonConvertedRoute() { BO = bo, Code = "919", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r919);

        //    EricssonConvertedRoute r961 = new EricssonConvertedRoute() { BO = bo, Code = "961", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r961);
        //    EricssonConvertedRoute r962 = new EricssonConvertedRoute() { BO = bo, Code = "962", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r962);
        //    EricssonConvertedRoute r963 = new EricssonConvertedRoute() { BO = bo, Code = "963", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r963);

        //    EricssonConvertedRoute r9615 = new EricssonConvertedRoute() { BO = bo, Code = "9615", RouteType = EricssonRouteType.BNumber, RCNumber = 1 }; convertedRoutes.Add(r9615);
        //    EricssonConvertedRoute r9614 = new EricssonConvertedRoute() { BO = bo, Code = "9614", RouteType = EricssonRouteType.BNumber, RCNumber = 2 }; convertedRoutes.Add(r9614);

        //    EricssonConvertedRoute r9644 = new EricssonConvertedRoute() { BO = bo, Code = "9644", RouteType = EricssonRouteType.BNumber, RCNumber = 4 }; convertedRoutes.Add(r9644);
        //    EricssonConvertedRoute r1 = new EricssonConvertedRoute() { BO = bo, Code = "1", RouteType = EricssonRouteType.BNumber, RCNumber = 4 }; convertedRoutes.Add(r1);
        //    EricssonConvertedRoute r2 = new EricssonConvertedRoute() { BO = bo, Code = "2", RouteType = EricssonRouteType.BNumber, RCNumber = 3 }; convertedRoutes.Add(r2);
        //    EricssonConvertedRoute r54 = new EricssonConvertedRoute() { BO = bo, Code = "54", RouteType = EricssonRouteType.BNumber, RCNumber = 46 }; convertedRoutes.Add(r54);
        //    EricssonConvertedRoute r87 = new EricssonConvertedRoute() { BO = bo, Code = "87", RouteType = EricssonRouteType.BNumber, RCNumber = 43 }; convertedRoutes.Add(r87);
        //}
    }
}