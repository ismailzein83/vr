using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Entities
{
    public class DealSupplierZoneGroup : IDateEffectiveSettings
    {
        public int DealSupplierZoneGroupNb { get; set; }

        public int SupplierId { get; set; }

        public List<DealSupplierZoneGroupZoneItem> Zones { get; set; }

        public List<DealSupplierZoneGroupTier> Tiers { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class DealSupplierZoneGroupZoneItem
    {
        public long ZoneId { get; set; }
    }

    public class DealSupplierZoneGroupTier
    {
        public int TierNumber { get; set; }

        public int? RetroActiveFromTierNumber { get; set; }

        public int Volume { get; set; }

        //public DealSupplierZoneRateEvaluator DefaultRateEvaluator { get; set; }
        public Decimal Rate { get; set; }

        public List<DealSupplierZoneGroupTierZoneRate> ExceptionRates { get; set; }
    }

    public class DealSupplierZoneGroupTierZoneRate
    {
        public long ZoneId { get; set; }

        public Decimal Rate { get; set; }
        //public DealSupplierZoneRateEvaluator RateEvaluator { get; set; }
    }

    public abstract class DealSupplierZoneRateEvaluator
    {
        public abstract void Evaluate(IDealSupplierZoneRateEvaluatorContext context);
    }

    public interface IDealSupplierZoneRateEvaluatorContext
    {
        long SupplierZoneId { get; }

        DateTime AttemptTime { get; }

        Decimal Rate { set; }
    }
}