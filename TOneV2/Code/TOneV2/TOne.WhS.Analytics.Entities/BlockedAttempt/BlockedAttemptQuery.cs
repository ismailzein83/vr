using System;

namespace TOne.WhS.Analytics.Entities
{
    public class BlockedAttemptQuery
    {
        public BlockedAttemptFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public int Top { get; set; }
    }
}
