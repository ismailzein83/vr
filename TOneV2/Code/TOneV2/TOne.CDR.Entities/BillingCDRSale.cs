using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class BillingCDRSale : BillingCDRPricingBase
    {
        public virtual void Copy(BillingCDRPricingBase other)
        {
            this.BillingCDRMainID = other.BillingCDRMainID;
            this.CommissionID = other.CommissionID;
            this.CommissionValue = other.CommissionValue;
            this.CurrencySymbol = other.CurrencySymbol;
            this.Discount = other.Discount;
            this.ExtraChargeID = other.ExtraChargeID;
            this.ExtraChargeValue = other.ExtraChargeValue;
            this.FirstPeriod = other.FirstPeriod;
            this.FractionUnit = other.FractionUnit;
            this.Net = other.Net;
            this.RateID = other.RateID;
            this.RateType = other.RateType;
            this.RateValue = other.RateValue;
            this.RepeatFirstperiod = other.RepeatFirstperiod;
            this.TariffID = other.TariffID;
            this.ToDConsiderationID = other.ToDConsiderationID;
            this.Updated = other.Updated;
            //this.Useri = other.User;
            this.ZoneID = other.ZoneID;
            this.DurationInSeconds = other.DurationInSeconds;
            this.Code = other.Code;
            this.Attempt = other.Attempt;
        }
    }
}
