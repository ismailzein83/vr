using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingZoneServices : IExistingEntity
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SaleEntityZoneService ZoneServiceEntity { get; set; }

        public ChangedZoneServices ChangedZoneServices { get; set; }

        public IChangedEntity ChangedEntity
        {
            get { return this.ChangedZoneServices; }
        }

        public DateTime BED
        {
            get { return ZoneServiceEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedZoneServices != null ? ChangedZoneServices.EED : ZoneServiceEntity.EED; }
        }

        public bool IsSameEntity(IExistingEntity nextEntity)
        {
            ExistingZoneServices nextExistingZoneServices = nextEntity as ExistingZoneServices;

            return this.ParentZone.Name.Equals(nextExistingZoneServices.ParentZone.Name, StringComparison.InvariantCultureIgnoreCase)
                && SameServiceIds(this.ZoneServiceEntity.Services, nextExistingZoneServices.ZoneServiceEntity.Services);
        }
        private bool SameServiceIds(List<ZoneService> zoneServices, List<ZoneService> serviceIds)
        {
            if (!(zoneServices.Count == serviceIds.Count))
                return false;

            foreach (ZoneService zoneService in zoneServices)
            {
                if (!serviceIds.Any(item => item.ServiceId == zoneService.ServiceId))
                    return false;
            }

            return true;
        }

        public DateTime? OriginalEED
        {
            get { return this.ZoneServiceEntity.EED; }
        }
    }
}
