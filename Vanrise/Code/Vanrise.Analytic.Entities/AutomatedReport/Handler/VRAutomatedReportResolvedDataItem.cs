using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportResolvedDataItem
    {
        public Dictionary<Guid, VRAutomatedReportResolvedDataItemSubTable> SubTables { get; set; }

        public Dictionary<string, VRAutomatedReportResolvedDataFieldValue> Fields { get; set; }
    }
    public class VRAutomatedReportResolvedDataItemSubTable
    {
        public Dictionary<string, VRAutomatedReportResolvedDataItemSubTableFieldInfo> Fields { get; set; }
    }
    public class VRAutomatedReportResolvedDataItemSubTableFieldInfo
    {
        public List<VRAutomatedReportResolvedDataFieldValue> FieldValues { get; set; }

    }
}
