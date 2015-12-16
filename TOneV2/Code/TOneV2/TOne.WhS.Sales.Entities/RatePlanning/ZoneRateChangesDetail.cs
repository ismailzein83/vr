using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneRateChangesDetail
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public decimal? CurrentRate { get; set; }
        public bool IsCurrentRateInherited { get; set; }
        public decimal? NewRate { get; set; }
        public RateChangeType ChangeType { get; set; }
        public DateTime EffectiveOn { get; set; }
        public DateTime? EffectiveUntil { get; set; }
    }

    public enum RateChangeType
    {
        New = 0,
        Increase = 1,
        Decrease = 2,
        Close = 3
    }
}
