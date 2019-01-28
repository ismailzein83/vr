using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class VRScheduledReportProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Scheduled Report";
        }
        public Guid ScheduledReportId { get; set; }
    }
}
