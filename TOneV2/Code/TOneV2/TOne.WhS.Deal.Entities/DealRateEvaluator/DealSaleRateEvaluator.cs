using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public abstract class DealSaleRateEvaluator : BaseDealRateEvaluator
    {
        public abstract void EvaluateRate(IDealSaleRateEvaluatorContext context);
    }

    public interface IDealSaleRateEvaluatorContext
    {
        List<long> ZoneIds { get; }

        DateTime DealBED { get; }

        DateTime DealEED { get; }

        Dictionary<long, List<EvaluatedDealSaleRate>> EvaluatedSaleRatesByZoneId { get; }

    }

    public class EvaluatedDealSaleRate
    {
        public Decimal Rate { get; set; }

        //public int CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

    }
}
