using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRReportGenerationDetail
    {
        public long ReportId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTime { get; set; }
        public String CreatedBy { get; set; }
        public String LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public String AccessLevel { get; set; }
        public bool DoesUserHaveManageAccess { get; set; }

    }
}
