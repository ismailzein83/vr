using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRReportGeneration
    {
        public long ReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public VRReportGenerationSettings Settings { get; set; }
    }
    public class VRReportGenerationSettings
    {
        public List<VRAutomatedReportQuery> Queries { get; set; }
        public VRReportGenerationAction ReportAction { get; set; }
    }
    public abstract class VRReportGenerationAction
    {
        public abstract string ActionTypeName { get; }
        public abstract Guid ConfigId { get; }
    }
}
