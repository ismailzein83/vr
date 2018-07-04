using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportDataItem
    {
        public Dictionary<Guid, VRAutomatedReportDataSubTable> SubTables { get; set; }
        public Dictionary<string, VRAutomatedReportDataFieldValue> Fields { get; set; }
    }
    public class VRAutomatedReportDataSubTable
    {
        public Dictionary<string, VRAutomatedReportDataSubTableFieldInfo> Fields { get; set; }
    }
    public class VRAutomatedReportDataSubTableFieldInfo
    {
        public List<VRAutomatedReportDataFieldValue> FieldsValues { get; set; }

    }
}
