using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class DealSellingPart
    {
        public string Name { get; set; }

        public List<long> SaleZoneIds { get; set; }

        public Decimal Volume { get; set; }

        public Decimal Rate { get; set; }

        public Decimal MaxBuyingRate { get; set; }

        public Decimal SubstituteRate { get; set; }

        public Decimal ExtraVolumeRate { get; set; }
    }
}
