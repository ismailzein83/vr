using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticReportInfo
    {
        public Guid AnalyticReportId { get; set; }
        public string Name { get; set; }
        public int UserID { get; set; }
        public AccessType AccessType { get; set; }
    }
}
