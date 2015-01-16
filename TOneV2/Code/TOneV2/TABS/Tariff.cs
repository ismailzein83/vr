using System;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public class Tariff : Components.DateTimeEffectiveEntity, Interfaces.ICachedCollectionContainer, Interfaces.IZoneSupplied, ICloneable
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _OurTariffs = null;
        }

        public override string Identifier { get { return "Tariff:" + TariffID; } }

        private int _TariffID;
        private Zone _Zone;
        private CarrierAccount _Supplier;
        private CarrierAccount _Customer;
        private decimal _CallFee;
        private decimal _FirstPeriodRate;
        private int _FirstPeriod;
        private string _RepeatFirstPeriod;
        private int _FractionUnit;

        internal static IList<Tariff> _OurTariffs;

        public static IList<Tariff> OurTariffs
        {
            get
            {
                if (_OurTariffs == null)
                {
                    _OurTariffs = ObjectAssembler.GetTariffs(CarrierAccount.SYSTEM, null, DateTime.Now);
                }
                return _OurTariffs;
            }
            set
            {
                _OurTariffs = value;
            }
        }

        public bool IsActive
        {
            get
            {
                if (IsEffective)
                {
                    DateTime now = DateTime.Now;
                    string time = now.ToString("HH:mm:ss.fff");

                    // Assume effective
                    bool isActive = true;

                    if (this._BeginEffectiveDate != null && (time.CompareTo(_BeginEffectiveDate) < 0 || (_EndEffectiveDate != null && time.CompareTo(_EndEffectiveDate) > 0))) isActive = false;
                    return isActive;
                }
                else
                    return false;
            }
        }
        public virtual int TariffID
        {
            get { return _TariffID; }
            set { _TariffID = value; }
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

        public virtual decimal CallFee
        {
            get { return _CallFee; }
            set { _CallFee = value; }
        }

        public virtual decimal FirstPeriodRate
        {
            get { return _FirstPeriodRate; }
            set { _FirstPeriodRate = value; }
        }

        public virtual int FirstPeriod
        {
            get { return _FirstPeriod; }
            set { _FirstPeriod = value; }
        }

        public virtual bool RepeatFirstPeriod
        {
            get { return "Y".Equals(_RepeatFirstPeriod); }
            set { _RepeatFirstPeriod = value ? "Y" : "N"; }
        }

        public virtual int FractionUnit
        {
            get { return _FractionUnit; }
            set { _FractionUnit = value; }
        }

        public static string FormatTime(Nullable<TimeSpan> timespan)
        {
            if (timespan == null) return null;
            else
                return string.Format(
                    "{0:00}:{1:00}:{2:00}.{3:000}",
                    timespan.Value.Hours,
                    timespan.Value.Minutes,
                    timespan.Value.Seconds,
                    timespan.Value.Milliseconds);
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(" Tariff for ");
            sb.Append(((Customer != null) ? (" Customer: " + Customer.CarrierProfile.Name) : ""));
            sb.Append(((Supplier != null) ? (" Supplier : " + Supplier.CarrierProfile.Name) : ""));
            sb.Append((Zone != null) ? " on :" + Zone.Name : "");
            return sb.ToString();
        }

        public string DefinitionDisplay
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(" Tariff for ");
                sb.Append(((Customer != null) ? (" Customer: " + Customer.CarrierProfile.Name) : ""));
                sb.Append(((Supplier != null) ? (" Supplier : " + Supplier.CarrierProfile.Name) : ""));
                sb.Append((Zone != null) ? " on :" + Zone.Name : "");
                return sb.ToString();
            }
        }

        public string IncrementDisplay
        {

            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (FirstPeriod > 0)
                    sb.Append(FirstPeriod);

                if (FractionUnit > 0)
                    sb.Append(FirstPeriod > 0 ? "/" : FractionUnit + "/")
                        .Append(FractionUnit);
                else
                    sb.Append("/1");

                if (RepeatFirstPeriod)
                    sb.Append("/").Append(FirstPeriod);

                return sb.ToString();
            }
        }

        public string DefinitionCompare
        {
            get
            {
                return Supplier.Name + Zone.Name + Customer.Name;
            }
        }

        public bool CompareBeginEndEffectiveDate(Tariff Other)
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

        public bool ConflictWith(Tariff Other)
        {
            return (this.DefinitionCompare.Equals(Other.DefinitionCompare) && CompareBeginEndEffectiveDate(Other));
        }

        #region ICloneable Members

        public object Clone()
        {
            Tariff clone = (Tariff)this.MemberwiseClone();
            clone.TariffID = this.TariffID;
            return clone;
        }

        #endregion
    }
}
