using System;

namespace TABS
{
    public abstract class Billing_CDR_Pricing_Base : Components.BaseEntity
    {
        #region DataMembers

        private long _ID;
        private Zone _Zone;
        private Double _Net;
        private Currency _Currency;
        private Double _RateValue;
        private Rate _Rate;
        private Double? _Discount;
        private ToDRateType _RateType = ToDRateType.Normal;
        private ToDConsideration _ToDConsideration;
        private Double? _FirstPeriod;
        private Byte? _RepeatFirstperiod;
        private Byte? _FractionUnit;
        private Tariff _Tariff;
        private Double _CommissionValue;
        private Commission _Commission;
        private Double _ExtraChargeValue;
        private Commission _ExtraCharge;
        private DateTime _Updated = DateTime.Now;
        private DateTime _AttemptDateTime;
        public virtual Decimal DurationInSeconds { get; set; }

        private Billing_CDR_Main _Billing_CDR_Main;

        public virtual long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public virtual Zone Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        public virtual Double Net
        {
            get { return _Net; }
            set { _Net = value; }
        }

        public virtual Currency Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }

        public virtual Double RateValue
        {
            get { return _RateValue; }
            set { _RateValue = value; }
        }

        public virtual Rate Rate
        {
            get { return _Rate; }
            set { _Rate = value; }
        }

        public virtual Double? Discount
        {
            get { return _Discount; }
            set { _Discount = value; }
        }

        public virtual ToDRateType RateType
        {
            get { return _RateType; }
            set { _RateType = value; }
        }

        public virtual ToDConsideration ToDConsideration
        {
            get { return _ToDConsideration; }
            set { _ToDConsideration = value; }
        }

        public virtual Double? FirstPeriod
        {
            get { return _FirstPeriod; }
            set { _FirstPeriod = value; }
        }

        public virtual Byte? RepeatFirstperiod
        {
            get { return _RepeatFirstperiod; }
            set { _RepeatFirstperiod = value; }
        }

        public virtual Byte? FractionUnit
        {
            get { return _FractionUnit; }
            set { _FractionUnit = value; }
        }

        public virtual Tariff Tariff
        {
            get { return _Tariff; }
            set { _Tariff = value; }
        }

        public virtual Double CommissionValue
        {
            get { return _CommissionValue; }
            set { _CommissionValue = value; }
        }

        public virtual Commission Commission
        {
            get { return _Commission; }
            set { _Commission = value; }
        }

        public virtual Double ExtraChargeValue
        {
            get { return _ExtraChargeValue; }
            set { _ExtraChargeValue = value; }
        }

        public virtual Commission ExtraCharge
        {
            get { return _ExtraCharge; }
            set { _ExtraCharge = value; }
        }

        public virtual DateTime Updated
        {
            get { return _Updated; }
            set { _Updated = value; }
        }

        public virtual DateTime Attempt
        {
            get { return _AttemptDateTime; }
            set { _AttemptDateTime = value; }
        }

        public virtual Billing_CDR_Main Billing_CDR_Main
        {
            get { return _Billing_CDR_Main; }
            set { _Billing_CDR_Main = value; }
        }

        public virtual string Code { get; set; }

        /// <summary>
        /// Copy all members of this Pricing Base from another instance
        /// </summary>
        /// <param name="other"></param>
        public virtual void Copy(Billing_CDR_Pricing_Base other)
        {
            this.Billing_CDR_Main = other.Billing_CDR_Main;
            this.Commission = other.Commission;
            this.CommissionValue = other.CommissionValue;
            this.Currency = other.Currency;
            this.Discount = other.Discount;
            this.ExtraCharge = other.ExtraCharge;
            this.ExtraChargeValue = other.ExtraChargeValue;
            this.FirstPeriod = other.FirstPeriod;
            this.FractionUnit = other.FractionUnit;
            this.Net = other.Net;
            this.Rate = other.Rate;
            this.RateType = other.RateType;
            this.RateValue = other.RateValue;
            this.RepeatFirstperiod = other.RepeatFirstperiod;
            this.Tariff = other.Tariff;
            this.ToDConsideration = other.ToDConsideration;
            this.Updated = other.Updated;
            this.User = other.User;
            this.Zone = other.Zone;
            this.DurationInSeconds = other.DurationInSeconds;
            this.Code = other.Code;
            this.Attempt = other.Attempt;
        }

        #endregion
    }
}
