using System;
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
        public List<CustomerRouteData> AffectedCustomerRoutes { get; set; }
    }

    public class LoadPartialCodeMatchesOutput
    {
        public List<RoutingCodeMatches> RoutingCodeMatchesList { get; set; }
    }

    public sealed class LoadPartialCodeMatches : BaseAsyncActivity<LoadPartialCodeMatchesInput, LoadPartialCodeMatchesOutput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<List<CustomerRouteData>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public OutArgument<List<RoutingCodeMatches>> RoutingCodeMatchesList { get; set; }

        protected override LoadPartialCodeMatchesOutput DoWorkWithResult(LoadPartialCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            List<CustomerRouteData> affectedCustomerRoutes = inputArgument.AffectedCustomerRoutes;

            Dictionary<string, HashSet<long>> codeSaleZones = new Dictionary<string, HashSet<long>>();

            foreach (CustomerRouteData customerRouteDefinition in affectedCustomerRoutes)
            {
                HashSet<long> saleZones = codeSaleZones.GetOrCreateItem(customerRouteDefinition.Code);
                saleZones.Add(customerRouteDefinition.SaleZoneId);
            }

            ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            dataManager.RoutingDatabase = inputArgument.RoutingDatabase;

            List<PartialCodeMatches> codeMatchesList = dataManager.GetPartialCodeMatchesByRouteCodes(codeSaleZones.Keys.ToHashSet());
            List<RoutingCodeMatches> routingCodeMatchesList = new List<RoutingCodeMatches>();

            SaleZoneManager saleZoneManager = new SaleZoneManager();

            foreach (PartialCodeMatches partialCodeMatches in codeMatchesList)
            {
                RoutingCodeMatches routingCodeMatches = new Entities.RoutingCodeMatches()
                {
                    Code = partialCodeMatches.Code,
                    //CodePrefix = partialCodeMatches.CodePrefix,
                    SupplierCodeMatches = partialCodeMatches.SupplierCodeMatches,
                    SupplierCodeMatchesBySupplier = partialCodeMatches.SupplierCodeMatchesBySupplier,
                    SaleZoneDefintions = new List<SaleZoneDefintion>()
                };
                HashSet<long> saleZones = codeSaleZones.GetOrCreateItem(partialCodeMatches.Code);
                foreach (long saleZoneId in saleZones)
                {
                    SaleZone saleZone = saleZoneManager.GetSaleZone(saleZoneId);
                    routingCodeMatches.SaleZoneDefintions.Add(new SaleZoneDefintion() { SaleZoneId = saleZoneId, SellingNumberPlanId = saleZone.SellingNumberPlanId });
                }

                routingCodeMatchesList.Add(routingCodeMatches);
            }

            return new LoadPartialCodeMatchesOutput()
            {
                RoutingCodeMatchesList = routingCodeMatchesList
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
            this.RoutingCodeMatchesList.Set(context, result.RoutingCodeMatchesList);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Loading Code Matches is done", null);
        }
    }
}