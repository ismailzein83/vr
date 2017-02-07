using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.MainExtensions
{
    public class SuspicionRuleStrategySettingsCriteria : StrategySettingsCriteria
    {
        public override Guid ConfigId { get { return new Guid("367615e0-1a10-42ce-9e50-2a572463ee6f"); } }
        public List<StrategyLevel> StrategyLevels { get; set; }
        public List<StrategyFilter> StrategyFilters { get; set; }
        public override void PrepareForExecution(IPrepareStrategySettingsCriteriaContext context)
        {
            if (this.StrategyLevels == null)
                throw new ArgumentNullException("StrategyLevels");
            if (this.StrategyFilters == null)
                throw new ArgumentNullException("StrategyFilters");
            var FilterManager = new FilterManager();
            Dictionary<int, Filter> criteriaDefinitions = FilterManager.GetCriteriaDefinitions();
            List<StrategyLevelWithCriterias> _levelsByPriority = new List<StrategyLevelWithCriterias>();
            foreach (StrategyLevel i in this.StrategyLevels)
            {
                StrategyLevelWithCriterias strategyLevelWithCriterias = new StrategyLevelWithCriterias();
                strategyLevelWithCriterias.SuspicionLevelId = i.SuspicionLevelId;


                strategyLevelWithCriterias.LevelCriteriasThresholdPercentage = new List<LevelCriteriaInfo>();

                foreach (var j in i.StrategyLevelCriterias)
                {
                    LevelCriteriaInfo levelCriteriasThresholdPercentage = new LevelCriteriaInfo();
                    levelCriteriasThresholdPercentage.FilterId = j.FilterId;
                    levelCriteriasThresholdPercentage.Percentage = j.Percentage;
                    levelCriteriasThresholdPercentage.Threshold = this.StrategyFilters.Where(x => x.FilterId == j.FilterId).FirstOrDefault().Threshold;
                    strategyLevelWithCriterias.LevelCriteriasThresholdPercentage.Add(levelCriteriasThresholdPercentage);
                }

                _levelsByPriority.Add(strategyLevelWithCriterias);

            }
            context.PreparedData = _levelsByPriority;
        }
        public override bool IsNumberSuspicious(IStrategySettingsCriteriaContext context)
        {

            List<StrategyLevelWithCriterias> _levelsByPriority = context.PreparedData as List<StrategyLevelWithCriterias>;
            if (_levelsByPriority != null)
            {
                foreach (StrategyLevelWithCriterias strategyLevelWithCriterias in _levelsByPriority)
                {
                    bool isLevelMatch = false;
                    Dictionary<int, decimal?> criteriaValuesThresholds = new Dictionary<int, decimal?>();

                    foreach (LevelCriteriaInfo levelCriteriaThresholdPercentage in strategyLevelWithCriterias.LevelCriteriasThresholdPercentage)
                    {
                        Decimal? criteriaValue = context.GetCriteriaValue(levelCriteriaThresholdPercentage.FilterId);

                        decimal? criteriaValuesThreshold = criteriaValue / levelCriteriaThresholdPercentage.Threshold;
                        criteriaValuesThresholds.Add(levelCriteriaThresholdPercentage.FilterId, criteriaValuesThreshold);

                        var filter = context.GetFilter(levelCriteriaThresholdPercentage.FilterId);
                        if (criteriaValuesThreshold != null)
                            isLevelMatch =
                                (filter.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual && criteriaValuesThreshold >= levelCriteriaThresholdPercentage.Percentage)
                                ||
                                (filter.CompareOperator == CriteriaCompareOperator.LessThanorEqual && criteriaValuesThreshold <= levelCriteriaThresholdPercentage.Percentage);
                        if (!isLevelMatch)
                            break;
                    }
                    if (isLevelMatch)
                    {
                        context.StrategyExecutionItem = new StrategyExecutionItem
                        {
                            AccountNumber = context.NumberProfile.AccountNumber,
                            SuspicionLevelID = strategyLevelWithCriterias.SuspicionLevelId,
                            FilterValues = criteriaValuesThresholds,
                            AggregateValues = context.NumberProfile.AggregateValues,
                            SuspicionOccuranceStatus = SuspicionOccuranceStatus.Open,
                            StrategyExecutionID = context.NumberProfile.StrategyExecutionID,
                            IMEIs = context.NumberProfile.IMEIs,
                        };

                        return true;
                    }
                }
            }
            return false;
        }

    }
    public class StrategyLevelWithCriterias
    {
        public int SuspicionLevelId { get; set; }

        public List<LevelCriteriaInfo> LevelCriteriasThresholdPercentage { get; set; }
    }
    public class LevelCriteriaInfo
    {
        public int FilterId { get; set; }

        public Decimal Threshold { get; set; }

        public Decimal Percentage { get; set; }
    }
    public class StrategyFilter
    {
        public int FilterId { get; set; }
        public string Description { get; set; }
        public Decimal Threshold { get; set; }
    }
    public class StrategyLevel
    {
        public int SuspicionLevelId { get; set; }
        public List<StrategyLevelCriteria> StrategyLevelCriterias { get; set; }
    }
    public class StrategyLevelCriteria
    {
        public int FilterId { get; set; }
        public decimal Percentage { get; set; }
    }
   

}
