using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace TestCallAnalysis.Business
{
    public enum ReportType { ReportType1 = 1, ReportType2 = 2, ReportType3 = 3}
    public class SaveReportHandler : VRAutomatedReportHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("2884E0F4-6FB0-4CC2-8F18-EE73184AB9AB"); }
        }

        public ReportType ReportType { get; set; }
        public Guid ReportQueryId { get; set; }
        public string ListName { get; set; }
        public string RecordId { get; set; }
        public override void Execute(IVRAutomatedReportHandlerExecuteContext context)
        {
            
        }
    }
}
