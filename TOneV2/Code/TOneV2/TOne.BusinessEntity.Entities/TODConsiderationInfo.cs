using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class TODConsiderationInfo
    {
        public long ID { get; set; }
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
        public int UserId { get; set; }
        public string ZoneName { get; set; }
        public string CustomerNameSuffix { get; set; }
        public string CustomerName { get; set; }
        public string DefinitionDisplay { get; set; }

    }
}
