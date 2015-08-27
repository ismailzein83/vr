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
        public string CustomerNameSuffix { get; set; }
        public string CarrierName { get; set; }
        public string DefinitionDisplayS { get; set; }
        public bool IsActive { get; set; }

    }
}
