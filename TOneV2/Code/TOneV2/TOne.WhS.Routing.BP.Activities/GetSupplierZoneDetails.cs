using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetSupplierZoneDetailsInput
    {
        public IEnumerable<SupplierCode> SupplierCodes { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public class GetSupplierZoneDetailsOutput
    {
        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }
    }


    public sealed class GetSupplierZoneDetails : BaseAsyncActivity<GetSupplierZoneDetailsInput, GetSupplierZoneDetailsOutput>
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierCode>> SupplierCodes { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public OutArgument<SupplierZoneDetailByZone> SupplierZoneDetails { get; set; }

        protected override GetSupplierZoneDetailsOutput DoWorkWithResult(GetSupplierZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {

            SupplierZoneDetailByZone supplierZoneDetailsByZone = null;
            
            ISupplierZoneDetailsDataManager supplierZoneDetailsManager = RoutingDataManagerFactory.GetDataManager<ISupplierZoneDetailsDataManager>();
            supplierZoneDetailsManager.DatabaseId = inputArgument.RoutingDatabaseId;

            IEnumerable<SupplierZoneDetail> supplierZoneDetails = supplierZoneDetailsManager.GetSupplierZoneDetails();

            if(supplierZoneDetails != null)
            {
                supplierZoneDetailsByZone = new SupplierZoneDetailByZone();

                foreach (SupplierCode code in inputArgument.SupplierCodes)
                {
                    if (!supplierZoneDetailsByZone.ContainsKey(code.ZoneId))
                        supplierZoneDetailsByZone.Add(code.ZoneId, supplierZoneDetails.FirstOrDefault(x => x.SupplierZoneId == code.ZoneId));
                }
            }

            return new GetSupplierZoneDetailsOutput {
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
        }
    }
}
