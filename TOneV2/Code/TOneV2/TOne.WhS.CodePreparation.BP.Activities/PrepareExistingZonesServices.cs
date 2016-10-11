using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class PrepareExistingZonesServices : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SaleEntityZoneService>> ExistingZonesServicesEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingZoneServices>> ExistingZonesServices { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleEntityZoneService> existingZonesServicesEntities = this.ExistingZonesServicesEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingZoneServices> existingZonesServices = existingZonesServicesEntities.Where(x => existingZonesByZoneId.ContainsKey(x.ZoneId)).MapRecords(
                (ZoneServicesEntity) => ExistingSaleEntityZoneServicesMapper(ZoneServicesEntity, existingZonesByZoneId));

            ExistingZonesServices.Set(context, existingZonesServices);
        }

        ExistingZoneServices ExistingSaleEntityZoneServicesMapper(SaleEntityZoneService ZoneServiceEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(ZoneServiceEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Zone Service Entity with Id {0} is not linked to Zone Id {1}", ZoneServiceEntity.SaleEntityServiceId, ZoneServiceEntity.ZoneId));

            ExistingZoneServices existingZonesServices = new ExistingZoneServices()
            {
                ZoneServiceEntity = ZoneServiceEntity,
                ParentZone = existingZone
            };

            existingZonesServices.ParentZone.ExistingZonesServices.Add(existingZonesServices);
            return existingZonesServices;
        }
    }
}
