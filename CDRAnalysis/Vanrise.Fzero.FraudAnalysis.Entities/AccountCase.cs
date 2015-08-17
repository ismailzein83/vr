using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCase
    {
        public string AccountNumber { get; set; }

        public int StatusID { get; set; }

        public DateTime? ValidTill { get; set; }

        public int StrategyId { get; set; }

        public int UserId { get; set; }

        public DateTime LogDate { get; set; }

    }



    public class CasesSummary
    {
        public string StatusName { get; set; }

        public int CountCases { get; set; }

    }

    public class StrategyCases
    {
        public string StrategyName { get; set; }

        public int CountCases { get; set; }

    }

    public class BTSCases
    {
        public int? BTS_Id { get; set; }

        public int CountCases { get; set; }

    }


    public class CellCases
    {
        public string Cell_Id { get; set; }

        public int CountCases { get; set; }

    }


}
