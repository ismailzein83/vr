using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class BillingCDRPricingBase
    {

        public  Decimal DurationInSeconds { get; set; }

        public  long ID { get; set; }

        public  int ZoneID { get; set; }

        public  Double Net { get; set; }

        public String CurrencySymbol { get; set; }

        public  Double RateValue { get; set; }

        public  long RateID { get; set; }

        public  Double? Discount { get; set; }

        public  TABS.ToDRateType RateType { get; set; }

        public  int ToDConsiderationID { get; set; }

        public  Double? FirstPeriod { get; set; }

        public  Byte? RepeatFirstperiod { get; set; }

        public  Byte? FractionUnit { get; set; }

        public  int TariffID { get; set; }

        public  Double CommissionValue { get; set; }

        public  int CommissionID { get; set; }

        public  Double ExtraChargeValue { get; set; }

        public  int ExtraChargeID { get; set; }

        public  DateTime Updated { get; set; }

        public  DateTime Attempt { get; set; }

        public  long BillingCDRMainID { get; set; }

        public  string Code { get; set; }

    }
}
