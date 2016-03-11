using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FraudManager
    {
        private List<StrategyLevelWithCriterias> _levelsByPriority;
        FilterManager _FilterManager = new FilterManager();
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
            if (strategy.StrategyFilters == null)
                throw new ArgumentNullException("strategy.StrategyFilters");


            var FilterManager = new FilterManager();
            Dictionary<int, Filter> criteriaDefinitions = FilterManager.GetCriteriaDefinitions();
            _levelsByPriority = new List<StrategyLevelWithCriterias>();


            foreach (StrategyLevel i in strategy.StrategyLevels)
            {
                StrategyLevelWithCriterias strategyLevelWithCriterias = new StrategyLevelWithCriterias();
                strategyLevelWithCriterias.SuspicionLevelId = i.SuspicionLevelId;


                strategyLevelWithCriterias.LevelCriteriasThresholdPercentage = new List<LevelCriteriaInfo>();

                foreach (var j in i.StrategyLevelCriterias)
                {
                    LevelCriteriaInfo levelCriteriasThresholdPercentage = new LevelCriteriaInfo();
                    levelCriteriasThresholdPercentage.CriteriaDefinitions = new Filter() { CompareOperator = criteriaDefinitions.Where(x => x.Value.FilterId == j.FilterId).FirstOrDefault().Value.CompareOperator, FilterId = j.FilterId, Description = criteriaDefinitions.Where(x => x.Value.FilterId == j.FilterId).FirstOrDefault().Value.Description };
                    levelCriteriasThresholdPercentage.Percentage = j.Percentage;
                    levelCriteriasThresholdPercentage.Threshold = strategy.StrategyFilters.Where(x => x.FilterId == j.FilterId).FirstOrDefault().Threshold;
                    strategyLevelWithCriterias.LevelCriteriasThresholdPercentage.Add(levelCriteriasThresholdPercentage);
                }

                _levelsByPriority.Add(strategyLevelWithCriterias);

            }
        }

        public bool IsNumberSuspicious(NumberProfile numberProfile, out StrategyExecutionItem strategyExecutionItem)
        {

            strategyExecutionItem = null;

            Dictionary<int, Decimal?> criteriaValues = new Dictionary<int, decimal?>();


            foreach (StrategyLevelWithCriterias strategyLevelWithCriterias in _levelsByPriority)
            {
                bool isLevelMatch = false;
                Dictionary<int, decimal?> criteriaValuesThresholds = new Dictionary<int, decimal?>();

                foreach (LevelCriteriaInfo levelCriteriaThresholdPercentage in strategyLevelWithCriterias.LevelCriteriasThresholdPercentage)
                {
                    Decimal? criteriaValue;
                    if (!criteriaValues.TryGetValue(levelCriteriaThresholdPercentage.CriteriaDefinitions.FilterId, out criteriaValue))
                    {
                        criteriaValue = _FilterManager.GetCriteriaValue(levelCriteriaThresholdPercentage.CriteriaDefinitions, numberProfile);
                        criteriaValues.Add(levelCriteriaThresholdPercentage.CriteriaDefinitions.FilterId, criteriaValue);
                    }


                    decimal? criteriaValuesThreshold = criteriaValue / levelCriteriaThresholdPercentage.Threshold;
                    criteriaValuesThresholds.Add(levelCriteriaThresholdPercentage.CriteriaDefinitions.FilterId, criteriaValuesThreshold);

                    if (criteriaValuesThreshold != null)
                        isLevelMatch =
                            (levelCriteriaThresholdPercentage.CriteriaDefinitions.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual && criteriaValuesThreshold >= levelCriteriaThresholdPercentage.Percentage)
                            ||
                            (levelCriteriaThresholdPercentage.CriteriaDefinitions.CompareOperator == CriteriaCompareOperator.LessThanorEqual && criteriaValuesThreshold <= levelCriteriaThresholdPercentage.Percentage);
                    if (!isLevelMatch)
                        break;
                }


                if (isLevelMatch)
                {
                    strategyExecutionItem = new StrategyExecutionItem();
                    strategyExecutionItem.AccountNumber = numberProfile.AccountNumber;
                    strategyExecutionItem.SuspicionLevelID = strategyLevelWithCriterias.SuspicionLevelId;
                    strategyExecutionItem.FilterValues = criteriaValuesThresholds;
                    strategyExecutionItem.AggregateValues = numberProfile.AggregateValues;
                    strategyExecutionItem.SuspicionOccuranceStatus = SuspicionOccuranceStatus.Open;
                    strategyExecutionItem.StrategyExecutionID = numberProfile.StrategyExecutionID;
                    strategyExecutionItem.IMEIs = numberProfile.IMEIs;
                    return true;
                }
            }
            return false;
        }


        #region Private Classes

        private class StrategyLevelWithCriterias
        {
            public int SuspicionLevelId { get; set; }

            public List<LevelCriteriaInfo> LevelCriteriasThresholdPercentage { get; set; }
        }

        private class LevelCriteriaInfo
        {
            public Filter CriteriaDefinitions { get; set; }

            public Decimal Threshold { get; set; }

            public Decimal Percentage { get; set; }
        }

        #endregion
    }
}
