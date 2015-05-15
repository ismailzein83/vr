﻿using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    
    public class GetStrategy :  CodeActivity 
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<int> StrategyId { get; set; }

        public OutArgument<Strategy> Strategy { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            context.SetValue(Strategy, new StrategyManager().GetDefaultStrategy());
        }
    }
}
