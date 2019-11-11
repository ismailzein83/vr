using System;

namespace Retail.Billing.Business
{
    public interface IRetailBillingCustomCodeChargeTypePriceEvaluator
    {
        decimal CalculateCharge();
    }
    public interface IRetailBillingCustomCodeChargeTypeDescriptionEvaluator
    {
        string GetDescription();
    }
}