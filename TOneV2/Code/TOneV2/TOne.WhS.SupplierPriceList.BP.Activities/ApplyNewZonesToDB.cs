using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyNewZonesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewZone>> NewZones { get; set; }
              

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewZone> zonesList = this.NewZones.Get(context);
            int supplierId = this.SupplierId.Get(context);
            int priceListId = this.PriceListId.Get(context);

            NewSupplierZoneManager manager = new NewSupplierZoneManager();
            manager.Insert(supplierId, priceListId, zonesList);
        }
    }
}
