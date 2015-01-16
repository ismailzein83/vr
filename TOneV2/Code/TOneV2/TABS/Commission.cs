using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    [Serializable]
    public class Commission : Components.DateTimeEffectiveEntity, Interfaces.ICachedCollectionContainer, Interfaces.IZoneSupplied, ICloneable
    {
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(Commission).FullName);
        }
        public override string Identifier { get { return "Commission:" + CommissionID; } }

        private int _CommissionID;
        private CarrierAccount _Supplier;
        private CarrierAccount _Customer;
        private Zone _Zone;
        private Nullable<decimal> _Amount;
        private Nullable<float> _Percentage;
        private float? _FromRate;
        private float? _ToRate;
        private bool _IsExtraCharge;

        public virtual int CommissionID
        {
            get { return _CommissionID; }
            set { _CommissionID = value; }
        }

        public virtual CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        public virtual CarrierAccount Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }

        public virtual Zone Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        public virtual Nullable<float> Percentage
        {
            get { return _Percentage; }
            set { _Percentage = value; }
        }

        public virtual Nullable<decimal> Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }
        public virtual float? FromRate
        {
            get { return _FromRate; }
            set { _FromRate = value; }
        }

        public virtual float? ToRate
        {
            get { return _ToRate; }
            set { _ToRate = value; }
        }

        public virtual bool IsExtraCharge
        {
            get { return _IsExtraCharge; }
            set { _IsExtraCharge = value; }
        }
        public string DefinitionDisplay
        {
            get
            {
                return string.Format("Commission on {0} of {1}", Zone.ToString(), Customer);
            }
        }


        public double DeductedRateValue(bool isCost, double value)
        {
            double deductedValue = value;
            double factor = isCost ? 1 : -1;

            // Percentage?
            if (this.Percentage.HasValue && this.Percentage.Value > 0)
                deductedValue = value * (1 + factor * this.Percentage.Value / 100.0);
            else // Fixed amount
                deductedValue = value + factor * (double)this.Amount.Value;
            return deductedValue;
        }

        public override string ToString()
        {
            return this.DefinitionDisplay;
        }

        protected string DefinitionCompare
        {
            get { return this.Supplier.Name + this.Zone.Name + this.Customer.Name + this.IsExtraCharge; }
        }

        public bool CompareBeginEndEffectiveDate(Commission Other)
        {
            bool ConditionBeginEndEffectiveDate = false;
            if (Other.BeginEffectiveDate == Other.EndEffectiveDate)
                ConditionBeginEndEffectiveDate = false;
            else
            {
                if ((this.BeginEffectiveDate <= Other.BeginEffectiveDate && !this.EndEffectiveDate.HasValue)
                || (this.BeginEffectiveDate <= Other.BeginEffectiveDate && this.EndEffectiveDate.HasValue && this.EndEffectiveDate > Other.BeginEffectiveDate)
                || (this.BeginEffectiveDate >= Other.BeginEffectiveDate && !Other.EndEffectiveDate.HasValue)
                || (this.BeginEffectiveDate >= Other.BeginEffectiveDate && Other.EndEffectiveDate.HasValue && this.BeginEffectiveDate < Other.EndEffectiveDate)
                   )
                    ConditionBeginEndEffectiveDate = true;
            }
            return ConditionBeginEndEffectiveDate;
        }

        public bool ConflictsWith(Commission Other)
        {
            return (this.DefinitionCompare.Equals(Other.DefinitionCompare) && CompareBeginEndEffectiveDate(Other));
        }

        protected static IList<Commission> _All;
        public static IList<Commission> All
        {
            get
            {
                if (_All == null)
                    _All = ObjectAssembler.GetList<Commission>();
                return _All;
            }
        }

        public static bool IsCommissioned(Rate rate, DateTime beginEffectiveDate)
        {
            bool result = false;

            try
            {
                var commission = All.FirstOrDefault(c =>
                    c.Zone.Equals(rate.Zone)
                  && c.Customer.Equals(rate.PriceList.Customer)
                  && c.Supplier.Equals(rate.PriceList.Supplier)
                  && c.IsEffectiveOn(beginEffectiveDate)
                    );
                result = commission != null;
            }
            catch { }

            return result;
        }

        //public bool CompareRatesValue(Commission Other)
        //{
        //    if (
        //        (Other.FromRate < this.FromRate && Other.FromRate < this.ToRate)
        //        ||
        //        (Other.FromRate > this.FromRate && Other.FromRate > this.ToRate)
        //        )
        //        return true;
        //    return false; 
        //}

        public bool CompareRatesValue(Commission Other)
        {
            if (
                (Other.ToRate <= this.FromRate && Other.FromRate < this.FromRate && Other.ToRate < this.ToRate)
                ||
                (Other.FromRate == this.FromRate && this.ToRate == Other.ToRate && (this.EndEffectiveDate > Other.EndEffectiveDate || Other.EndEffectiveDate > this.BeginEffectiveDate))
                ||
                (Other.FromRate == this.FromRate && Other.ToRate == this.ToRate && this.BeginEffectiveDate > Other.BeginEffectiveDate)
                ||
                (Other.FromRate > this.FromRate && Other.FromRate >= this.ToRate)
                )
                return true;
            return false;
        }

        #region ICloneable Members

        public object Clone()
        {
            Commission clone = (Commission)this.MemberwiseClone();
            clone.CommissionID = this.CommissionID;
            return clone;
        }

        #endregion
    }
}