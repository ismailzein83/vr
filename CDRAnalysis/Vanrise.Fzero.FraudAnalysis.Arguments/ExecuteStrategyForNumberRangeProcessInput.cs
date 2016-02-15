using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class ExecuteStrategyForNumberRangeProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public List<ExecuteStrategyExecutionItem> ExecuteStrategiesExecutionItems { get; set; }

        public List<string> NumberPrefixes { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool OverridePrevious { get; set; }

        public bool IncludeWhiteList { get; set; }

        public override string GetTitle()
        {
            IStrategyManager strategyManager = FraudManagerFactory.GetManager<IStrategyManager>();
            return String.Format("Execute Strategy Process For Number Prefixes '{0}', Time Range ({1:dd-MMM-yy HH:00} to {2:dd-MMM-yy HH:00}), Strategies: {3}", string.Join(",", this.NumberPrefixes), this.FromDate, this.ToDate, String.Join(",", strategyManager.GetStrategyNames(ExecuteStrategiesExecutionItems.Select(itm => itm.StrategyId).ToList())));
        }
    }

    public class ExecuteStrategyExecutionItem
    {
        public int StrategyId { get; set; }

        public long StrategyExecutionId { get; set; }

    }
}
