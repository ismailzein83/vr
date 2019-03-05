using Retail.BusinessEntity.Business;
using System;
using System.Activities;
using System.Collections.Generic;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation;
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
        public MemoryQueue<UpdatedMappedCDRs> UpdatedMappedCDRsInput { get; set; }
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
        public InOutArgument<MemoryQueue<UpdatedMappedCDRs>> UpdatedMappedCDRsInput { get; set; }

        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> UpdatedCDRCorrelationBatchInput { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputCorrelationBatchQueue.Get(context) == null)
                this.OutputCorrelationBatchQueue.Set(context, new MemoryQueue<Entities.CDRCorrelationBatch>());
            if (this.OutputCaseQueue.Get(context) == null)
                this.OutputCaseQueue.Set(context, new MemoryQueue<Entities.CDRCaseBatch>());
            if (this.UpdatedMappedCDRsInput.Get(context) == null)
                this.UpdatedMappedCDRsInput.Set(context, new MemoryQueue<UpdatedMappedCDRs>());
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
                                UpdatedMappedCDRs updateMappedCDRs = new UpdatedMappedCDRs();
                                CalledNumberMappingManager calledNumberMappingManager = new CalledNumberMappingManager();

                                // Divide all CDRs between generated and received
                                foreach (var cdr in recordBatch.Records)
                                {
                                    TCAnalMappedCDR mappedCDR = new TCAnalMappedCDR
                                    {
                                        ID = cdr.ID,
                                        DataSourceId = cdr.DataSourceId,
                                        AttemptDateTime = cdr.AttemptDateTime,
                                        DurationInSeconds = cdr.DurationInSeconds,
                                        CalledNumber = cdr.CalledNumber,
                                        CallingNumber = cdr.CallingNumber,
                                        CDRType = (CDRType)cdr.CDRType,
                                        OperatorID = cdr.OperatorID,
                                        OrigCallingNumber = cdr.OrigCallingNumber,
                                        OrigCalledNumber = cdr.OrigCalledNumber,
                                        CreatedDate = cdr.CreatedDate,
                                        CallingNumberType = cdr.CallingNumberType,
                                        CalledNumberType = cdr.CalledNumberType,
                                        IsCorrelated = cdr.IsCorrelated
                                    };

                                    if (mappedCDR.CDRType.Equals(CDRType.Generated))
                                        generatedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);

                                    else
                                        receivedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);
                                    
                                    updateMappedCDRs.MappedCDRsToUpdate.Add(mappedCDR); // List of all mapped cdrs to update the "IsCorrelated" field
                                }

                                List<TCAnalCorrelatedCDR> correlatedCDRs = new List<TCAnalCorrelatedCDR>();
                                if (receivedCDRs != null && receivedCDRs.Count > 0 && generatedCDRs != null && generatedCDRs.Count > 0)
                                {
                                    var mappingRuleManager = new MappingRuleManager();
                                    var accountBEManager = new AccountBEManager();
                                    var identificationRuleDefinitionId = new Guid("F3B8689D-AA16-46A6-BE4F-202626231C6F");
                                    var numberTypeRuleDefinitionId = new Guid("60ED7A5B-18A0-40CD-BF45-63E70C1BC01C");
                                 

                                    foreach (var recievedCDR in receivedCDRs)
                                    {
                                        if (recievedCDR.Value != null && recievedCDR.Value.Count > 0)
                                        {
                                            foreach (var rcvdcdr in recievedCDR.Value)
                                            {
                                                TCAnalCorrelatedCDR correlatedCDR = new TCAnalCorrelatedCDR();
                                                var rcvdIndex = updateMappedCDRs.MappedCDRsToUpdate.IndexOf(rcvdcdr);

                                                var generatedCDR = generatedCDRs.GetRecord(recievedCDR.Key);
                                                if (generatedCDR != null && generatedCDR.Count > 0)
                                                {
                                                    foreach (var gnrtdCDR in generatedCDR)
                                                    {
                                                        var gnrtdIndex = updateMappedCDRs.MappedCDRsToUpdate.IndexOf(gnrtdCDR);

                                                        correlatedCDR.ID = rcvdcdr.ID;
                                                        correlatedCDR.AttemptDateTime = rcvdcdr.AttemptDateTime;
                                                        correlatedCDR.DurationInSeconds = rcvdcdr.DurationInSeconds;
                                                        correlatedCDR.CalledNumber = rcvdcdr.CalledNumber;
                                                        correlatedCDR.ReceivedCallingNumber = rcvdcdr.CallingNumber;
                                                        correlatedCDR.OperatorID = rcvdcdr.OperatorID;
                                                        correlatedCDR.OrigCallingNumber = rcvdcdr.OrigCallingNumber;
                                                        correlatedCDR.OrigCalledNumber = rcvdcdr.OrigCalledNumber;
                                                        correlatedCDR.ReceivedCallingNumberOperatorID = rcvdcdr.OperatorID;
                                                        correlatedCDR.ReceivedCallingNumberType = (ReceivedCallingNumberType?)rcvdcdr.CallingNumberType;
                                                        correlatedCDR.CaseId = null;

                                                        List<string> mappingNumberList = calledNumberMappingManager.GetMappingNumber(rcvdcdr.OperatorID, gnrtdCDR.CalledNumber);

                                                        if (rcvdcdr.CalledNumber == gnrtdCDR.CalledNumber && rcvdcdr.AttemptDateTime.Subtract(gnrtdCDR.AttemptDateTime) <= dateTimeMargin)
                                                        {
                                                            updateMappedCDRs.UpdatedIds.Add(gnrtdCDR.ID);
                                                            updateMappedCDRs.MappedCDRsToUpdate[gnrtdIndex].IsCorrelated = true;
                                                            updateMappedCDRs.MappedCDRsToUpdate[rcvdIndex].IsCorrelated = true;
                                                            correlatedCDR.GeneratedCallingNumber = gnrtdCDR.CallingNumber;
                                                            rcvdcdr.IsCorrelated = true;
                                                        }
                                                        else if (mappingNumberList != null && mappingNumberList.Count >0 && mappingNumberList.Contains(rcvdcdr.CalledNumber))
                                                        {
                                                            updateMappedCDRs.UpdatedIds.Add(gnrtdCDR.ID);
                                                            updateMappedCDRs.MappedCDRsToUpdate[gnrtdIndex].IsCorrelated = true;
                                                            updateMappedCDRs.MappedCDRsToUpdate[rcvdIndex].IsCorrelated = true;
                                                            correlatedCDR.GeneratedCallingNumber = gnrtdCDR.CallingNumber;
                                                            rcvdcdr.IsCorrelated = true;
                                                        }
                                                    }
                                                }

                                                if (!rcvdcdr.IsCorrelated  && DateTime.Now.Subtract(rcvdcdr.CreatedDate) > inputArgument.TimeOutMargin)
                                                {
                                                    correlatedCDR.GeneratedCallingNumber = null;
                                                    rcvdcdr.IsCorrelated = true;
                                                    updateMappedCDRs.MappedCDRsToUpdate[rcvdIndex].IsCorrelated = true;
                                                }

                                                if (rcvdcdr.IsCorrelated)
                                                {
                                                    correlatedCDRs.Add(correlatedCDR);
                                                    updateMappedCDRs.UpdatedIds.Add(rcvdcdr.ID);
                                                }
                                            }
                                        }
                                    }
                                }

                                // Transform correlation to runtime in order to insert the batch and insert the fraud and suspect cases in their corresponding argument
                                Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
                                var casesBatch = new CDRCaseBatch();
                                if (correlatedCDRs != null && correlatedCDRs.Count > 0)
                                {
                                    var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                                    Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_CorrelatedCDR");

                                    foreach (var correlatedCDR in correlatedCDRs)
                                    {
                                        dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                                        runtimeCDR.ID = correlatedCDR.ID;
                                        runtimeCDR.AttemptDateTime = correlatedCDR.AttemptDateTime;
                                        runtimeCDR.DurationInSeconds = correlatedCDR.DurationInSeconds;
                                        runtimeCDR.CalledNumber = correlatedCDR.CalledNumber;
                                        runtimeCDR.OperatorID = correlatedCDR.OperatorID;
                                        runtimeCDR.OrigCallingNumber = correlatedCDR.OrigCallingNumber;
                                        runtimeCDR.OrigCalledNumber = correlatedCDR.OrigCalledNumber;
                                        runtimeCDR.GeneratedCallingNumber = correlatedCDR.GeneratedCallingNumber;
                                        runtimeCDR.ReceivedCallingNumber = correlatedCDR.ReceivedCallingNumber;
                                        runtimeCDR.ReceivedCallingNumberType = (int?)correlatedCDR.ReceivedCallingNumberType;
                                        runtimeCDR.ReceivedCallingNumberOperatorID = correlatedCDR.ReceivedCallingNumberOperatorID;
                                        runtimeCDR.CaseId = correlatedCDR.CaseId;
                                        correlationBatch.OutputRecordsToInsert.Add(runtimeCDR);

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
