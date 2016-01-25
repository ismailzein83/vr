using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Business;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public sealed class OverridePreviousStrategyExecution : CodeActivity
    { 
        #region Arguments

        [RequiredArgument]
        public InArgument<List<int>> StrategyIds { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            Console.WriteLine("Started overriding previous results");
            StrategyExecutionManager strategyExecutionManager = new StrategyExecutionManager();
            strategyExecutionManager.OverrideStrategyExecution(context.GetValue(StrategyIds), context.GetValue(FromDate), context.GetValue(ToDate));

            Console.WriteLine("Ended overriding previous results");
        }
    }
}
