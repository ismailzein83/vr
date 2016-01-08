using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DWAccountCase
    {
        public int CaseID { get; set; }
        public int CaseStatus { get; set; }
        public int NetType { get; set; }
        public int StrategyID { get; set; }
        public bool IsDefault { get; set; }
        public int PeriodID { get; set; }
        public int SuspicionLevelID { get; set; }
        public DateTime CaseGenerationTime { get; set; }
        public int StrategyUser { get; set; }
        public int? CaseUser { get; set; }
        public string AccountNumber { get; set; }
       
    }
}
