using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierAccountCustomerSettings
    {
        public int? DefaultRoutingProductId { get; set; }

        public RoutingStatus RoutingStatus { get; set; }

        public int? TimeZoneId { get; set; }

        public bool InvoiceTimeZone { get; set; }

        public PassThroughCustomerRateEvaluator PassThroughCustomerRateEvaluator { get; set; }

        public PricingSettings PricingSettings { get; set; }

        public PricelistSettings PricelistSettings { get; set; }
    }

    public abstract class PassThroughCustomerRateEvaluator
    {
        public abstract Guid ConfigId { get; }
        public abstract decimal? EvaluateCustomerRate(IPassThroughEvaluateCustomerRateContext context);
    }

    public interface IPassThroughEvaluateCustomerRateContext
    {
        decimal? CostRate { get; }

        int? CostCurrencyId { get; }
    }

    public class PassThroughEvaluateCustomerRateContext : IPassThroughEvaluateCustomerRateContext
    {
        public decimal? CostRate { get; set; }

        public int? CostCurrencyId { get; set; }
    }
}
