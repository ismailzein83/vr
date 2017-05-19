using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealBuyRouteRuleSettings : Vanrise.Entities.VRRuleSettings
    {
        //public override Guid VRRuleSettingsConfigId { get { return new Guid("5CCCA10A-4980-4F2A-8004-8F7198377FB7"); } }

        public string Description { get; set; }

        public int SwapDealId { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public SwapDealBuyRouteRuleExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class SwapDealBuyRouteRuleExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Evaluate(ISwapDealBuyRouteRuleEvaluateContext context);
    }

    public interface ISwapDealBuyRouteRuleEvaluateContext
    {
        List<SwapDealBuyRouteRuleEvaluationItem> EvaluationItems { set; }

        List<long> SaleZoneIds { get; }
    }

    public class SwapDealBuyRouteRuleEvaluationItem
    {
        public int CustomerId { get; set; }

        public long SaleZoneId { get; set; }

        public Decimal Percentage { get; set; }
    }
}
