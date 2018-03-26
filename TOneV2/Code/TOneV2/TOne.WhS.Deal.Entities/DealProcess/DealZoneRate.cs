using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealZoneRate : Vanrise.Entities.IDateEffectiveSettings
	{
        public long DealZoneRateId { get; set; }
        public int DealId { get; set; }
        public int ZoneGroupNb { get; set; }
        public bool IsSale { get; set; }
        public int TierNb { get; set; }
        public long ZoneId { get; set; }
        public decimal Rate { get; set; }
		public int CurrencyId { get; set; }
		public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}