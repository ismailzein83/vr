using System;
namespace TOne.WhS.Deal.Entities
{
    public class DealProgress
    {
        public long DealProgressID { get; set; }

        public int DealID { get; set; }

        public int ZoneGroupNb { get; set; }

        public bool IsSale { get; set; }

        public int CurrentTierNb { get; set; }

        public decimal? ReachedDurationInSeconds { get; set; }

        public decimal TargetDurationInSeconds { get; set; }

        public bool IsComplete { get { return TargetDurationInSeconds == ReachedDurationInSeconds; } }

        public DateTime CreatedTime { get; set; }
    }
}