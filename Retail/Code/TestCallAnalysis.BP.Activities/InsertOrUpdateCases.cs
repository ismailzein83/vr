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
    public class InsertCaseCDRsInput
    {
        public MemoryQueue<PrepareCDRCasesToInsert> InputQueue { get; set; }
    }

    public class InsertCaseCDRsOutput
    {
    }
    #endregion

    public class InsertOrUpdateCases : DependentAsyncActivity<InsertCaseCDRsInput, InsertCaseCDRsOutput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<PrepareCDRCasesToInsert>> InputQueueToInsert { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.InputQueueToInsert.Get(context) == null)
                this.InputQueueToInsert.Set(context, new MemoryQueue<PrepareCDRCasesToInsert>());
            base.OnBeforeExecute(context, handle);
        }

        protected override InsertCaseCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new InsertCaseCDRsInput()
            {
                InputQueue = this.InputQueueToInsert.Get(context),
            };
        }

        protected override InsertCaseCDRsOutput DoWorkWithResult(InsertCaseCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            CaseCDRManager caseCDRManager = new CaseCDRManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItem = inputArgument.InputQueue.TryDequeue((caseCDRsList) =>
                        {
                            DateTime batchStartTime = DateTime.Now;

                            List<string> existingCallingNumbers = new List<string>();
                            existingCallingNumbers = caseCDRManager.GetCasesCDRCallingNumbersList();
                            
                            if (existingCallingNumbers != null && existingCallingNumbers.Count() > 0)
                            {
                                // dividing cases between insert and update 
                                List<dynamic> caseCDRsToInsert = new List<dynamic>();
                                PrepareCDRCasesToInsert cdrCaseBatch = new PrepareCDRCasesToInsert();
                                foreach (var caseCDR in caseCDRsList.TCAnalListToInsert)
                                {
                                    var indexCaseCDR = existingCallingNumbers.IndexOf(caseCDR.CallingNumber);

                                    if (indexCaseCDR != -1)
                                    {
                                        caseCDR.NumberOfCDRs++;
                                        cdrCaseBatch.TCAnalListToInsert.Add(caseCDR);
                                    }

                                    else
                                        caseCDRsToInsert.Add(caseCDR);
                                }

                                // Update case CDRs
                                if (cdrCaseBatch != null && cdrCaseBatch.TCAnalListToInsert.Count > 0)
                                {
                                    caseCDRManager.UpdateCases(cdrCaseBatch.TCAnalListToInsert);
                                    double elapsedTimeToUpdate = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                        cdrCaseBatch.TCAnalListToInsert.Count, elapsedTimeToUpdate.ToString());
                                }

                                // Insert Case CDRs
                                if (caseCDRsToInsert != null && caseCDRsToInsert.Count > 0)
                                {
                                    caseCDRManager.InsertCases(caseCDRsToInsert);
                                    double elapsedTimeToInsert = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                        caseCDRsToInsert.Count, elapsedTimeToInsert.ToString());
                                }
                            }
                            else
                            {
                                // Insert Case CDRs if no existing cases
                                caseCDRManager.InsertCases(caseCDRsList.TCAnalListToInsert);
                                double elapsedTimeToInsert = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                    caseCDRsList.TCAnalListToInsert.Count, elapsedTimeToInsert.ToString());
                            }
                        });
                    }
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert case CDRs is done.");
            return new InsertCaseCDRsOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InsertCaseCDRsOutput result)
        {
        }
    }
}
