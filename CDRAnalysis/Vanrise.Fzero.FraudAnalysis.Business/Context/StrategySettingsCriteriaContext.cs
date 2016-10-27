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
        public Decimal? GetCriteriaValue(Filter filter)
        {
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
    }
}
