using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountSuspicionDetail
    {
        public int DetailID { get; set; }
        public int? AnalystID { get; set; }
        public string AnalystName { get; set; }
        public SuspicionOccuranceStatus StatusID { get; set; }
        public DateTime? LogDate { get; set; }
    }
}
