using System.Collections.Generic;

namespace TOne.WhS.Analytics.Entities
{
    public class BlockedAttemptFilter
    {
        public List<int> SwitchIds { get; set; }
        public List<int> CustomerIds { get; set; }
        public List<long> SaleZoneIds { get; set; }
        public bool GroupByNumber { get; set; }
        
    }
}
