using System;

namespace TOne.WhS.Analytics.Entities
{

    public class BlockedAttemptDetail
    {
        public BlockedAttempt Entity { get; set; }
        public String SaleZoneName { get; set; }
        public String CustomerName { get; set; }
        public string ReleaseCodeDescription { get; set; }
    }
}
