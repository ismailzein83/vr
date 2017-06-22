using System;

namespace Retail.EntitiesMigrator.Entities
{
    public class IncomingRate
    {
        public long SubscriberId { get; set; }
        public RateDetails LocalRate { get; set; }
        public RateDetails InternationalRate { get; set; }
        public DateTime ActivationDate { get; set; }
    }

    public class RateDetails
    {
        public int FractionUnit { get; set; }
        public decimal Rate { get; set; }
    }
}
