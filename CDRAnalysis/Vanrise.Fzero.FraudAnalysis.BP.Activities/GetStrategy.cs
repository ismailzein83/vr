using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Business;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetStrategyInput
    {
        public int StrategyId { get; set; }

    }

    public class GetStrategyOutput
    {
        public Strategy Strategy { get; set; }

    }

    #endregion
    public class GetStrategy : BaseAsyncActivity<GetStrategyInput, GetStrategyOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<int> StrategyId { get; set; }

        public OutArgument<Strategy> Strategy { get; set; }

        #endregion


        protected override GetStrategyOutput DoWorkWithResult(GetStrategyInput inputArgument, AsyncActivityHandle handle)
        {
            return new GetStrategyOutput
            {
                Strategy =  new StrategyManager().GetStrategy(inputArgument.StrategyId)
            };
        }

        protected override GetStrategyInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetStrategyInput
            {
                StrategyId = this.StrategyId.Get(context)
            };
        }

        protected override void OnWorkComplete(System.Activities.AsyncCodeActivityContext context, GetStrategyOutput result)
        {
            this.Strategy.Set(context, result.Strategy);
        }
    }
}
