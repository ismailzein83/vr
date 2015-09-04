using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SpecialInput
    {
        public string accountNumber { get; set; }
        public CaseStatus caseStatus { get; set; }
        public DateTime? validTill { get; set; }
    }
}
