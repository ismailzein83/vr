using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Entities
{
    public abstract class DealSupplierRateEvaluator : BaseDealRateEvaluator
    {
        public abstract void EvaluateRate(IDealSupplierRateEvaluatorContext context);
    }

    public interface IDealSupplierRateEvaluatorContext
    {
        DateTime DealBED { get; }
        DateTime? DealEED { get; }
        int CurrencyId { get; }
        IEnumerable<DealRate> SupplierRates { get; set; }
        IEnumerable<DealSupplierZoneGroupZoneItem> SupplierZoneItem { get; set; }
        Dictionary<long, SupplierRate> SupplierZoneRateByZoneId { get; set; }

    }
}
