using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportDataListSchema
    {
        public Dictionary<Guid, VRAutomatedReportDataSubTableSchema> SubTablesSchemas { get; set; }
        public Dictionary<string, VRAutomatedReportDataFieldSchema> FieldSchemas { get; set; }
    }
    public class VRAutomatedReportDataSubTableSchema
    {
        public string SubTableTitle { get; set; }
        public Dictionary<string, VRAutomatedReportDataFieldSchema> FieldSchemas { get; set; }
    }
}
