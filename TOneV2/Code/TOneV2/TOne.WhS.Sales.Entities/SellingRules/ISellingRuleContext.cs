using System;
using Vanrise.Rules;

namespace TOne.WhS.Sales.Entities
{
    public interface ISellingRuleContext : IRuleExecutionContext
    {
        decimal CurrentRate { get; set; }
        decimal NewRate { get; set; }
        bool ViolateRateRule { get; set; }
        int CurrentCurrencyId { get; set; }
    }
}
