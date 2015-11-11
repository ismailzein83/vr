using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{

    public class LoadSupplierZoneDetailsInput
    {
        public IEnumerable<SupplierCode> SupplierCodes { get; set; }
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
        public OutArgument<SupplierZoneDetailByZone> SupplierZoneDetails { get; set; }

        protected override LoadSupplierZoneDetailsOutput DoWorkWithResult(LoadSupplierZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {

            SupplierZoneDetailByZone supplierZoneDetails = new SupplierZoneDetailByZone();

            foreach (SupplierCode code in inputArgument.SupplierCodes)
            {
                if(!supplierZoneDetails.ContainsKey(code.ZoneId))
                {
                    SupplierZoneDetail zoneDetail = new SupplierZoneDetail()
                    {
                        SupplierId = 1,
                        SupplierZoneId = code.ZoneId,
                        EffectiveRateValue = 12
                    };

                    supplierZoneDetails.Add(code.ZoneId, zoneDetail);
                }
            }

            return new LoadSupplierZoneDetailsOutput {
                SupplierZoneDetails = supplierZoneDetails
            };
        }

        protected override LoadSupplierZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadSupplierZoneDetailsInput
            {
                SupplierCodes = this.SupplierCodes.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadSupplierZoneDetailsOutput result)
        {
            this.SupplierZoneDetails.Set(context, result.SupplierZoneDetails);
        }
    }
}
