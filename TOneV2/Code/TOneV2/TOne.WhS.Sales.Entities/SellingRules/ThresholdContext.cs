using System;
using Vanrise.Rules;

namespace TOne.WhS.Sales.Entities
{
    public class ThresholdContext : ISellingRuleContext
    {
        public decimal CurrentRate { get; set; }
        public decimal NewRate { get; set; }
        public bool ViolateRateRule { get; set; }
        public IVRRule Rule { get; set; }
        public int CurrentCurrencyId { get; set; }
    }
}
