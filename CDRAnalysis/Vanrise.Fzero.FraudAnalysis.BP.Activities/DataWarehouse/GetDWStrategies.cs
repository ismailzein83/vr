using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetDWStrategiesInput
    {

    }

    public class GetDWStrategiesOutput
    {
        public DWStrategyDictionary DWStrategies { get; set; }
    }

    #endregion

    public sealed class GetDWStrategies : BaseAsyncActivity<GetDWStrategiesInput, GetDWStrategiesOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<DWStrategyDictionary> DWStrategies { get; set; }

        #endregion


        protected override GetDWStrategiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWStrategiesInput
            {
                
            };
        }

        protected override GetDWStrategiesOutput DoWorkWithResult(GetDWStrategiesInput inputArgument, AsyncActivityHandle handle)
        {
            DWStrategyManager dwStrategyManager = new DWStrategyManager();
            IEnumerable<DWStrategy> listDWStrategies = dwStrategyManager.GetStrategies();
            DWStrategyDictionary DWStrategies = new DWStrategyDictionary();
            if (listDWStrategies.Count() > 0)
                foreach (var i in listDWStrategies)
                    DWStrategies.Add(i.Id, i);

            return new GetDWStrategiesOutput{
                DWStrategies = DWStrategies
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDWStrategiesOutput result)
        {
            this.DWStrategies.Set(context, result.DWStrategies);
        }
    }
}
