using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class DealBuyingPart
    {
        public string Name { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public Decimal Volume { get; set; }

        public Decimal Rate { get; set; }

        public Decimal MinSellingRate { get; set; }

        public Decimal SubstituteRate { get; set; }

        public Decimal ExtraVolumeRate { get; set; }
        public Decimal ASR { get; set; }
        public Decimal NER { get; set; }
        public Decimal ACD { get; set; }
    }
}
