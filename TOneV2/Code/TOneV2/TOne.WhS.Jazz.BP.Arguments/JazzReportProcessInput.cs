using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Jazz.BP.Arguments
{
    public class JazzReportProcessInput :Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public override string GetTitle()
        {
            return "Report Process";
        }
    }
}
