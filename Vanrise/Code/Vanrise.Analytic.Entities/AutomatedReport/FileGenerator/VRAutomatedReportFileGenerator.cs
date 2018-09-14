using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportFileGenerator
    {
        public Guid VRAutomatedReportFileGeneratorId { get; set; }
        public string Name { get; set; }
        public bool CompressFile { get; set; }
        public VRAutomatedReportFileGeneratorSettings Settings { get; set; }
    }
}
