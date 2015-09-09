using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CustomerToDConsideration
    {
        public int ToDConsiderationID { get; set; }
        public int ZoneId { get; set; }
        public String SupplierId { get; set; }
        public String CustomerId { get; set; }
        public String BeginTime { get; set; }
        public String EndTime { get; set; }
        public Nullable<DayOfWeek> WeekDay { get; set; }
        public Nullable<DateTime> HolidayDate { get; set; }
        public String HolidayName { get; set; }
        public ToDRateType RateType { get; set; }
        public DateTime? BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }

        public CustomerToDConsideration Clone()
        {
            CustomerToDConsideration copy = new CustomerToDConsideration();
            copy.ToDConsiderationID = this.ToDConsiderationID;
            copy.ZoneId = this.ZoneId;
            copy.SupplierId = this.SupplierId;
            copy.CustomerId = this.CustomerId;
            copy.BeginTime = this.BeginTime;
            copy.EndTime = this.EndTime;
            copy.WeekDay = this.WeekDay;
            copy.HolidayDate = this.HolidayDate;
            copy.HolidayName = this.HolidayName;
            copy.RateType = this.RateType;
            copy.BeginEffectiveDate = this.BeginEffectiveDate;
            copy.EndEffectiveDate = this.EndEffectiveDate;

            return copy;
        }

        public bool WasActive(DateTime when)
        {
            if (!GetIsEffective(this.BeginEffectiveDate, this.EndEffectiveDate, when)) return false;

            string time = when.ToString("HH:mm:ss.fff");

            // Assume effective
            bool isActive = true;

            // In any 
            if (this.BeginTime != null && (time.CompareTo(this.BeginTime) < 0 || time.CompareTo(this.EndTime) > 0)) isActive = false;
            if (this.WeekDay != null && when.DayOfWeek != this.WeekDay.Value) isActive = false;
            if (this.HolidayDate != null && (this.HolidayDate.Value.Month != when.Month || this.HolidayDate.Value.Day != when.Day)) isActive = false;

            return isActive;
        }

        public double ActiveRateValue(Rate rate)
        {
            double activeRate = (double)rate.NormalRate;
            switch (this.RateType)
            {
                case ToDRateType.OffPeak:
                    activeRate = rate.OffPeakRate.HasValue && rate.OffPeakRate.Value > 0 ? (double)rate.OffPeakRate.Value : (double)rate.NormalRate;
                    break;
                case ToDRateType.Weekend:
                case ToDRateType.Holiday:
                    activeRate = rate.WeekendRate.HasValue && rate.WeekendRate.Value > 0 ? (double)rate.WeekendRate.Value : (double)rate.NormalRate;
                    break;
            }
            return activeRate;
        }

        public bool GetIsEffective(DateTime? BeginEffectiveDate, DateTime? EndEffectiveDate, DateTime when)
        {
            bool isEffective = BeginEffectiveDate.HasValue ? BeginEffectiveDate.Value <= when : true;
            if (isEffective)
                isEffective = EndEffectiveDate.HasValue ? EndEffectiveDate.Value >= when : true;
            return isEffective;
        }
    }
}
