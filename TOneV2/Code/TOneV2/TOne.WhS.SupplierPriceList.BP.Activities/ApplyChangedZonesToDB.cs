using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyChangedZonesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedZone>> ChangedZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedZone> zonesList = this.ChangedZones.Get(context);
            int priceListId = this.PriceListId.Get(context);

            ChangedSupplierZoneManager manager = new ChangedSupplierZoneManager();
            manager.Insert(priceListId, zonesList);
        }
    }
}
