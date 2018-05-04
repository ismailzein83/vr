using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportDataResult
    {
        public Dictionary<string, VRAutomatedReportDataList> Lists { get; set; }

        public Dictionary<string, VRAutomatedReportDataFieldValue> Fields { get; set; }
    }
}
