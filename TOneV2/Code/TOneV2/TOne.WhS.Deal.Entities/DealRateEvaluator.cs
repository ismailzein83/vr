using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public abstract class DealRateEvaluator
    {
        public abstract void EvaluateRate(IDealRateEvaluatorContext context);
    }

    public interface IDealRateEvaluatorContext
    {
        List<long> ZoneIds { get; }

        DateTime DealBED { get; }

        DateTime DealEED { get; }

        Dictionary<long, List<EvaluatedDealRate>> EvaluatedRatesByZoneId { get; }

    }

    public class EvaluatedDealRate
    {
        public Decimal Rate { get; set; }

        //public int CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

    }
}
