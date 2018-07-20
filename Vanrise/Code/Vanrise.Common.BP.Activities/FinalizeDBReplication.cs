using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Activities
{
    public sealed class FinalizeDBReplication : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<IDBReplicationDataManager> DBReplicationDataManager { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            IDBReplicationDataManager dbReplicationDataManager = this.DBReplicationDataManager.Get(context.ActivityContext);
            dbReplicationDataManager.Finalize(new DBReplicationFinalizeContext()
            {
                WriteInformation = (message) =>
                    {
                        context.ActivityContext.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
                    }
            });
        }
    }
}