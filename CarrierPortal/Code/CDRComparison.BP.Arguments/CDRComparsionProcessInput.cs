using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.BP.Arguments
{
    public class CDRComparsionProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {

        public override string GetTitle()
        {
            return "CDR Comparsion";
        }
    }
}
