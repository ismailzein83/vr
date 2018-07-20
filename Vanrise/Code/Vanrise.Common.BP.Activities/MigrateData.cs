using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Activities
{
    public sealed class MigrateData : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> FromTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToTime { get; set; }

        [RequiredArgument]
        public InArgument<int> NumberOfDaysPerInterval { get; set; }

        [RequiredArgument]
        public InArgument<IDBReplicationDataManager> DBReplicationDataManager { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            IDBReplicationDataManager dbReplicationDataManager = this.DBReplicationDataManager.Get(context.ActivityContext);
            DateTime fromTime = this.FromTime.Get(context.ActivityContext);
            DateTime toTime = this.ToTime.Get(context.ActivityContext);
            int numberOfDaysPerInterval = this.NumberOfDaysPerInterval.Get(context.ActivityContext);

            dbReplicationDataManager.MigrateData(new DBReplicationMigrateDataContext()
            {
                FromTime = fromTime,
                ToTime = toTime,
                NumberOfDaysPerInterval = numberOfDaysPerInterval,
                WriteInformation = (message) =>
                {
                    context.ActivityContext.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
                }
            });
        }
    }
}