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

            _levelsByPriority = new List<StrategyLevelWithCriterias>();


            foreach (StrategyLevel i in strategy.StrategyLevels)
            {

                StrategyLevelWithCriterias strategyLevelWithCriterias = new StrategyLevelWithCriterias();
                strategyLevelWithCriterias.SuspectionLevelId = i.SuspectionLevelId;


                strategyLevelWithCriterias.LevelCriteriasThresholdPercentage = new List<LevelCriteria_Threshold_Percentage>();

                foreach (var j in i.StrategyLevelCriterias)
                {
                    LevelCriteria_Threshold_Percentage levelCriterias_Threshold_Percentage = new LevelCriteria_Threshold_Percentage();
                    levelCriterias_Threshold_Percentage.CriteriaDefinitions = new CriteriaDefinition() { CompareOperator = dictionary_int_criteriaDefinition.Where(x => x.Value.CriteriaId == j.CriteriaId).FirstOrDefault().Value.CompareOperator, CriteriaId = j.CriteriaId, Description = dictionary_int_criteriaDefinition.Where(x => x.Value.CriteriaId == j.CriteriaId).FirstOrDefault().Value.Description };
                    levelCriterias_Threshold_Percentage.Percentage=j.Percentage;
                    levelCriterias_Threshold_Percentage.Threshold=strategy.StrategyCriterias.Where(x=>x.CriteriaId==j.CriteriaId).FirstOrDefault().Threshold;
                    
                    strategyLevelWithCriterias.LevelCriteriasThresholdPercentage.Add(levelCriterias_Threshold_Percentage);
                }


                _levelsByPriority.Add(strategyLevelWithCriterias);

            }

            
        }

        public bool IsNumberSuspicious(NumberProfile profile, out SuspiciousNumber suspiciousNumber)
        {
            suspiciousNumber = null;
           // return false;
            //CriteriaDefinition c = new CriteriaDefinition();
            Dictionary<int, Decimal> dict_criteriaValues = new Dictionary<int, decimal>();
            bool IsSuspicious = false;

            foreach (StrategyLevelWithCriterias strategyLevelWithCriterias in _levelsByPriority)
            {
                IsSuspicious = false;

                dict_criteriaValues = new Dictionary<int, decimal>();

                foreach (LevelCriteria_Threshold_Percentage levelCriteria in strategyLevelWithCriterias.LevelCriteriasThresholdPercentage)
                {
                    Decimal d = 0;
                    //criteriaValues.TryGetValue(LCriteria.Criteria.CriteriaId, out d);

                    CriteriaManager m = new CriteriaManager();
                    d = m.GetCriteriaValue(levelCriteria.CriteriaDefinitions, profile);

                    Decimal ThrCri = d / levelCriteria.Threshold;

                    dict_criteriaValues.Add(levelCriteria.CriteriaDefinitions.CriteriaId, ThrCri);

                    if (levelCriteria.CriteriaDefinitions.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual)
                    {
                        if (ThrCri >= levelCriteria.Percentage)
                        {
                            IsSuspicious = true;
                        }
                        else
                        {
                            IsSuspicious = false;
                            break;
                        }
                    }
                    else if (levelCriteria.CriteriaDefinitions.CompareOperator == CriteriaCompareOperator.LessThanorEqual)
                    {
                        if (ThrCri <= levelCriteria.Percentage)
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
                    suspiciousNumber.CriteriaValues = dict_criteriaValues;
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
