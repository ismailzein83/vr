using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportDataList
    {
        public VRAutomatedReportDataItem SummaryDataItem { get; set; }
        public Dictionary<Guid, VRAutomatedReportDataSubTable> ItemTables { get; set; }
        public List<VRAutomatedReportDataItem> Items { get; set; }
    }
}
