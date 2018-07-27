using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.GenericData.BP.Arguments
{
    public class CDRCorrelationProcessInput : BaseProcessInputArgument
    {
        public TimeSpan DateTimeMargin { get; set; }

        public TimeSpan DurationMargin { get; set; }

        public override string GetTitle()
        {
            return "CDR Correlation Process";
        }
    }
}