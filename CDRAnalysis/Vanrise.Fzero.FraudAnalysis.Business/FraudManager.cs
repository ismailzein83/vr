using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FraudManager
    {
        private List<StrategyLevelWithCriterias> _levelsByPriority;
        CriteriaManager _criteriaManager = new CriteriaManager();
        public int StrategyId { get; set; }

        public FraudManager(Strategy strategy) 
        {
            StrategyId = strategy.Id;
            LoadLevelsByPriority(strategy);
        }

        void LoadLevelsByPriority(Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException("strategy");
            if (strategy.StrategyLevels == null)
                throw new ArgumentNullException("strategy.StrategyLevels");
            if (strategy.StrategyCriterias == null)
                throw new ArgumentNullException("strategy.StrategyCriterias");

            var criteriaManager = new CriteriaManager();
            Dictionary<int, CriteriaDefinition> criteriaDefinitions = criteriaManager.GetCriteriaDefinitions();
            if (criteriaDefinitions == null)
                throw new ArgumentNullException("criteriaDefinitions");

            _levelsByPriority = new List<StrategyLevelWithCriterias>();


            foreach (StrategyLevel i in strategy.StrategyLevels)
            {

                StrategyLevelWithCriterias strategyLevelWithCriterias = new StrategyLevelWithCriterias();
                strategyLevelWithCriterias.SuspectionLevelId = i.SuspectionLevelId;


                strategyLevelWithCriterias.LevelCriteriasThresholdPercentage = new List<LevelCriteriaInfo>();

                foreach (var j in i.StrategyLevelCriterias)
                {
                    LevelCriteriaInfo levelCriteriasThresholdPercentage = new LevelCriteriaInfo();
                    levelCriteriasThresholdPercentage.CriteriaDefinitions = new CriteriaDefinition() { CompareOperator = criteriaDefinitions.Where(x => x.Value.CriteriaId == j.CriteriaId).FirstOrDefault().Value.CompareOperator, CriteriaId = j.CriteriaId, Description = criteriaDefinitions.Where(x => x.Value.CriteriaId == j.CriteriaId).FirstOrDefault().Value.Description };
                    levelCriteriasThresholdPercentage.Percentage=j.Percentage;
                    levelCriteriasThresholdPercentage.Threshold=strategy.StrategyCriterias.Where(x=>x.CriteriaId==j.CriteriaId).FirstOrDefault().Threshold;
                    
                    strategyLevelWithCriterias.LevelCriteriasThresholdPercentage.Add(levelCriteriasThresholdPercentage);
                }


                _levelsByPriority.Add(strategyLevelWithCriterias);

            }

            
        }

        public bool IsNumberSuspicious(NumberProfile profile, out SuspiciousNumber suspiciousNumber, int StrategyId)
        {
            suspiciousNumber = null;

            Dictionary<int, Decimal> criteriaValuesThresholds;
            Dictionary<int, Decimal> criteriaValues ;
            bool IsSuspicious = false;

            foreach (StrategyLevelWithCriterias strategyLevelWithCriterias in _levelsByPriority)
            {
                IsSuspicious = false;

                criteriaValues = new Dictionary<int, decimal>();
                criteriaValuesThresholds = new Dictionary<int, decimal>();

                foreach (LevelCriteriaInfo LevelCriteriaThresholdPercentage in strategyLevelWithCriterias.LevelCriteriasThresholdPercentage)
                {
                    Decimal criteriaValue;
                    if (!criteriaValues.TryGetValue(LevelCriteriaThresholdPercentage.CriteriaDefinitions.CriteriaId, out criteriaValue))
                    {
                        criteriaValue = _criteriaManager.GetCriteriaValue(LevelCriteriaThresholdPercentage.CriteriaDefinitions, profile) ;
                        criteriaValues.Add(LevelCriteriaThresholdPercentage.CriteriaDefinitions.CriteriaId, criteriaValue);
                    }


                    decimal criteriaValuesThreshold ;


                    if (!criteriaValuesThresholds.TryGetValue(LevelCriteriaThresholdPercentage.CriteriaDefinitions.CriteriaId, out criteriaValuesThreshold))
                    {
                        criteriaValuesThreshold = criteriaValue / LevelCriteriaThresholdPercentage.Threshold;
                        criteriaValuesThresholds.Add(LevelCriteriaThresholdPercentage.CriteriaDefinitions.CriteriaId, criteriaValuesThreshold);
                    }




                    if (LevelCriteriaThresholdPercentage.CriteriaDefinitions.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual)
                    {
                        if (criteriaValuesThreshold >= LevelCriteriaThresholdPercentage.Percentage)
                        {
                            IsSuspicious = true;
                        }
                        else
                        {
                            IsSuspicious = false;
                            break;
                        }
                    }
                    else if (LevelCriteriaThresholdPercentage.CriteriaDefinitions.CompareOperator == CriteriaCompareOperator.LessThanorEqual)
                    {
                        if (criteriaValuesThreshold <= LevelCriteriaThresholdPercentage.Percentage)
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
                    suspiciousNumber.Number = profile.SubscriberNumber;
                    suspiciousNumber.SuspectionLevel = strategyLevelWithCriterias.SuspectionLevelId;
                    suspiciousNumber.CriteriaValues = criteriaValuesThresholds;
                    suspiciousNumber.DateDay = profile.FromDate;
                    suspiciousNumber.Period = profile.Period;
                    suspiciousNumber.StrategyId = StrategyId;
                    return IsSuspicious;
                }
            }
            return IsSuspicious;
        }

        #region Private Classes

        private class StrategyLevelWithCriterias
        {
            public int SuspectionLevelId { get; set; }

            public List<LevelCriteriaInfo> LevelCriteriasThresholdPercentage { get; set; }
        }

        private class LevelCriteriaInfo
        {
            public CriteriaDefinition CriteriaDefinitions { get; set; }

            public Decimal Threshold { get; set; }

            public Decimal Percentage { get; set; }
        }

        #endregion
    }
}
