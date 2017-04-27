using System;
using System.Activities;
using Vanrise.BusinessProcess;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class ComputeBatchesTimeSpan : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        [RequiredArgument]
        public InArgument<int> StorageRowCount { get; set; }

        //[RequiredArgument]
        public InArgument<int> RecordCountPerTransaction { get; set; }

        [RequiredArgument]
        public OutArgument<TimeSpan> BatchesTimeSpan { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            DateTime from = this.From.Get(context.ActivityContext);
            DateTime to = this.To.Get(context.ActivityContext);
            double storageRowCount = this.StorageRowCount.Get(context.ActivityContext);
            double maxBatchSize = this.RecordCountPerTransaction.Get(context.ActivityContext);  //10000; TODO: load it from settings

            int batchCount = (int)Math.Ceiling(storageRowCount / maxBatchSize);
            TimeSpan reprocessDuration = to.Subtract(from);
            TimeSpan batchDuration = TimeSpan.FromMinutes(reprocessDuration.TotalMinutes / batchCount);

            //batchDuration = new TimeSpan(batchDuration.Ticks - (batchDuration.Ticks % 10000000));//To Remove Milliseconds and Ticks
            batchDuration = new TimeSpan(batchDuration.Days, batchDuration.Hours, batchDuration.Minutes, batchDuration.Seconds);//To Remove Milliseconds and Ticks

            this.BatchesTimeSpan.Set(context.ActivityContext, batchDuration);
        }
    }
}