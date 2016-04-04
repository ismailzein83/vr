using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyDWStrategiesToDBInput
    {
        public List<DWStrategy> ToBeInsertedStrategies { get; set; }
    }

    public sealed class ApplyDWStrategiesToDB : DependentAsyncActivity<ApplyDWStrategiesToDBInput>
    {
        [RequiredArgument]
        public InOutArgument<List<DWStrategy>> ToBeInsertedStrategies { get; set; }

        protected override void DoWork(ApplyDWStrategiesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started adding {0} strategies ", inputArgument.ToBeInsertedStrategies.Count);
            IDWStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWStrategyDataManager>();
            dataManager.SaveDWStrategiesToDB(inputArgument.ToBeInsertedStrategies);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} strategies ", inputArgument.ToBeInsertedStrategies.Count);
        }

        protected override ApplyDWStrategiesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyDWStrategiesToDBInput()
            {
                ToBeInsertedStrategies = this.ToBeInsertedStrategies.Get(context)
            };
        }
    }
}
