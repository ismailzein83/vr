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

    public class GetExistingZonesInput
    {
        public int SupplierId { get; set; }

        public DateTime MinimumDate { get; set; }
    }
    public class GetExistingZonesOutput
    {
        public IEnumerable<SupplierZone> ExistingZoneEntities { get; set; }
    }

    public sealed class GetExistingZones : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingZonesInput, GetExistingZonesOutput>
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierZone>> ExistingZoneEntities { get; set; }

        protected override GetExistingZonesOutput DoWorkWithResult(GetExistingZonesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            List<SupplierZone> suppZones = supplierZoneManager.GetSupplierZonesEffectiveAfter(inputArgument.SupplierId, inputArgument.MinimumDate);

            return new GetExistingZonesOutput()
            {
                ExistingZoneEntities = suppZones
            };
        }

        protected override GetExistingZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingZonesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SupplierId = this.SupplierId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingZonesOutput result)
        {
            this.ExistingZoneEntities.Set(context, result.ExistingZoneEntities);
        }
    }
}
