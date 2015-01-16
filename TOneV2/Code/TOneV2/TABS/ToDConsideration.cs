using System;
using System.Collections.Generic;


namespace TABS
{
    [Serializable]
    public class ToDConsideration : Components.DateTimeEffectiveEntity, Interfaces.ICachedCollectionContainer, Interfaces.IZoneSupplied
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _OwnToDConsiderations = null;
            _SupplierToDConsiderations = null;
        }

        public override string Identifier { get { return "ToDConsideration:" + ToDConsiderationID; } }


        internal static IList<ToDConsideration> _SupplierToDConsiderations;
        public static IList<ToDConsideration> SupplierToDConsiderations
        {
            get
            {
                if (_SupplierToDConsiderations == null)
                {
                    _SupplierToDConsiderations = ObjectAssembler.GetToDConsiderations(null, TABS.CarrierAccount.SYSTEM, DateTime.Now);
                }
                return _SupplierToDConsiderations;
            }
            set
            {
                _SupplierToDConsiderations = value;
            }
        }

        internal static IList<ToDConsideration> _OwnToDConsiderations;
        public static IList<ToDConsideration> OwnToDConsiderations
        {
            get
            {
                if (_OwnToDConsiderations == null)
                {
                    _OwnToDConsiderations = ObjectAssembler.GetToDConsiderations(CarrierAccount.SYSTEM, null, DateTime.Now);
                }
                return _OwnToDConsiderations;
            }
            set
            {
                _OwnToDConsiderations = value;
            }
        }
        private int _ToDConsiderationID;
        private Zone _Zone;
        private CarrierAccount _Supplier;
        private CarrierAccount _Customer;
        private string _BeginTime;
        private string _EndTime;
        private Nullable<DayOfWeek> _WeekDay;
        private Nullable<DateTime> _HolidayDate;
        private string _HolidayName;
        private ToDRateType _RateType;

        public virtual int ToDConsiderationID
        {
            get { return _ToDConsiderationID; }
            set { _ToDConsiderationID = value; }
        }

        public virtual Zone Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
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

        public virtual Nullable<TimeSpan> BeginTime
        {
            get { if (_BeginTime == null) return null; else return TimeSpan.Parse(_BeginTime); }
            set { _BeginTime = value == null ? null : FormatTime(value.Value); }
        }

        public virtual Nullable<TimeSpan> EndTime
        {
            get { if (_EndTime == null) return null; else return TimeSpan.Parse(_EndTime); }
            set { _EndTime = value == null ? null : FormatTime(value.Value); }
        }

        public virtual Nullable<DayOfWeek> WeekDay
        {
            get { return _WeekDay; }
            set { _WeekDay = value; }
        }

        public virtual Nullable<DateTime> HolidayDate
        {
            get { return _HolidayDate; }
            set { _HolidayDate = value; }
        }

        public virtual string HolidayName
        {
            get { return _HolidayName; }
            set { _HolidayName = value; }
        }

        public virtual ToDRateType RateType
        {
            get { return _RateType; }
            set { _RateType = value; }
        }

        public string DefinitionDisplay
        {
            get
            {
                string beginDate = "";
                string endDate = "";
                if (this.BeginTime.HasValue && this.EndTime.HasValue)
                {
                    beginDate = DateTime.Today.Add(this.BeginTime.Value).ToString("HH:mm:ss");
                    endDate = DateTime.Today.Add(this.EndTime.Value).ToString("HH:mm:ss");
                }
                string definition = "";
                switch (RateType)
                {
                    case ToDRateType.OffPeak:
                        definition = string.Format("Offpeak ({0} from {1} to {2} on {3}) ", this.WeekDay != null ? ("" + this.WeekDay) : "Everyday", beginDate, endDate, this.Zone); break;
                    case ToDRateType.Weekend:
                        definition = string.Format("Weekend ({0} from {1} to {2} on {3})", this.WeekDay, beginDate, endDate, this.Zone);
                        break;
                    case ToDRateType.Holiday:
                        definition = string.Format("Holiday ({0} - {1:MMMM dd} on {2})", this.HolidayName, this.HolidayDate, this.Zone);
                        break;
                }
                return definition;
            }
        }

        public bool WasActive(DateTime when)
        {
            if (!GetIsEffective(this, when)) return false;

            string time = when.ToString("HH:mm:ss.fff");

            // Assume effective
            bool isActive = true;

            // In any 
            if (this._BeginTime != null && (time.CompareTo(_BeginTime) < 0 || time.CompareTo(_EndTime) > 0)) isActive = false;
            if (this.WeekDay != null && when.DayOfWeek != WeekDay.Value) isActive = false;
            if (this.HolidayDate != null && (this.HolidayDate.Value.Month != when.Month || this.HolidayDate.Value.Day != when.Day)) isActive = false;

            return isActive;
        }

        public bool IsActive
        {
            get
            {
                return WasActive(DateTime.Now);
            }
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
            return this.DefinitionDisplay;
        }

        public double ActiveRateValue(RateBase rate)
        {
            double activeRate = (double)rate.Value.Value;
            switch (this.RateType)
            {
                case ToDRateType.OffPeak:
                    activeRate = rate.OffPeakRate.HasValue && rate.OffPeakRate.Value > 0 ? (double)rate.OffPeakRate.Value : (double)rate.Value.Value;
                    break;
                case ToDRateType.Weekend:
                case ToDRateType.Holiday:
                    activeRate = rate.WeekendRate.HasValue && rate.WeekendRate.Value > 0 ? (double)rate.WeekendRate.Value : (double)rate.Value.Value;
                    break;
            }
            return activeRate;
        }

        // to compare 2 Tod Considerations 
        public string DefinitionCompare
        {
            get
            {
                return Supplier.Name + Zone.Name + Customer.Name + RateType.ToString();
            }
        }

        public string DefinitionCompareNoType
        {
            get
            {
                return Supplier.Name + Zone.Name + Customer.Name;
            }
        }

        public bool CompareBeginEndEffectiveDate(ToDConsideration Other)
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

        public bool compareDateTime(ToDConsideration Other)
        {
            //bool ConditionBeginEffectiveDate = this.BeginEffectiveDate >= Other.BeginEffectiveDate && ((Other.EndEffectiveDate.HasValue) ? this.BeginEffectiveDate < Other.EndEffectiveDate : true);
            //bool ConditionEndEffectiveDate = (this.EndEffectiveDate.HasValue) ? this.EndEffectiveDate >= Other.BeginEffectiveDate && ((Other.EndEffectiveDate.HasValue) ? this.EndEffectiveDate <= Other.EndEffectiveDate : true) : !Other.EndEffectiveDate.HasValue || this.BeginEffectiveDate < Other.BeginEffectiveDate;
            bool ConditionBeginEndEffectiveDate = CompareBeginEndEffectiveDate(Other);
            bool ConditionBeginTime = this.BeginTime >= Other.BeginTime && this.BeginTime <= Other.EndTime;
            bool ConditionEndTime = this.EndTime >= Other.BeginTime && this.EndTime <= Other.EndTime;
            bool ConditionWeekDay = this.WeekDay == Other.WeekDay;
            //if ((ConditionBeginEffectiveDate || ConditionEndEffectiveDate) && (ConditionBeginTime || ConditionEndTime) && (ConditionWeekDay))
            //    return true;
            if ((ConditionBeginEndEffectiveDate) && (ConditionBeginTime || ConditionEndTime) && (ConditionWeekDay))
                return true;
            return false;
        }

        public bool ConflictWith(ToDConsideration Other)
        {
            if (this.DefinitionCompare.Equals(Other.DefinitionCompare))
            {
                switch (Other.RateType)
                {
                    case ToDRateType.OffPeak:
                        if (this.WeekDay == Other.WeekDay)
                            return compareDateTime(Other);
                        return false;
                    case ToDRateType.Weekend:
                        return compareDateTime(Other);
                    case ToDRateType.Holiday:
                        return (this.HolidayDate.Value.Day == Other.HolidayDate.Value.Day
                              && this.HolidayDate.Value.Month == Other.HolidayDate.Value.Month
                              && CompareBeginEndEffectiveDate(Other));
                    default:
                        return false;
                }
            }
            else
                return false;

        }

        public bool NoTypeConflictWith(ToDConsideration Other)
        {
            if (this.DefinitionCompareNoType.Equals(Other.DefinitionCompareNoType))
            {
                if (this.WeekDay == Other.WeekDay)
                    return compareDateTime(Other);
                return false;
            }
            else
                return false;
        }

        public bool HolidayConflictWith(ToDConsideration Other)
        {
            if (this.DefinitionCompareNoType.Equals(Other.DefinitionCompareNoType))
            {
                if (this.RateType == ToDRateType.Holiday)
                {
                    if (this.HolidayDate.Value.DayOfWeek == Other.WeekDay)
                        return true;
                    return false;
                }
            }
            return false;
        }

    }
}