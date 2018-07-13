using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Activities
{
    public sealed class MigrateData : BaseCodeActivity
    {
        //[RequiredArgument]
        //public InArgument<DBReplicationSettings> DBReplicationSettings { get; set; }

        //[RequiredArgument]
        //public InArgument<DBReplicationDefinition> DBReplicatioDefinition { get; set; }

        //[RequiredArgument]
        //public InArgument<IDBReplicationDataManager> DBReplicationDataManager { get; set; }
        
        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            //DBReplicationSettings dbReplicationSettings = this.DBReplicationSettings.Get(context.ActivityContext);
            //DBReplicationDefinition dbReplicatioDefinition = this.DBReplicatioDefinition.Get(context.ActivityContext);
            //IDBReplicationDataManager dbReplicationDataManager = this.DBReplicationDataManager.Get(context.ActivityContext);

            //dbReplicationDataManager.MigrateData(new DBReplicationMigrateDataContext() { });

        }
    }
}