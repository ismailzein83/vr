using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FraudManager
    {
        private List<StrategyLevelWithCriterias> levelsByPriority;

        public FraudManager(Strategy strategy) 
        {
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
            Dictionary<int, CriteriaDefinition> dictionary_int_criteriaDefinition = criteriaManager.GetCriteriaDefinitions();
            if (dictionary_int_criteriaDefinition == null)
                throw new ArgumentNullException("dictionary_int_criteriaDefinitions");

            levelsByPriority = new List<StrategyLevelWithCriterias>();


            foreach (StrategyLevel i in strategy.StrategyLevels)
            {

                StrategyLevelWithCriterias strategyLevelWithCriterias = new StrategyLevelWithCriterias();
                strategyLevelWithCriterias.SuspectionLevelId = i.SuspectionLevelId;


                strategyLevelWithCriterias.LevelCriteriasThresholdPercentage = new List<LevelCriteria_Threshold_Percentage>();

                foreach (var j in i.StrategyLevelCriterias)
                {
                    LevelCriteria_Threshold_Percentage levelCriteriasThresholdPercentage = new LevelCriteria_Threshold_Percentage();
                    levelCriteriasThresholdPercentage.CriteriaDefinitions = new CriteriaDefinition() { CompareOperator = dictionary_int_criteriaDefinition.Where(x => x.Value.CriteriaId == j.CriteriaId).FirstOrDefault().Value.CompareOperator, CriteriaId = j.CriteriaId, Description = dictionary_int_criteriaDefinition.Where(x => x.Value.CriteriaId == j.CriteriaId).FirstOrDefault().Value.Description };
                    levelCriteriasThresholdPercentage.Percentage=j.Percentage;
                    levelCriteriasThresholdPercentage.Threshold=strategy.StrategyCriterias.Where(x=>x.CriteriaId==j.CriteriaId).FirstOrDefault().Threshold;
                    
                    strategyLevelWithCriterias.LevelCriteriasThresholdPercentage.Add(levelCriteriasThresholdPercentage);
                }


                levelsByPriority.Add(strategyLevelWithCriterias);

            }

            
        }

        public bool IsNumberSuspicious(NumberProfile profile, out SuspiciousNumber suspiciousNumber)
        {
            suspiciousNumber = null;
         
            Dictionary<int, Decimal> criteriaValues ;
            bool IsSuspicious = false;

            foreach (StrategyLevelWithCriterias strategyLevelWithCriterias in levelsByPriority)
            {
                IsSuspicious = false;

                criteriaValues = new Dictionary<int, decimal>();

                foreach (LevelCriteria_Threshold_Percentage LevelCriteriaThresholdPercentage in strategyLevelWithCriterias.LevelCriteriasThresholdPercentage)
                {

                    CriteriaManager criteriaManager = new CriteriaManager();

                    Decimal ValueoverPercentage = criteriaManager.GetCriteriaValue(LevelCriteriaThresholdPercentage.CriteriaDefinitions, profile) / LevelCriteriaThresholdPercentage.Percentage;

                    criteriaValues.Add(LevelCriteriaThresholdPercentage.CriteriaDefinitions.CriteriaId, ValueoverPercentage);

                    if (LevelCriteriaThresholdPercentage.CriteriaDefinitions.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual)
                    {
                        if (ValueoverPercentage >= LevelCriteriaThresholdPercentage.Threshold)
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
                        if (ValueoverPercentage <= LevelCriteriaThresholdPercentage.Threshold)
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
                    suspiciousNumber.CriteriaValues = criteriaValues;
                    suspiciousNumber.DateDay = profile.FromDate;
                    suspiciousNumber.PeriodId = profile.PeriodId;
                    return IsSuspicious;
                }
            }
            return IsSuspicious;
        }

        #region Private Classes

        private class StrategyLevelWithCriterias
        {
            public int SuspectionLevelId { get; set; }

            public List<LevelCriteria_Threshold_Percentage> LevelCriteriasThresholdPercentage { get; set; }
        }

        private class LevelCriteria_Threshold_Percentage
        {
            public CriteriaDefinition CriteriaDefinitions { get; set; }

            public Decimal Threshold { get; set; }

            public Decimal Percentage { get; set; }
        }

        #endregion
    }
}
