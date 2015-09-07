using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class BasePriceListDetail
    {
        public Int64 RateID { get; set; }

        public int PriceListID { get; set; }

        public int ZoneID { get; set; }
        public string ZoneName { get; set; }

        public Decimal Rate { get; set; }
        public Decimal OffPeakRate { get; set; }
        public Decimal WeekendRate { get; set; }
        public Change Change { get; set; }
        public Int16 ServicesFlag { get; set; }
        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
        public string Notes { get; set; }
        public string CodeGroup { get; set; }	
    }
}
