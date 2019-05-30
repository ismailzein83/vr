using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Activities
{
    public class FinalizeDBReplicationInput
    {
        public IDBReplicationDataManager DBReplicationDataManager { get; set; }
    }

    public sealed class FinalizeDBReplication : BaseAsyncActivity<FinalizeDBReplicationInput>
    {
        [RequiredArgument]
        public InArgument<IDBReplicationDataManager> DBReplicationDataManager { get; set; }

        protected override void DoWork(FinalizeDBReplicationInput inputArgument, AsyncActivityHandle handle)
        {
            IDBReplicationDataManager dbReplicationDataManager = inputArgument.DBReplicationDataManager;
            dbReplicationDataManager.Finalize(new DBReplicationFinalizeContext()
            {
                WriteInformation = (message) =>
                {
                    handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
                },
                ShouldStop = () => { return ShouldStop(handle); }
            });
        }

        protected override FinalizeDBReplicationInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new FinalizeDBReplicationInput()
            {
                DBReplicationDataManager = this.DBReplicationDataManager.Get(context)
            };
        }
    }
}