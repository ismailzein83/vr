using System;
using System.Activities;
using TestCallAnalysis.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments
    public class UpdateMappedCDRsInput
    {
        public MemoryQueue<Entities.UpdatedMappedCDRs> InputQueue { get; set; }
    }

    public class UpdateMappedCDRsOutput
    {
    }
    #endregion

    public class UpdateMappedCDRs : DependentAsyncActivity<UpdateMappedCDRsInput, UpdateMappedCDRsOutput>
    {
        [RequiredArgument]
        public InArgument<MemoryQueue<Entities.UpdatedMappedCDRs>> InputQueueIds { get; set; }

        protected override UpdateMappedCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateMappedCDRsInput
            {
                InputQueue = this.InputQueueIds.Get(context),
            };
        }

        protected override UpdateMappedCDRsOutput DoWorkWithResult(UpdateMappedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            MappedCDRManager mappedCDRManager = new MappedCDRManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItems = inputArgument.InputQueue.TryDequeue((updatedMappedCDRs) =>
                        {
                            DateTime batchStartTime = DateTime.Now;
                            if (updatedMappedCDRs.UpdatedIds != null && updatedMappedCDRs.UpdatedIds.Count > 0)
                            {
                                mappedCDRManager.UpdateMappedCDRs(updatedMappedCDRs);
                                double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'IsCorrolated' field in MappedCDRs Table is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                    updatedMappedCDRs.UpdatedIds.Count, elapsedTime.ToString());
                            }
                        });
                    };
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'IsCorrolated' field in MappedCDRs Table is done.");
            return new UpdateMappedCDRsOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, UpdateMappedCDRsOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
