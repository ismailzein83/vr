using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FraudManager
    {
        private IEnumerable<StrategyLevelWithCriterias> _levelsByPriority;

        public FraudManager(int strategyId) 
        {
            LoadLevels(strategyId);
        }

        void LoadLevels(int strategyId)
        {
            StrategyManager strategyManager = new StrategyManager();
            var strategy = strategyManager.GetStrategy(strategyId);
            if (strategy == null)
                throw new ArgumentNullException("strategy");
            if(strategy.Levels == null)
                throw new ArgumentNullException("strategy.Levels");
            if(strategy.Criterias == null)
                throw new ArgumentNullException("strategy.CriteriaThresholds");

            var criteriaManager = new CriteriaManager();
            Dictionary<int, CriteriaDefinition> criterias = criteriaManager.GetCriteriaDefinitions();
            if (criterias == null)
                throw new ArgumentNullException("criterias");

            _levelsByPriority = new List<StrategyLevelWithCriterias>();
            foreach(var level in strategy.Levels.OrderByDescending(itm => itm.SuspectionLevel))
            {
                //TODO: Fill Levels
            }
        }

        public bool IsNumberSuspicious(NumberProfile numberProfile, out SuspiciousNumber suspiciousNumber)
        {
            //TODO
            throw new NotImplementedException();
        }

        #region Private Classes

        private class StrategyLevelWithCriterias
        {
            public int SuspectionLevel { get; set; }

            public List<LevelCriteria> Criterias { get; set; }
        }

        private class LevelCriteria
        {
            public CriteriaDefinition Criteria { get; set; }

            public Decimal Threshold { get; set; }

            public int Percentage { get; set; }
        }

        #endregion
    }
}
