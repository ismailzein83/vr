using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TestCallAnalysis.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments
    public class InsertOrUpdateCaseCDRsInput
    {
        public MemoryQueue<PrepareCDRCasesOutput> InputQueue { get; set; }
    }
    #endregion

    public class InsertOrUpdateCases : DependentAsyncActivity<InsertOrUpdateCaseCDRsInput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<PrepareCDRCasesOutput>> InputQueueToInsertOrUpdate { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.InputQueueToInsertOrUpdate.Get(context) == null)
                this.InputQueueToInsertOrUpdate.Set(context, new MemoryQueue<PrepareCDRCasesOutput>());
            base.OnBeforeExecute(context, handle);
        }

        protected override InsertOrUpdateCaseCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new InsertOrUpdateCaseCDRsInput()
            {
                InputQueue = this.InputQueueToInsertOrUpdate.Get(context),
            };
        }

        protected override void DoWork(InsertOrUpdateCaseCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            CaseCDRManager caseCDRManager = new CaseCDRManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItem = inputArgument.InputQueue.TryDequeue((casesToInsertOrUpdate) =>
                        {
                            DateTime batchStartTime = DateTime.Now;

                            if (casesToInsertOrUpdate != null && casesToInsertOrUpdate.CasesToInsert != null && casesToInsertOrUpdate.CasesToInsert.Count() > 0)
                            {
                                caseCDRManager.InsertCases(casesToInsertOrUpdate.CasesToInsert);
                                double elapsedTimeToInsert = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                    casesToInsertOrUpdate.CasesToInsert.Count, elapsedTimeToInsert.ToString());
                            }

                            if (casesToInsertOrUpdate != null && casesToInsertOrUpdate.CasesToUpdate != null && casesToInsertOrUpdate.CasesToUpdate.Count() > 0)
                            {
                                caseCDRManager.UpdateCases(casesToInsertOrUpdate.CasesToUpdate);
                                double elapsedTimeToUpdate = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                    casesToInsertOrUpdate.CasesToUpdate.Count, elapsedTimeToUpdate.ToString());
                            }
                        });
                    }
                    else
                    {
                        hasItem = false;
                    }
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert/Update case CDRs is done.");
        }
    }
}
