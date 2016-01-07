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
        public DWStrategyDictionary DWStrategies { get; set; }
    }

    #endregion

    public sealed class GetDWStrategies : BaseAsyncActivity<GetDWStrategiesInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<DWStrategyDictionary> DWStrategies { get; set; }

        #endregion

        protected override void DoWork(GetDWStrategiesInput inputArgument, AsyncActivityHandle handle)
        {
            DWStrategyManager dwStrategyManager = new DWStrategyManager();
            IEnumerable<DWStrategy> listDWStrategies = dwStrategyManager.GetStrategies();
            inputArgument.DWStrategies = new DWStrategyDictionary();
            if (listDWStrategies.Count() > 0)
                foreach (var i in listDWStrategies)
                    inputArgument.DWStrategies.Add(i.Id, i);
        }


        protected override GetDWStrategiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWStrategiesInput
            {
                DWStrategies = this.DWStrategies.Get(context),
            };
        }
    }
}
