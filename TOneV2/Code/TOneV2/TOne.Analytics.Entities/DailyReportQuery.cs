using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailyReportQuery
    {
        public List<int> CarrierIds { get; set; }
        public List<int> SupplierIds { get; set; }
        public List<int> ZoneIds { get; set; }
        public DateTime TargetDate { get; set; }
    }
}
