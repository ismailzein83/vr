using Retail.Billing.Business;
using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.MainExtensions.RetailBillingCharge
{
    public class RetailBillingCustomCodeCharge : Retail.Billing.Entities.RetailBillingCharge
    {
        public Dictionary<string, Object> FieldValues { get; set; }
        public override string GetDescription()
        {
            Retail.Billing.Entities.RetailBillingChargeType chargeType = new RetailBillingChargeTypeManager().GetRetailBillingChargeType(RetailBillingChargeTypeId);

            if (chargeType.Settings == null || chargeType.Settings.ExtendedSettings == null)
                return null;

            return chargeType.Settings.ExtendedSettings.GetDescription(new RetailBillingChargeTypeGetDescriptionContext()
            {
                ChargeTypeId = chargeType.VRComponentTypeId,
                Charge = this,
            });
        }
    }
}