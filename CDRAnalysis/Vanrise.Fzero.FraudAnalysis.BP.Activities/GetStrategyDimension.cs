using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetStrategyDimensionInput
    {
        public Dictionary<int, Strategy> Strategies { get; set; }
    }

    #endregion

    public sealed class GetStrategyDimension : BaseAsyncActivity<GetStrategyDimensionInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Strategy>> Strategies { get; set; }

        #endregion

        protected override void DoWork(GetStrategyDimensionInput inputArgument, AsyncActivityHandle handle)
        {
            StrategyManager strategyManager = new StrategyManager();
            IEnumerable<Strategy> listDWStrategies = strategyManager.GetAll();
            inputArgument.Strategies = listDWStrategies.ToDictionary(dim => dim.Id, dim => dim);
        }


        protected override GetStrategyDimensionInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetStrategyDimensionInput
            {
                Strategies = this.Strategies.Get(context),
            };
        }
    }
}
