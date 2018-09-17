using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TestCallAnalysis.BP.Arguments
{
    public class CDRCorrelationAndAnalysisProcessInput : BaseProcessInputArgument
    {
        public TimeSpan DateTimeMargin { get; set; }

        public override string GetTitle()
        {
            return "CDR Correlation And Analysis Process";
        }
    }
}
