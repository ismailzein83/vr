using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RateTypeConfiguration
    {
        public int OffPeakRateTypeId { get; set; }
        public int WeekendRateTypeId { get; set; }
        public int HolidayRateTypeId { get; set; }
    }
}
