using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class DeleteStrategyExecutionDetails : CodeActivity
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        [RequiredArgument]
        public InArgument<List<StrategyExecutionInfo>> StrategiesExecutionInfo { get; set; }


        [RequiredArgument]
        public InArgument<bool> OverridePrevious { get; set; }

        #endregion



        protected override void Execute(CodeActivityContext context)
        {
            Console.WriteLine("Check if overiding previous results is allowed");
            if (context.GetValue(OverridePrevious))
            {
                Console.WriteLine("Started deleting previous results");
                StrategyManager manager = new StrategyManager();

                List<int> StrategyIds = new List<int>();
                foreach (var strategyExecutionIfo in context.GetValue(StrategiesExecutionInfo))
                {
                    StrategyIds.Add(strategyExecutionIfo.Strategy.Id);
                }

                foreach (var strategyId in StrategyIds)
                    manager.OverrideStrategyExecution(strategyId, context.GetValue(FromDate), context.GetValue(ToDate));


                Console.WriteLine("Ended deleting previous results");
            }
          
        }
    }
}

