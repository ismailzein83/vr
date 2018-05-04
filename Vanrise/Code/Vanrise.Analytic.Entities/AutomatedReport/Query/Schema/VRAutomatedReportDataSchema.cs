using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportDataSchema
    {
        public Dictionary<string, VRAutomatedReportDataListSchema> ListSchemas { get; set; }

        public Dictionary<string, VRAutomatedReportDataFieldSchema> FieldSchemas { get; set; }
    }
}
