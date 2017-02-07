using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategySettingsCriteriaContext : IStrategySettingsCriteriaContext
    {
        public object PreparedData { get; set; }
        public NumberProfile NumberProfile { get; set; }
        public Dictionary<int, decimal?> CriteriaValues { get; set; }
        public StrategyExecutionItem StrategyExecutionItem { get; set; }
        private Dictionary<int, Filter> CriteriaDefinitions { get; set; }

        public Decimal? GetCriteriaValue(int filterID)
        {
            var filter = GetFilter(filterID);
            Decimal? criteriaValue;
            if (this.CriteriaValues == null)
                this.CriteriaValues = new Dictionary<int, decimal?>();
            if (!this.CriteriaValues.TryGetValue(filter.FilterId, out criteriaValue))
            {
                FilterManager _FilterManager = new FilterManager();
                criteriaValue = _FilterManager.GetCriteriaValue(filter, this.NumberProfile);
                this.CriteriaValues.Add(filter.FilterId, criteriaValue);
            }
            return criteriaValue;
        }
        public bool IsMatch(int filterId, decimal threshold, decimal percentage)
        {
            var filter = GetFilter(filterId);
            if ((filter.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual && threshold >= percentage) ||  (filter.CompareOperator == CriteriaCompareOperator.LessThanorEqual && threshold <= percentage))
            return true;
          
            return false;
        }
        public Filter GetFilter(int filterId)
        {
            if (this.CriteriaDefinitions == null)
            {
                var FilterManager = new FilterManager();
                this.CriteriaDefinitions = FilterManager.GetCriteriaDefinitions();
            }

            return this.CriteriaDefinitions.Where(x => x.Value.FilterId == filterId).FirstOrDefault().Value;
        }
    }
}
