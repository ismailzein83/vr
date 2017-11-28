using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingZoneRoutingProducts : IExistingEntity
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SaleZoneRoutingProduct ZoneRoutingProductEntity { get; set; }

        public ChangedZoneRoutingProducts ChangedZoneRoutingProducts { get; set; }

        public IChangedEntity ChangedEntity
        {
            get { return this.ChangedZoneRoutingProducts; }
        }

        public DateTime BED
        {
            get { return ZoneRoutingProductEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedZoneRoutingProducts != null ? ChangedZoneRoutingProducts.EED : ZoneRoutingProductEntity.EED; }
        }

        public bool IsSameEntity(IExistingEntity nextEntity)
        {
            ExistingZoneRoutingProducts nextExistingZoneRoutingProducts = nextEntity as ExistingZoneRoutingProducts;

            return this.ParentZone.Name.Equals(nextExistingZoneRoutingProducts.ParentZone.Name, StringComparison.InvariantCultureIgnoreCase) && nextExistingZoneRoutingProducts.ZoneRoutingProductEntity.RoutingProductId==this.ZoneRoutingProductEntity.RoutingProductId;
                
        }
        public DateTime? OriginalEED
        {
            get { return this.ZoneRoutingProductEntity.EED; }
        }
    }
}
