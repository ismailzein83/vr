using System;

namespace TOne.WhS.Routing.Entities
{
    public class PartialRouteProcessState
    {
        public byte[] LastRuleTimeStamp { get; set; }

        public DateTime? LastProcessDate { get; set; }
    }
}
