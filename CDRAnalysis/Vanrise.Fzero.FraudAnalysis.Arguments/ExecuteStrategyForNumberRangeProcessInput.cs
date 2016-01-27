﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class ExecuteStrategyForNumberRangeProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public List<ExecuteStrategyExecutionItem> StrategyExecutionItems { get; set; }

        public List<string> NumberPrefixes { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool OverridePrevious { get; set; }

        public bool IncludeWhiteList { get; set; }

        public override string GetTitle()
        {
            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            return String.Format("Execute Strategy Process For Number Prefixes '{0}', Time Range ({1:dd-MMM-yy HH:mm} to {2:dd-MMM-yy HH:mm}), Strategies: {3}",  string.Join(",", this.NumberPrefixes), this.FromDate, this.ToDate, String.Join(",", dataManager.GetStrategyNames(StrategyExecutionItems.Select(itm => itm.StrategyId).ToList())));
        }
    }

    public class ExecuteStrategyExecutionItem
    {
        public int StrategyId { get; set; }

        public long StrategyExecutionId { get; set; }
    }
}
