using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountSuspicionDetailQuery
    {
        public string AccountNumber { get; set; }
        
        public DateTime ExecutionDate { get; set; }
    }
}
