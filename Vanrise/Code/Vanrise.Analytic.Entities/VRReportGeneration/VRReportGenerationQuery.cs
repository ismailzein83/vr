using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum ReportOwner { AllReports = 0, OnlyMyReports = 1 }
    public class VRReportGenerationQuery
    {
        public string Name { get; set; }
        public ReportOwner Owner { get; set; }
    }
}
