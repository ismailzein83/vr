using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class BillingCDRPricingBase
    {
        private long _ID;
        private int _Zone;
        private Double _Net;
        private String _Currency;
        private Double _RateValue;
        private long _Rate;
        private Double? _Discount;
        private TABS.ToDRateType _RateType = TABS.ToDRateType.Normal;
        private int _ToDConsideration;
        private Double? _FirstPeriod;
        private Byte? _RepeatFirstperiod;
        private Byte? _FractionUnit;
        private int _Tariff;
        private Double _CommissionValue;
        private int _Commission;
        private Double _ExtraChargeValue;
        private int _ExtraCharge;
        private DateTime _Updated = DateTime.Now;
        private DateTime _AttemptDateTime;
        public  Decimal DurationInSeconds { get; set; }

        private long _Billing_CDR_Main;

        public  long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public  int ZoneID
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        public  Double Net
        {
            get { return _Net; }
            set { _Net = value; }
        }

        public String CurrencySymbol
        {
            get { return _Currency; }
            set { _Currency = value; }
        }

        public  Double RateValue
        {
            get { return _RateValue; }
            set { _RateValue = value; }
        }

        public  long RateID
        {
            get { return _Rate; }
            set { _Rate = value; }
        }

        public  Double? Discount
        {
            get { return _Discount; }
            set { _Discount = value; }
        }

        public  TABS.ToDRateType RateType
        {
            get { return _RateType; }
            set { _RateType = value; }
        }

        public  int ToDConsiderationID
        {
            get { return _ToDConsideration; }
            set { _ToDConsideration = value; }
        }

        public  Double? FirstPeriod
        {
            get { return _FirstPeriod; }
            set { _FirstPeriod = value; }
        }

        public  Byte? RepeatFirstperiod
        {
            get { return _RepeatFirstperiod; }
            set { _RepeatFirstperiod = value; }
        }

        public  Byte? FractionUnit
        {
            get { return _FractionUnit; }
            set { _FractionUnit = value; }
        }

        public  int TariffID
        {
            get { return _Tariff; }
            set { _Tariff = value; }
        }

        public  Double CommissionValue
        {
            get { return _CommissionValue; }
            set { _CommissionValue = value; }
        }

        public  int CommissionID
        {
            get { return _Commission; }
            set { _Commission = value; }
        }

        public  Double ExtraChargeValue
        {
            get { return _ExtraChargeValue; }
            set { _ExtraChargeValue = value; }
        }

        public  int ExtraChargeID
        {
            get { return _ExtraCharge; }
            set { _ExtraCharge = value; }
        }

        public  DateTime Updated
        {
            get { return _Updated; }
            set { _Updated = value; }
        }

        public  DateTime Attempt
        {
            get { return _AttemptDateTime; }
            set { _AttemptDateTime = value; }
        }

        public  long BillingCDRMainID
        {
            get { return _Billing_CDR_Main; }
            set { _Billing_CDR_Main = value; }
        }

        public  string Code { get; set; }

    }
}
