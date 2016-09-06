using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class GetExistingZonesServicesInput
    {
        public int SupplierId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingZonesServicesOutput
    {
        public IEnumerable<SupplierZoneService> ExistingZoneServiceEntities { get; set; }
    }

    public sealed class GetExistingZonesServices : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingZonesServicesInput, GetExistingZonesServicesOutput>
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierZoneService>> ExistingZoneServiceEntities { get; set; }

        protected override GetExistingZonesServicesOutput DoWorkWithResult(GetExistingZonesServicesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SupplierZoneServiceManager supplierZoneManager = new SupplierZoneServiceManager();
            List<SupplierZoneService> supplierZonesServices = supplierZoneManager.GetSupplierZonesServicesEffectiveAfter(inputArgument.SupplierId, inputArgument.MinimumDate);

            return new GetExistingZonesServicesOutput()
            {
                ExistingZoneServiceEntities = supplierZonesServices
            };
        }

        protected override GetExistingZonesServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingZonesServicesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SupplierId = this.SupplierId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingZonesServicesOutput result)
        {
            this.ExistingZoneServiceEntities.Set(context, result.ExistingZoneServiceEntities);
        }
    }
}
