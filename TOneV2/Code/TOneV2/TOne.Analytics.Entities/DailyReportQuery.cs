using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailyReportQuery
    {
        public List<int> SelectedZoneIDs { get; set; }
        public List<string> SelectedCustomerIDs { get; set; }
        public List<string> SelectedSupplierIDs { get; set; }
        public DateTime TargetDate { get; set; }
    }
}
