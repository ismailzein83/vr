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

    public class LoadSupplierZoneDetailsInput
    {
        public IEnumerable<SupplierCode> SupplierCodes { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public class LoadSupplierZoneDetailsOutput
    {
        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }
    }


    public sealed class LoadSupplierZoneDetails : BaseAsyncActivity<LoadSupplierZoneDetailsInput, LoadSupplierZoneDetailsOutput>
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierCode>> SupplierCodes { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public OutArgument<SupplierZoneDetailByZone> SupplierZoneDetails { get; set; }

        protected override LoadSupplierZoneDetailsOutput DoWorkWithResult(LoadSupplierZoneDetailsInput inputArgument, AsyncActivityHandle handle)
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

            return new LoadSupplierZoneDetailsOutput {
                SupplierZoneDetails = supplierZoneDetailsByZone
            };
        }

        protected override LoadSupplierZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadSupplierZoneDetailsInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                SupplierCodes = this.SupplierCodes.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadSupplierZoneDetailsOutput result)
        {
            this.SupplierZoneDetails.Set(context, result.SupplierZoneDetails);
        }
    }
}
