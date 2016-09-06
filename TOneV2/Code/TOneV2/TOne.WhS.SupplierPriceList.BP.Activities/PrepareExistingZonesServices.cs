using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareExistingZonesServices : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SupplierZoneService>> ExistingZoneServiceEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingZoneService>> ExistingZonesServices { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SupplierZoneService> existingZoneServiceEntities = this.ExistingZoneServiceEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingZoneService> existingZonesServices = existingZoneServiceEntities.OrderBy(item => item.BED).Where(x => existingZonesByZoneId.ContainsKey(x.ZoneId)).MapRecords(
                (codeEntity) => ExistingZoneServiceMapper(codeEntity, existingZonesByZoneId));

            ExistingZonesServices.Set(context, existingZonesServices);
        }

        ExistingZoneService ExistingZoneServiceMapper(SupplierZoneService zoneServiceEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(zoneServiceEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Zone Service Entity with Id {0} is not linked to Zone Id {1}", zoneServiceEntity.SupplierZoneServiceId, zoneServiceEntity.ZoneId));

            ExistingZoneService existingZoneService = new ExistingZoneService()
            {
                ZoneServiceEntity = zoneServiceEntity,
                ParentZone = existingZone
            };

            existingZoneService.ParentZone.ExistingZonesServices.Add(existingZoneService);
            return existingZoneService;
        }
    }
}
