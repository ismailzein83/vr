using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;


namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareExistingZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SupplierZone>> ExistingZoneEntities { get; set; }

        [RequiredArgument]
        public InArgument<int> CountryId { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SupplierZone> existingZones = context.GetValue(this.ExistingZoneEntities);
            int countryId = context.GetValue(this.CountryId);

            Dictionary<long, ExistingZone> existingZoneDic = existingZones.OrderBy(item => item.BED).Where(x => x.CountryId == countryId).ToDictionary<BusinessEntity.Entities.SupplierZone, long, ExistingZone>((zoneEntity) => 
                zoneEntity.SupplierZoneId, (zoneEntity) => new ExistingZone { ZoneEntity = zoneEntity });

            ExistingZonesByZoneId.Set(context, existingZoneDic);
        }
    }
}
