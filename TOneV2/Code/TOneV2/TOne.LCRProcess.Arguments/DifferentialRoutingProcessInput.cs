using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCRProcess.Arguments
{
    public class DifferentialRoutingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("Differential Routing Process");
        }
    }
}
