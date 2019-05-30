using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Activities
{
    public class MigrateDataInput
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public int NumberOfDaysPerInterval { get; set; }

        public IDBReplicationDataManager DBReplicationDataManager { get; set; }
    }

    public sealed class MigrateData : BaseAsyncActivity<MigrateDataInput>
    {
        [RequiredArgument]
        public InArgument<DateTime> FromTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToTime { get; set; }

        [RequiredArgument]
        public InArgument<int> NumberOfDaysPerInterval { get; set; }

        [RequiredArgument]
        public InArgument<IDBReplicationDataManager> DBReplicationDataManager { get; set; }

        protected override void DoWork(MigrateDataInput inputArgument, AsyncActivityHandle handle)
        {
            IDBReplicationDataManager dbReplicationDataManager = inputArgument.DBReplicationDataManager;
            DateTime fromTime = inputArgument.FromTime;
            DateTime toTime = inputArgument.ToTime;
            int numberOfDaysPerInterval = inputArgument.NumberOfDaysPerInterval;

            dbReplicationDataManager.MigrateData(new DBReplicationMigrateDataContext()
            {
                FromTime = fromTime,
                ToTime = toTime,
                NumberOfDaysPerInterval = numberOfDaysPerInterval,
                WriteInformation = (message) =>
                {
                    handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
                },
                ShouldStop = () => { return ShouldStop(handle); }
            });
        }

        protected override MigrateDataInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new MigrateDataInput
            {
                DBReplicationDataManager = this.DBReplicationDataManager.Get(context),
                FromTime = this.FromTime.Get(context),
                NumberOfDaysPerInterval = this.NumberOfDaysPerInterval.Get(context),
                ToTime = this.ToTime.Get(context)
            };
        }
    }
}