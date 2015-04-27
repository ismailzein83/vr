using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Strategy
    {
        //public string Description { get; set; }

        //public int UserId { get; set; }

        //public DateTime CreationDate { get; set; }

        //public string Name { get; set; }

        //public bool IsDefault { get; set; }

        public List<StrategyCriteria> Criterias { get; set; }

        public List<StrategyLevel> Levels { get; set; }
    }

    public class StrategyCriteria
    {
        public int CriteriaId { get; set; }

        public Decimal Threshold { get; set; }
    }

    public class StrategyLevel
    {
        public int SuspectionLevel { get; set; }

        public List<StrategyLevelCriteria> Criterias { get; set; }
    }

    public class StrategyLevelCriteria
    {
        public int CriteriaId { get; set; }

        public decimal Percentage { get; set; }
    }
}

