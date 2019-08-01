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

    public class CDRCasesInsertedOrUpdatedOutput
    {
        public Dictionary<long,string> CasesInsertedOrUpdated { get; set; }
    }
    public class InsertOrUpdateCaseCDRsInput
    {
        public MemoryQueue<PrepareCDRCasesOutput> InputQueue { get; set; }
        public MemoryQueue<CDRCasesInsertedOrUpdatedOutput> OutputQueue { get; set; }

    }
    #endregion

    public class InsertOrUpdateCases : DependentAsyncActivity<InsertOrUpdateCaseCDRsInput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<PrepareCDRCasesOutput>> InputQueueToInsertOrUpdate { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<CDRCasesInsertedOrUpdatedOutput>> OutputQueueToInsertOrUpdate { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.InputQueueToInsertOrUpdate.Get(context) == null)
                this.InputQueueToInsertOrUpdate.Set(context, new MemoryQueue<PrepareCDRCasesOutput>());
            if (this.OutputQueueToInsertOrUpdate.Get(context) == null)
                this.OutputQueueToInsertOrUpdate.Set(context, new MemoryQueue<CDRCasesInsertedOrUpdatedOutput>());
            base.OnBeforeExecute(context, handle);
        }

        protected override InsertOrUpdateCaseCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new InsertOrUpdateCaseCDRsInput()
            {
                InputQueue = this.InputQueueToInsertOrUpdate.Get(context),
                OutputQueue = this.OutputQueueToInsertOrUpdate.Get(context),
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

                            if(casesToInsertOrUpdate != null)
                            {
                                if(casesToInsertOrUpdate.CasesToInsert != null)
                                {
                                    CDRCasesInsertedOrUpdatedOutput cdrCasesInsertedOrUpdatedOutput = new CDRCasesInsertedOrUpdatedOutput
                                    {
                                        CasesInsertedOrUpdated = new Dictionary<long, string>()
                                    };
                                    foreach (var caseToInsert in casesToInsertOrUpdate.CasesToInsert)
                                    {
                                        cdrCasesInsertedOrUpdatedOutput.CasesInsertedOrUpdated.Add(caseToInsert.CaseId, caseToInsert.CallingNumber);
                                    }
                                    foreach (var caseToUpdate in casesToInsertOrUpdate.CasesToUpdate)
                                    {
                                        cdrCasesInsertedOrUpdatedOutput.CasesInsertedOrUpdated.Add(caseToUpdate.CaseId, caseToUpdate.CallingNumber);

                                    }
                                    inputArgument.OutputQueue.Enqueue(cdrCasesInsertedOrUpdatedOutput);
                                }
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
