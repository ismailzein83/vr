using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyDWBTSsToDBInput
    {
        public List<DWDimension> ToBeInsertedCallBTSs { get; set; }
    }

    public sealed class ApplyDWBTSsToDB : DependentAsyncActivity<ApplyDWBTSsToDBInput>
    {
        [RequiredArgument]
        public InArgument<List<DWDimension>> ToBeInsertedCallBTSs { get; set; }


        protected override void DoWork(ApplyDWBTSsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDWDimensionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWDimensionDataManager>();

            dataManager.TableName = "[dbo].[Dim_BTS]";
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedCallBTSs);
            if (inputArgument.ToBeInsertedCallBTSs.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "added {0} BTS(s) ", inputArgument.ToBeInsertedCallBTSs.Count);
        }

        protected override ApplyDWBTSsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyDWBTSsToDBInput()
            {
                ToBeInsertedCallBTSs = this.ToBeInsertedCallBTSs.Get(context)
            };
        }
    }
}
