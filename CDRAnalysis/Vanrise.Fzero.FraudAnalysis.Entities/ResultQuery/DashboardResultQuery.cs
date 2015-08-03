using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class FraudResultQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SuspicionLevelsList { get; set; }

        public string StrategiesList { get; set; }
        public string CaseStatusesList { get; set; }

    }
}
