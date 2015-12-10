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

    public sealed class GetExistingZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierZone>> ExistingZoneEntities { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int supplierId = context.GetValue(this.SupplierId);
            DateTime minDate = context.GetValue(this.MinimumDate);

            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            List<SupplierZone> suppZones = supplierZoneManager.GetSupplierZonesEffectiveAfter(supplierId, minDate);
            ExistingZoneEntities.Set(context, suppZones);
        }
    }
}
