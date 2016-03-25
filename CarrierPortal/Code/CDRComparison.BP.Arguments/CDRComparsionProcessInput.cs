using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.BP.Arguments
{
    public class CDRComparsionProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public CDRSource SystemCDRSource { get; set; }

        public CDRSource PartnerCDRSource { get; set; }

        public override string GetTitle()
        {
            return "CDR Comparsion";
        }
    }
}
