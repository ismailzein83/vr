using System;

namespace TOne.WhS.Analytics.Entities
{
    public class BlockedAttemptInput
    {
        public BlockedAttemptFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
