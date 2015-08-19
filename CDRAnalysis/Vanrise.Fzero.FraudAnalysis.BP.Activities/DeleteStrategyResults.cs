using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class DeleteStrategyResults : CodeActivity
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        [RequiredArgument]
        public InArgument<List<int>> StrategyIds { get; set; }


        [RequiredArgument]
        public InArgument<bool> OverridePrevious { get; set; }

        #endregion



        protected override void Execute(CodeActivityContext context)
        {
            if (context.GetValue(OverridePrevious))
            {
                StrategyManager manager = new StrategyManager();
                manager.DeleteStrategyResults(string.Join(",", context.GetValue(StrategyIds)), context.GetValue(FromDate), context.GetValue(ToDate));
            }
          
        }
    }
}

