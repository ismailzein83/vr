using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Strategy
    {
        public int Id { get; set; }

        public List<StrategyCriteria> StrategyCriterias { get; set; }

        public List<StrategyLevel> StrategyLevels { get; set; }
    }

    public class StrategyCriteria
    {
        public int CriteriaId { get; set; }

        public Decimal Threshold { get; set; }
    }

    public class StrategyLevel
    {
        public int SuspectionLevelId { get; set; }

        public List<StrategyLevelCriteria> StrategyLevelCriterias { get; set; }
    }

    public class StrategyLevelCriteria
    {
        public int CriteriaId { get; set; }

        public decimal Percentage { get; set; }
    }
}