using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments
    public class CorrelateCDRsInput
    {
        public MemoryQueue<RecordBatch> InputRecordsQueue { get; set; }
        public TimeSpan DateTimeMargin { get; set; }
        public TimeSpan TimeOutMargin { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> OutputCorrelationBatchQueue { get; set; }
        public MemoryQueue<Entities.CDRCaseBatch> OutputCaseQueue { get; set; }
        public MemoryQueue<Entities.UpdatedMappedCDRs> UpdatedMappedCDRsInput { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> UpdatedCDRCorrelationBatchInput { get; set; }
    }
    #endregion

    public sealed class CorrelateCDRs : DependentAsyncActivity<CorrelateCDRsInput>
    {
        [RequiredArgument]
        public InArgument<MemoryQueue<RecordBatch>> InputRecordsQueue { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }
       
        public InArgument<TimeSpan> TimeOutMargin { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> OutputCorrelationBatchQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCaseBatch>> OutputCaseQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.UpdatedMappedCDRs>> UpdatedMappedCDRsInput { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> UpdatedCDRCorrelationBatchInput { get; set; } 

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputCorrelationBatchQueue.Get(context) == null)
                this.OutputCorrelationBatchQueue.Set(context, new MemoryQueue<Entities.CDRCorrelationBatch>());
            if (this.OutputCaseQueue.Get(context) == null)
                this.OutputCaseQueue.Set(context, new MemoryQueue<Entities.CDRCaseBatch>());
            if (this.UpdatedMappedCDRsInput.Get(context) == null)
                this.UpdatedMappedCDRsInput.Set(context, new MemoryQueue<Entities.UpdatedMappedCDRs>());
            if (this.UpdatedCDRCorrelationBatchInput.Get(context) == null)
                this.UpdatedCDRCorrelationBatchInput.Set(context, new MemoryQueue<Entities.CDRCorrelationBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override CorrelateCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CorrelateCDRsInput()
            {
                InputRecordsQueue = this.InputRecordsQueue.Get(context),
                DateTimeMargin = this.DateTimeMargin.Get(context),
                TimeOutMargin = this.TimeOutMargin.Get(context),
                OutputCorrelationBatchQueue = this.OutputCorrelationBatchQueue.Get(context),
                OutputCaseQueue = this.OutputCaseQueue.Get(context),
                UpdatedMappedCDRsInput = this.UpdatedMappedCDRsInput.Get(context),
                UpdatedCDRCorrelationBatchInput = this.UpdatedCDRCorrelationBatchInput.Get(context),
            };
        }

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;
            inputArgument.TimeOutMargin = new TimeSpan(0, 15, 0);
            CalledNumberMappingManager calledNumberMappingManager = new CalledNumberMappingManager();
            MappedCDRManager mappedCDRManager = new MappedCDRManager();
            CorrelatedCDRManager correlatedCDRManager = new CorrelatedCDRManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    if (inputArgument.InputRecordsQueue != null && inputArgument.InputRecordsQueue.Count > 0)
                    {
                        hasItems = inputArgument.InputRecordsQueue.TryDequeue((recordBatch) =>
                        {
                            DateTime batchStartTime = DateTime.Now;

                            if (recordBatch.Records != null && recordBatch.Records.Count > 0)
                            {
                                Dictionary<long, List<TCAnalMappedCDR>> receivedCDRs = new Dictionary<long, List<TCAnalMappedCDR>>();
                                Dictionary<long, List<TCAnalMappedCDR>> generatedCDRs = new Dictionary<long, List<TCAnalMappedCDR>>();
                                Entities.UpdatedMappedCDRs updateMappedCDRs = new Entities.UpdatedMappedCDRs();

                                // Divide all CDRs between generated and received
                                foreach (var cdr in recordBatch.Records)
                                {
                                    TCAnalMappedCDR mappedCDR = mappedCDRManager.MappedCDRMapper(cdr);
                                    if (mappedCDR.CDRType.Equals(CDRType.Generated))
                                        generatedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);
                                    else
                                        receivedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);
                                    
                                    updateMappedCDRs.MappedCDRsToUpdate.Add(mappedCDR); // List of all mapped cdrs to update the "IsCorrelated" field
                                }

                                // Create the correlatedCDRs list
                                List<TCAnalCorrelatedCDR> correlatedCDRs = new List<TCAnalCorrelatedCDR>();
                                if (receivedCDRs != null && receivedCDRs.Count > 0 && generatedCDRs != null && generatedCDRs.Count > 0)
                                {
                                    foreach (var recievedCDR in receivedCDRs)
                                    {
                                        if (recievedCDR.Value != null && recievedCDR.Value.Count > 0)
                                        {
                                            foreach (var rcvdcdr in recievedCDR.Value)
                                            {
                                                TCAnalCorrelatedCDR correlatedCDR = new TCAnalCorrelatedCDR();
                                                var rvd = updateMappedCDRs.MappedCDRsToUpdate.Find(x => x.MappedCDRId == rcvdcdr.MappedCDRId);

                                                var generatedCDR = generatedCDRs.GetRecord(recievedCDR.Key);
                                                if (generatedCDR != null && generatedCDR.Count > 0)
                                                {
                                                    correlatedCDR = correlatedCDRManager.CorrelatedCDRMapper(rcvdcdr);
                                                    foreach (var gnrtdCDR in generatedCDR)
                                                    {
                                                        var gntd = updateMappedCDRs.MappedCDRsToUpdate.Find(y => y.MappedCDRId == gnrtdCDR.MappedCDRId);

                                                        IEnumerable<string> mappingNumberList = calledNumberMappingManager.GetMappingNumber(rcvdcdr.OperatorID, gnrtdCDR.CalledNumber);

                                                        if (rcvdcdr.CalledNumber == gnrtdCDR.CalledNumber && rcvdcdr.AttemptDateTime.Subtract(gnrtdCDR.AttemptDateTime) <= dateTimeMargin)
                                                        {
                                                            updateMappedCDRs.UpdatedIds.Add(gnrtdCDR.MappedCDRId);
                                                            rvd.IsCorrelated = true;
                                                            gntd.IsCorrelated = true;
                                                            correlatedCDR.GeneratedCallingNumber = gnrtdCDR.CallingNumber;
                                                        }
                                                        else if (mappingNumberList != null && mappingNumberList.Count() >0 && mappingNumberList.Contains(rcvdcdr.CalledNumber))
                                                        {
                                                            updateMappedCDRs.UpdatedIds.Add(gnrtdCDR.MappedCDRId);
                                                            rvd.IsCorrelated = true;
                                                            gntd.IsCorrelated = true;
                                                            correlatedCDR.GeneratedCallingNumber = gnrtdCDR.CallingNumber;
                                                        }
                                                    }
                                                }

                                                if (!rvd.IsCorrelated && DateTime.Now.Subtract(rcvdcdr.CreatedTime) > inputArgument.TimeOutMargin)
                                                {
                                                    correlatedCDR.GeneratedCallingNumber = null;
                                                    rvd.IsCorrelated = true;
                                                }

                                                if (rvd.IsCorrelated)
                                                {
                                                    correlatedCDRs.Add(correlatedCDR);
                                                    updateMappedCDRs.UpdatedIds.Add(rcvdcdr.MappedCDRId);
                                                }
                                            }
                                        }
                                    }
                                }

                                // Transform correlation to runtime in order to insert 
                                Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
                                var casesBatch = new CDRCaseBatch();
                                if (correlatedCDRs != null && correlatedCDRs.Count > 0)
                                {
                                    correlationBatch.OutputRecordsToInsert = correlatedCDRManager.CorrelatedCDRsToRuntime(correlatedCDRs);

                                    // prepare cases
                                    foreach (var correlatedCDR in correlatedCDRs)
                                    {
                                        if (correlatedCDR.ReceivedCallingNumberType != ReceivedCallingNumberType.International || String.IsNullOrEmpty(correlatedCDR.CalledNumber) || String.IsNullOrEmpty(correlatedCDR.GeneratedCallingNumber))
                                        {
                                            casesBatch.OutputRecordsToInsert.Add(correlatedCDR);
                                        }
                                    }
                                }
                                inputArgument.OutputCorrelationBatchQueue.Enqueue(correlationBatch);
                                inputArgument.UpdatedCDRCorrelationBatchInput.Enqueue(correlationBatch);
                                inputArgument.OutputCaseQueue.Enqueue(casesBatch);
                                inputArgument.UpdatedMappedCDRsInput.Enqueue(updateMappedCDRs);
                            }
                        });
                    }
                } while (!ShouldStop(handle) && hasItems);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Correlate CDRs is done.");
        }
    }
}
