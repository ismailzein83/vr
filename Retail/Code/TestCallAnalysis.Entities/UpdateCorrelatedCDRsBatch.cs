using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public class UpdateCorrelatedCDRsBatch
    {
        public Dictionary<long, string> ExcitingCaseCDRs { get; set; }

        public UpdateCorrelatedCDRsBatch()
        {
            ExcitingCaseCDRs = new Dictionary<long, string>();
        }
    }
}
