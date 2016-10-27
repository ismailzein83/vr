using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FraudManager
    {
        private Object _PreparedData;
        FilterManager _FilterManager = new FilterManager();
        public Strategy strategy { get; set; }

        public FraudManager(Strategy strategy)
        {
            this.strategy = strategy;
            PrepareStrategySettingsCriteriaContext context = new PrepareStrategySettingsCriteriaContext();
            strategy.Settings.StrategySettingsCriteria.PrepareForExecution(context);
            _PreparedData = context.PreparedData;
        }
        public bool IsNumberSuspicious(NumberProfile numberProfile, out StrategyExecutionItem strategyExecutionItem)
        {
            StrategySettingsCriteriaContext context = new StrategySettingsCriteriaContext
            {
                NumberProfile = numberProfile,
                PreparedData = _PreparedData,
            };
            bool result =   strategy.Settings.StrategySettingsCriteria.IsNumberSuspicious(context);
            strategyExecutionItem = context.StrategyExecutionItem;
            return result;
        }
    }
}
