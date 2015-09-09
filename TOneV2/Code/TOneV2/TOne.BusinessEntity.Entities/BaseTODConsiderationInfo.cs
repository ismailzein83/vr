using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class BaseTODConsiderationInfo
    {
        public long ToDConsiderationID { get; set; }
        public int ZoneID { get; set; } 
        public String SupplierID { get; set; }
        public String CustomerID { get; set; }
        public String BeginTime { get; set; }
        public String EndTime { get; set; }
        public Nullable<DayOfWeek> WeekDay { get; set; }
        public Nullable<DateTime> HolidayDate { get; set; }
        public string HolidayName { get; set; }
        public ToDRateType RateType { get; set; }
        public DateTime? BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int UserID {get; set; }
        public string ZoneName { get; set; }
        public string CarrierName { get; set; }
        public string DefinitionDisplayS { get; set; }
        public bool IsActive { get; set; }
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
        public bool WasActive(DateTime when)
        {
            string time = when.ToString("HH:mm:ss.fff");

            // Assume effective
            bool isActive = true;

            // In any 
            if (this.BeginTime != null && (time.CompareTo(this.BeginTime) < 0 || time.CompareTo(this.EndTime) > 0)) isActive = false;
            if (this.WeekDay != null && when.DayOfWeek != this.WeekDay.Value) isActive = false;
            if (this.HolidayDate != null && (this.HolidayDate.Value.Month != when.Month || this.HolidayDate.Value.Day != when.Day)) isActive = false;

            return isActive;
        }

    }

}
