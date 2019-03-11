using System;
using System.Activities;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments
    public class UpdateCorrelatedCDRsInput
    {
        public MemoryQueue<CDRCorrelationBatch> InsertedCorrelatedCDRs { get; set; }
    }
    public class UpdateCorrelatedCDRsOutput
    {
    }
    #endregion

    public class UpdateCorrelatedCDRs : DependentAsyncActivity<UpdateCorrelatedCDRsInput, UpdateCorrelatedCDRsOutput>
    {
        public InOutArgument<MemoryQueue<Entities.UpdateCorrelatedCDRsBatch>> UpdateCorrelatedCDRsInput { get; set; } // this argument must be deleted
        [RequiredArgument]
        public InOutArgument<MemoryQueue<CDRCorrelationBatch>> InsertedCorrelatedCDRs { get; set; }

        protected override UpdateCorrelatedCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateCorrelatedCDRsInput
            {
                InsertedCorrelatedCDRs = this.InsertedCorrelatedCDRs.Get(context),
            };
        }

        protected override UpdateCorrelatedCDRsOutput DoWorkWithResult(UpdateCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            CaseCDRManager caseCDRManager = new CaseCDRManager();
            CorrelatedCDRManager correlatedCDRManager = new CorrelatedCDRManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    if (inputArgument.InsertedCorrelatedCDRs != null && inputArgument.InsertedCorrelatedCDRs.Count > 0)
                    {
                        DateTime batchStartTime = DateTime.Now;
                        var callingNumbersList = caseCDRManager.GetCasesCDRCallingNumbers();
                        if (callingNumbersList != null && callingNumbersList.Count > 0)
                        {
                            hasItems = inputArgument.InsertedCorrelatedCDRs.TryDequeue((listOfUpdatedCorrolatedCDRs) =>
                            {
                                if (listOfUpdatedCorrolatedCDRs.OutputRecordsToInsert != null && listOfUpdatedCorrolatedCDRs.OutputRecordsToInsert.Count > 0)
                                {
                                    var numberOfUpdatedCorrelation = correlatedCDRManager.UpdateCorrelatedCDRs(listOfUpdatedCorrolatedCDRs, callingNumbersList);
                                    double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'CaseId' field in CorrelatedCDRs Table is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                       numberOfUpdatedCorrelation, elapsedTime.ToString());
                                }
                            });
                        }
                    }
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'CaseId' field in CorrelatedCDRs Table is done.");
            return new UpdateCorrelatedCDRsOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, UpdateCorrelatedCDRsOutput result)
        {
        }
    }
}
