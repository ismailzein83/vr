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
        private List<StrategyLevelWithCriterias> _levelsByPriority;

        public FraudManager(Strategy strategy) 
        {
            LoadLevels(strategy);
        }

        void LoadLevels(Strategy strategy)
        {
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
            
            //List<StrategyLevelWithCriterias> levelsByPriority = new List<StrategyLevelWithCriterias>();
            foreach(var level in strategy.Levels.OrderByDescending(itm => itm.SuspectionLevel))
            {
                //TODO: Fill Levels
                StrategyLevelWithCriterias s = new StrategyLevelWithCriterias();
                s.SuspectionLevel = level.SuspectionLevel;
                s.Criterias = new List<LevelCriteria>();
                LevelCriteria lc = new LevelCriteria();

                foreach (KeyValuePair<int, CriteriaDefinition> pair in criterias)
                {
                    foreach (var criteria in strategy.Criterias.OrderByDescending(itm => itm.CriteriaId))
                    {
                        if(pair.Key == criteria.CriteriaId)
                        {
                            lc.Criteria = pair.Value;
                            lc.Threshold = criteria.Threshold;
                            
                        }
                        foreach (var c in level.Criterias.OrderByDescending(itm => itm.CriteriaId))
                        {
                            if (c.CriteriaId == criteria.CriteriaId)
                            {
                                lc.Percentage = c.Percentage;
                            }
                        }
                    }
                }
                s.Criterias.Add(lc);
                _levelsByPriority.Add(s);
            }
        }

        public bool IsNumberSuspicious(NumberProfile profile, out SuspiciousNumber suspiciousNumber)
        {
            suspiciousNumber = null;
            return false;
            //CriteriaDefinition c = new CriteriaDefinition();
            Dictionary<int, Decimal> criteriaValues = new Dictionary<int, decimal>();
            bool IsSuspicious = false;

            foreach (StrategyLevelWithCriterias StrategyLevel in _levelsByPriority)
            {
                IsSuspicious = false;
                
                criteriaValues = new Dictionary<int, decimal>();

                foreach (LevelCriteria LCriteria in StrategyLevel.Criterias)
                {
                    Decimal d = 0;
                    //criteriaValues.TryGetValue(LCriteria.Criteria.CriteriaId, out d);

                    CriteriaManager m = new CriteriaManager();
                    d = m.GetCriteriaValue(LCriteria.Criteria, profile);

                    Decimal ThrCri = d / LCriteria.Threshold;
                    criteriaValues.Add(LCriteria.Criteria.CriteriaId, ThrCri);
                    if (LCriteria.Criteria.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual)
                    {
                        if (ThrCri >= LCriteria.Percentage)
                        {
                            IsSuspicious = true;
                        }
                        else
                        {
                            IsSuspicious = false;
                            break;
                        }
                    }
                    else if (LCriteria.Criteria.CompareOperator == CriteriaCompareOperator.LessThanorEqual)
                    {
                        if (ThrCri <= LCriteria.Percentage)
                        {
                            IsSuspicious = true;
                        }
                        else
                        {
                            IsSuspicious = false;
                            break;
                        }
                    }
                }


                if (IsSuspicious)
                {
                    suspiciousNumber = new SuspiciousNumber();
                    suspiciousNumber.Number = profile.subscriberNumber;
                    suspiciousNumber.SuspectionLevel = StrategyLevel.SuspectionLevel;
                    suspiciousNumber.CriteriaValues = criteriaValues;
                    return IsSuspicious;
                }
            }
            return IsSuspicious;
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

            public Decimal Percentage { get; set; }
        }

        #endregion
    }
}
