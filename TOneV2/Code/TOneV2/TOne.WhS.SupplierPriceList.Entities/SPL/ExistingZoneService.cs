using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingZoneService : IExistingEntity
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierZoneService ZoneServiceEntity { get; set; }

        public ChangedZoneService ChangedZoneService { get; set; }

        public IChangedEntity ChangedEntity
        {
            get { return this.ChangedZoneService; }
        }

        public DateTime BED
        {
            get { return ZoneServiceEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedZoneService != null ? ChangedZoneService.EED : ZoneServiceEntity.EED; }
        }

        public bool IsSameEntity(IExistingEntity nextEntity)
        {
            ExistingZoneService nextZoneService = nextEntity as ExistingZoneService;

            return this.ParentZone.Name.Equals(nextZoneService.ParentZone.Name, StringComparison.InvariantCultureIgnoreCase)
                && this.ZoneServiceEntity.EffectiveServices == nextZoneService.ZoneServiceEntity.EffectiveServices;
        }
    }

}
