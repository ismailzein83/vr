using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class GetZoneInheritedServiceInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public long ZoneId { get; set; }
        public DateTime EffectiveOn { get; set; }
        public Changes NewDraft { get; set; }
    }
}
