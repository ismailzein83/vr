using Retail.Billing.Entities;
using System;
using System.Collections.Generic;

namespace Retail.Billing.Business
{
    public class RetailBillingChargeManager
    {
        #region Public Methods

        public Decimal EvaluateRetailBillingCharge(RetailBillingCharge charge, Dictionary<string, Object> targetFieldValues)
        {
            if (charge == null || charge.RetailBillingChargeTypeId == null)
                return 0;

            RetailBillingChargeType chargeType = new RetailBillingChargeTypeManager().GetRetailBillingChargeType(charge.RetailBillingChargeTypeId);

            if (chargeType.Settings == null || chargeType.Settings.ExtendedSettings == null)
                return 0;

            return chargeType.Settings.ExtendedSettings.CalculateCharge(new RetailBillingChargeTypeCalculateChargeContext()
            {
                Charge = charge,
                TargetFieldValues = targetFieldValues
            });
        }

        public string GetChargeDescription(RetailBillingCharge charge)
        {
            if (charge == null)
                return null;
            return charge.GetHashCode().ToString();
        }

        #endregion
    }
}