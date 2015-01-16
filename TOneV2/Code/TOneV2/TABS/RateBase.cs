using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    [Serializable]
    public abstract class RateBase : Components.DateTimeEffectiveFlaggedServicesEntity, Interfaces.IZoneSupplied, IComparable<RateBase>
    {
        public override string Identifier { get { return ID > 0 ? string.Concat(GetType().Name, ":", ID.ToString()) : string.Concat(GetType().Name, ":", Zone); } }

        #region Data Members

        private long _ID;
        private PriceListBase _PriceListBase;
        private Zone _Zone;
        private Nullable<decimal> _Rate;
        private Nullable<decimal> _OffPeakRate;
        private Nullable<decimal> _WeekendRate;
        private Change _Change;

        public virtual CarrierAccount Supplier { get { return (PriceListBase == null) ? null : PriceListBase.Supplier; } set { } }
        public virtual CarrierAccount Customer { get { return (PriceListBase == null) ? null : PriceListBase.Customer; } set { } }

        public virtual string SupplierID { get { return (Supplier == null) ? null : Supplier.CarrierAccountID; } set { } }
        public virtual string CustomerID { get { return (Customer == null) ? null : Customer.CarrierAccountID; } set { } }

        public virtual Zone Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        public virtual int ZoneID { get { return (Zone != null) ? Zone.ZoneID : 0; } }

        public virtual long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public virtual PriceListBase PriceListBase
        {
            get { return _PriceListBase; }
            set { _PriceListBase = value; }
        }


        public virtual Nullable<decimal> Value
        {
            get { return _Rate; }
            set { _Rate = value; }
        }

        public virtual Nullable<decimal> ValueInMainCurrency
        {
            get
            {
                if (Value.HasValue)
                {
                    return decimal.Parse(((float)Value.Value / this.PriceListBase.Currency.LastRate).ToString("0.00000"));
                }
                else
                    return null;
            }
        }

        public virtual Nullable<decimal> OffPeakValueInMainCurrency
        {
            get
            {
                if (OffPeakRate.HasValue)
                {
                    return decimal.Parse(((float)OffPeakRate.Value / this.PriceListBase.Currency.LastRate).ToString("0.00000"));
                }
                else
                    return null;
            }
        }

        public virtual Nullable<decimal> WeekEndInMainCurrency
        {
            get
            {
                if (WeekendRate.HasValue)
                {
                    return decimal.Parse(((float)WeekendRate.Value / this.PriceListBase.Currency.LastRate).ToString("0.00000"));
                }
                else
                    return null;
            }
        }

        public virtual Nullable<decimal> OffPeakRate
        {
            get { return _OffPeakRate; }
            set { _OffPeakRate = value; }
        }

        public virtual Nullable<decimal> WeekendRate
        {
            get { return _WeekendRate; }
            set { _WeekendRate = value; }
        }

        public virtual Change Change
        {
            get { return _Change; }
            set { _Change = value; }
        }

        public virtual string Notes { get; set; }

        #endregion Data Members

        #region ILifecycle Members

        protected List<Code> _EffectiveCodes;
        public List<Code> EffectiveCodes
        {
            get
            {
                if (_EffectiveCodes == null)
                    if (this.ID > 0)
                        _EffectiveCodes = TABS.ObjectAssembler.GetCurrentandFutureEffectiveCodes(this.Zone
                            , this.PriceListBase.BeginEffectiveDate.Value).ToList();
                    else _EffectiveCodes = new List<Code>();

                _EffectiveCodes = _EffectiveCodes.Where(r => Nullable.Compare<DateTime>(r.BeginEffectiveDate, r.EndEffectiveDate) != 0).ToList();

                return _EffectiveCodes;
            }
            set
            {
                _EffectiveCodes = value;
            }
        }

        public static decimal GetRate(decimal rate, TABS.Currency originalCurrency, TABS.Currency otherCurrency, DateTime date)
        {
            decimal result = rate;

            if (originalCurrency.Equals(otherCurrency)) return result;

            var exchanges = TABS.CurrencyExchangeRate.ExchangeRates.SelectMany(exch => exch.Value).Where(e => e.ExchangeDate <= date);

            if (exchanges == null) return result;

            decimal? ExchangeInOriginalCurrency = originalCurrency.Equals(TABS.Currency.Main) ? 1m : (
                                                exchanges.Any(exch => exch.Currency.Equals(originalCurrency))
                                                ? (decimal?)exchanges.OrderByDescending(e => e.ExchangeDate).First(exch => exch.Currency.Equals(originalCurrency)).Rate
                                                : null);

            decimal? ExchangeInOtherCurrency = otherCurrency.Equals(TABS.Currency.Main) ? 1m : (
                                                exchanges.Any(exch => exch.Currency.Equals(otherCurrency))
                                                ? (decimal?)exchanges.OrderByDescending(e => e.ExchangeDate).First(exch => exch.Currency.Equals(otherCurrency)).Rate
                                                : null);

            if (ExchangeInOriginalCurrency == null || ExchangeInOtherCurrency == null) return result;

            result = result * (decimal)ExchangeInOtherCurrency / (decimal)ExchangeInOriginalCurrency;

            return result;
        }

        public override NHibernate.Classic.LifecycleVeto OnDelete(NHibernate.ISession s)
        {
            if (this == Rate.None)
                return NHibernate.Classic.LifecycleVeto.Veto;
            else
                return NHibernate.Classic.LifecycleVeto.NoVeto;
        }


        public override NHibernate.Classic.LifecycleVeto OnSave(NHibernate.ISession s)
        {
            if (this == Rate.None)
                return NHibernate.Classic.LifecycleVeto.Veto;
            else
                return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        public override NHibernate.Classic.LifecycleVeto OnUpdate(NHibernate.ISession s)
        {
            if (this == Rate.None)
                return NHibernate.Classic.LifecycleVeto.Veto;
            else
                return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1:0." + SystemConfiguration.GetRateFormat() + "} {2}, {3:dd/MM/yyyy}", this.Zone.Name, Value, PriceListBase.Currency, BeginEffectiveDate);
        }

        #endregion

        #region IComparable<RateBase> Members

        public int CompareTo(RateBase other)
        {
            if (other == null) return 0;
            int result = Nullable.Compare<decimal>(this.Value, other.Value);
            if (result == 0) result = other.ServicesFlag.CompareTo(this.ServicesFlag);
            return result;
        }



        #endregion

        //public override bool Equals(object obj)
        //{
        //    RateBase other = obj as RateBase;
        //    if (other == null) return false;
        //    if (other.Supplier.Equals(this.Supplier) && other.Zone.Equals(other.Zone)) return true;
        //    return false;
        //}
    }
}
