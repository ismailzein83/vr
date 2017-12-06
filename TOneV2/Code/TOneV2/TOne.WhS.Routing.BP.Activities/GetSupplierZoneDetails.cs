using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetSupplierZoneDetailsInput
    {
        public IEnumerable<CodePrefixSupplierCodes> SupplierCodes { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public class GetSupplierZoneDetailsOutput
    {
        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }
    }

    public sealed class GetSupplierZoneDetails : BaseAsyncActivity<GetSupplierZoneDetailsInput, GetSupplierZoneDetailsOutput>
    {
        public InArgument<IEnumerable<CodePrefixSupplierCodes>> SupplierCodes { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public OutArgument<SupplierZoneDetailByZone> SupplierZoneDetails { get; set; }

        protected override GetSupplierZoneDetailsOutput DoWorkWithResult(GetSupplierZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierZoneDetailByZone supplierZoneDetailsByZone = null;

            ISupplierZoneDetailsDataManager supplierZoneDetailsManager = RoutingDataManagerFactory.GetDataManager<ISupplierZoneDetailsDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            supplierZoneDetailsManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            IEnumerable<SupplierZoneDetail> supplierZoneDetails;
            
            if (inputArgument.SupplierCodes != null)
                supplierZoneDetails = supplierZoneDetailsManager.GetFilteredSupplierZoneDetailsBySupplierZone(inputArgument.SupplierCodes.SelectMany(itm => itm.SupplierCodes).Select(itm => itm.ZoneId).Distinct());
            else
                supplierZoneDetails = supplierZoneDetailsManager.GetSupplierZoneDetails();

            if (supplierZoneDetails != null)
            {
                supplierZoneDetailsByZone = new SupplierZoneDetailByZone();

                foreach (SupplierZoneDetail item in supplierZoneDetails)
                {
                    if (!supplierZoneDetailsByZone.ContainsKey(item.SupplierZoneId))
                        supplierZoneDetailsByZone.Add(item.SupplierZoneId, item);
                }
            }

            return new GetSupplierZoneDetailsOutput
            {
                SupplierZoneDetails = supplierZoneDetailsByZone
            };
        }

        protected override GetSupplierZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSupplierZoneDetailsInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                SupplierCodes = this.SupplierCodes.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetSupplierZoneDetailsOutput result)
        {
            this.SupplierZoneDetails.Set(context, result.SupplierZoneDetails);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Supplier Zone Details is done", null);
        }
    }
}
