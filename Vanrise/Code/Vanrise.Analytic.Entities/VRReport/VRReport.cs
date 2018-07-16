using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRReport
    {
        public long ReportId { get; set; }
        public string Name { get; set; }
        public VRReportSettings Settings { get; set; }
    }
    public class VRReportSettings
    {
        public List<VRAutomatedReportQuery> Queries { get; set; }
        public VRReportAction Action { get; set; }
    }
    public abstract class VRReportAction
    {
        public string ActionName { get; set; }
        public abstract Guid ConfigId { get; }
    }
}
