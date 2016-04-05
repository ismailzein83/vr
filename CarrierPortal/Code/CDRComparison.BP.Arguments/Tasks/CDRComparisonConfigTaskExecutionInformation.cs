using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace CDRComparison.BP.Arguments
{
    public class CDRComparisonConfigTaskExecutionInformation : BPTaskExecutionInformation
    {
        public string SystemCDRSourceConfigName { get; set; }
        public string PartnerCDRSourceConfigName { get; set; }
    }
}
