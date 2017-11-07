﻿using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class LoadPartialCodeMatchesInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
        public List<CustomerRoute> AffectedCustomerRoutes { get; set; }
    }

    public class LoadPartialCodeMatchesOutput
    {
        public BaseQueue<RoutingCodeMatches> CodeMatchesQueue { get; set; }
    }

    public sealed class LoadPartialCodeMatches : BaseAsyncActivity<LoadPartialCodeMatchesInput, LoadPartialCodeMatchesOutput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<List<CustomerRoute>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public OutArgument<BaseQueue<RoutingCodeMatches>> CodeMatchesQueue { get; set; }

        protected override LoadPartialCodeMatchesOutput DoWorkWithResult(LoadPartialCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            List<CustomerRoute> affectedCustomerRoutes = inputArgument.AffectedCustomerRoutes;

            Dictionary<string, HashSet<long>> codeSaleZones = new Dictionary<string, HashSet<long>>();

            foreach (CustomerRoute customerRoute in affectedCustomerRoutes)
            {
                HashSet<long> saleZones = codeSaleZones.GetOrCreateItem(customerRoute.Code);
                saleZones.Add(customerRoute.SaleZoneId);
            }

            ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            dataManager.RoutingDatabase = inputArgument.RoutingDatabase;

            List<PartialCodeMatches> codeMatchesList = dataManager.GetPartialCodeMatchesByRouteCodes(codeSaleZones.Keys.ToHashSet());
            MemoryQueue<RoutingCodeMatches> queue = new MemoryQueue<RoutingCodeMatches>();

            SaleZoneManager saleZoneManager = new SaleZoneManager();

            foreach (PartialCodeMatches codeMatches in codeMatchesList)
            {
                RoutingCodeMatches routingCodeMatches = new Entities.RoutingCodeMatches()
                {
                    Code = codeMatches.Code,
                    CodePrefix = codeMatches.CodePrefix,
                    SupplierCodeMatches = codeMatches.SupplierCodeMatches,
                    SupplierCodeMatchesBySupplier = codeMatches.SupplierCodeMatchesBySupplier,
                    SaleZoneDefintions = new List<SaleZoneDefintion>()
                };
                HashSet<long> saleZones = codeSaleZones.GetOrCreateItem(codeMatches.Code);
                foreach (long saleZoneId in saleZones)
                {
                    SaleZone saleZone = saleZoneManager.GetSaleZone(saleZoneId);
                    routingCodeMatches.SaleZoneDefintions.Add(new SaleZoneDefintion() { SaleZoneId = saleZoneId, SellingNumberPlanId = saleZone.SellingNumberPlanId });
                }

                queue.Enqueue(routingCodeMatches);
            }

            return new LoadPartialCodeMatchesOutput()
            {
                CodeMatchesQueue = queue
            };
        }

        protected override LoadPartialCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadPartialCodeMatchesInput()
            {
                AffectedCustomerRoutes = this.AffectedCustomerRoutes.Get(context),
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadPartialCodeMatchesOutput result)
        {
            this.CodeMatchesQueue.Set(context, result.CodeMatchesQueue);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Loading Code Matches is done", null);
        }
    }
}