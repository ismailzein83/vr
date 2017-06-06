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

        public decimal? TargetDurationInSeconds { get; set; }

        public bool IsComplete { get { return TargetDurationInSeconds.HasValue && TargetDurationInSeconds == ReachedDurationInSeconds; } }

        public DateTime CreatedTime { get; set; }

        public bool IsEqual(DealProgress dealProgress)
        {
            if (DealID != dealProgress.DealID)
                return false;

            if (ZoneGroupNb != dealProgress.ZoneGroupNb)
                return false;

            if (ReachedDurationInSeconds != dealProgress.ReachedDurationInSeconds)
                return false;

            if (TargetDurationInSeconds != dealProgress.TargetDurationInSeconds)
                return false;

            if (CurrentTierNb != dealProgress.CurrentTierNb)
                return false;

            if (IsSale != dealProgress.IsSale)
                return false;

            return true;
        }
    }
}