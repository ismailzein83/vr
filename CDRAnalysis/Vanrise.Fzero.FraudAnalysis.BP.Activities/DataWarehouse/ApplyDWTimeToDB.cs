using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyDWTimesToDBInput
    {
        public List<DWTime> ToBeInsertedTimes { get; set; }
    }

    public sealed class ApplyDWTimesToDB : DependentAsyncActivity<ApplyDWTimesToDBInput>
    {
        [RequiredArgument]
        public InArgument<List<DWTime>> ToBeInsertedTimes { get; set; }

        protected override void DoWork(ApplyDWTimesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDWTimeDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWTimeDataManager>();

            dataManager.SaveDWTimesToDB(inputArgument.ToBeInsertedTimes);
        }

        protected override ApplyDWTimesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyDWTimesToDBInput()
            {
                ToBeInsertedTimes = this.ToBeInsertedTimes.Get(context)
            };
        }
    }
}
