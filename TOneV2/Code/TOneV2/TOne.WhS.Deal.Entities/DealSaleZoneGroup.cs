using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealSaleZoneGroup
    {
        public Guid DealSaleZoneGroupId { get; set; }

        public int CustomerId { get; set; }

        public List<DealSaleZoneGroupItem> Items { get; set; }
    }

    public class DealSaleZoneGroupItem
    {
        public List<DealSaleZoneGroupZoneItem> Zones { get; set; }

        public List<DealSaleZoneGroupTier> Tiers { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class DealSaleZoneGroupZoneItem
    {
        public long ZoneId { get; set; }
    }

    public class DealSaleZoneGroupTier
    {
        public int TierNumber { get; set; }

        public int? RetroActiveFromTierNumber { get; set; }

        public int Volume { get; set; }

        public DealSaleZoneRateEvaluator DefaultRateEvaluator { get; set; }

        public List<DealSaleZoneGroupTierZoneRate> ExceptionRates { get; set; }
    }

    public class DealSaleZoneGroupTierZoneRate
    {
        public long ZoneId { get; set; }

        public DealSaleZoneRateEvaluator RateEvaluator { get; set; }
    }

    public abstract class DealSaleZoneRateEvaluator
    {
        public abstract void Evaluate(IDealSaleZoneRateEvaluatorContext context);
    }

    public interface IDealSaleZoneRateEvaluatorContext
    {
        long SaleZoneId { get; }

        DateTime AttemptTime { get; }

        Decimal Rate { set; }
    }
}
