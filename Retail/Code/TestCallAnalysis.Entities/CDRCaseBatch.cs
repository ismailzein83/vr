using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TestCallAnalysis.Entities
{
    public class CDRCaseBatch
    {
        public List<TCAnalCorrelatedCDR> CaseCDRsToInsert { get; set; }
    }
}
