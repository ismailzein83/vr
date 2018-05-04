using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportResolvedDataList
    {
        public Dictionary<string, VRAutomatedReportFieldInfo> FieldInfos { get; set; }

        public List<VRAutomatedReportResolvedDataItem> Items { get; set; }
    }
}
