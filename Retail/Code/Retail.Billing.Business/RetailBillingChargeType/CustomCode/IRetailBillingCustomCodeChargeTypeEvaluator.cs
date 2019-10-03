using System;

namespace Retail.Billing.Business
{
    public interface IRetailBillingCustomCodeChargeTypeEvaluator
    {
        decimal CalculateCharge();
    }
}