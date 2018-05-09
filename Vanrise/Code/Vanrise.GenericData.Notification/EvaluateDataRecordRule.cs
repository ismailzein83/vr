using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Notification
{
    #region Arguments

    public class EvaluateDataRecordRuleInput
    {
        public Guid AlertRuleTypeId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public BaseQueue<RecordBatch> InputQueue { get; set; }
    }

    #endregion
    public sealed class EvaluateDataRecordRule : DependentAsyncActivity<EvaluateDataRecordRuleInput>
    {
        [RequiredArgument]
        public InArgument<Guid> AlertRuleTypeId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToTime { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RecordBatch>> InputQueue { get; set; }


        protected override void DoWork(EvaluateDataRecordRuleInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
          
            var counter = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (recordBatch) =>
                        {
                            handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "Evaluating Record {0}", counter);
                            DataRecordRuleEvaluationManager dataRecordRuleEvaluationManager = new DataRecordRuleEvaluationManager();
                            dataRecordRuleEvaluationManager.EvaluateDataRecordRule(recordBatch.Records, inputArgument.AlertRuleTypeId);
                            counter++;

                        });
                } while (!ShouldStop(handle) && hasItems);
            });
          //  handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0}  Processed", counter);
        }

        protected override EvaluateDataRecordRuleInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new EvaluateDataRecordRuleInput()
            {
                AlertRuleTypeId = this.AlertRuleTypeId.Get(context),
                FromTime = this.FromTime.Get(context),
                InputQueue = this.InputQueue.Get(context),
                ToTime = this.ToTime.Get(context),
            };
        }
    }
}
