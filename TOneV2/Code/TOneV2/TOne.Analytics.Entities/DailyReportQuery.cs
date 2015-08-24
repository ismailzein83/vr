using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailyReportQuery
    {
        public List<int> selectedZoneIDs { get; set; }
        public List<string> selectedCustomerIDs { get; set; }
        public List<string> selectedSupplierIDs { get; set; }
        public DateTime targetDate { get; set; }
    }
}
