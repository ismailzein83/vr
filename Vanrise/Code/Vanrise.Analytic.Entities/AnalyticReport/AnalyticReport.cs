using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum AccessType { Public = 0, Private = 1 }
    public class AnalyticReport
    {
        public Guid AnalyticReportId { get; set; }
        public int UserID { get; set; }
        public AccessType AccessType { get; set; }
        public string Name { get; set; }
        public AnalyticReportSettings Settings { get; set; }
    }
}
