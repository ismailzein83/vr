using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SuspiciousNumber
    {
        public string Number { get; set; }

        public int SuspectionLevel { get; set; }

        public Dictionary<int,Decimal> CriteriaValues { get; set; }
    }
}
