using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
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
        public TCAnalSettingsData TCAnalSettingsData { get; set; }
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
        public InArgument<TCAnalSettingsData> TCAnalSettingsData { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> OutputCorrelationBatchQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCaseBatch>> OutputCaseQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.UpdatedMappedCDRs>> UpdatedMappedCDRsInput { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> UpdatedCDRCorrelationBatchInput { get; set; }

        public static Guid countryCodesAccountPartDefinition = new Guid("22144C7D-42B7-4503-A665-FD547856BB43");
        public static Guid operatorsAccountBEdefiniton = new Guid("d4028716-97aa-4664-8eaa-35b99603b2e7");

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
                TCAnalSettingsData = this.TCAnalSettingsData.Get(context),
                OutputCorrelationBatchQueue = this.OutputCorrelationBatchQueue.Get(context),
                OutputCaseQueue = this.OutputCaseQueue.Get(context),
                UpdatedMappedCDRsInput = this.UpdatedMappedCDRsInput.Get(context),
                UpdatedCDRCorrelationBatchInput = this.UpdatedCDRCorrelationBatchInput.Get(context),
            };
        }

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.TCAnalSettingsData.TimeMargin;
            TimeSpan timeOutMargin = inputArgument.TCAnalSettingsData.TimeOutMargin;
            CalledNumberMappingManager calledNumberMappingManager = new CalledNumberMappingManager();
            MappedCDRManager mappedCDRManager = new MappedCDRManager();
            CorrelatedCDRManager correlatedCDRManager = new CorrelatedCDRManager();
            AccountBEManager accountBEManager = new AccountBEManager();

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
                                Dictionary<long, List<TCAnalMappedCDR>> receivedCDRsByOperatorId = new Dictionary<long, List<TCAnalMappedCDR>>();
                                Dictionary<long, List<TCAnalMappedCDR>> generatedCDRsByOperatorId = new Dictionary<long, List<TCAnalMappedCDR>>();
                                Entities.UpdatedMappedCDRs updateMappedCDRs = new Entities.UpdatedMappedCDRs();
                                updateMappedCDRs.MappedCDRsToUpdate = new List<TCAnalMappedCDR>();

                                // Divide all CDRs between generated and received
                                foreach (var cdr in recordBatch.Records)
                                {
                                    TCAnalMappedCDR mappedCDR = mappedCDRManager.MappedCDRMapper(cdr);
                                    if (mappedCDR.CDRType.Equals(CDRType.Generated))
                                        generatedCDRsByOperatorId.GetOrCreateItem(mappedCDR.CalledOperatorID.Value).Add(mappedCDR);
                                    else
                                        receivedCDRsByOperatorId.GetOrCreateItem(mappedCDR.CalledOperatorID.Value).Add(mappedCDR);
                                }

                                // Create the correlatedCDRs list
                                List<TCAnalCorrelatedCDR> correlatedCDRs = new List<TCAnalCorrelatedCDR>();
                                if (receivedCDRsByOperatorId != null && receivedCDRsByOperatorId.Count > 0 && generatedCDRsByOperatorId != null && generatedCDRsByOperatorId.Count > 0)
                                {
                                    foreach (var recievedOperator in receivedCDRsByOperatorId)
                                    {
                                        if (recievedOperator.Value != null && recievedOperator.Value.Count > 0)
                                        {
                                            foreach (var rcvdcdr in recievedOperator.Value)
                                            {
                                                if (rcvdcdr.CalledOperatorID.HasValue)
                                                {
                                                    TCAnalCorrelatedCDR correlatedCDR = new TCAnalCorrelatedCDR();
                                                    correlatedCDR = correlatedCDRManager.CorrelatedCDRMapper(rcvdcdr);

                                                    IEnumerable<string> mappingNumberList = calledNumberMappingManager.GetMappingNumber(rcvdcdr.CalledOperatorID.Value, rcvdcdr.CalledNumber);

                                                    var generatedCDRs = generatedCDRsByOperatorId.GetRecord(recievedOperator.Key);
                                                    if (generatedCDRs != null && generatedCDRs.Count > 0)
                                                    {
                                                        foreach (var generatedCDR in generatedCDRs)
                                                        {
                                                            if (!generatedCDR.IsCorrelated && rcvdcdr.AttemptDateTime.Subtract(generatedCDR.AttemptDateTime) <= dateTimeMargin)
                                                            {
                                                                if (rcvdcdr.CalledNumber == generatedCDR.CalledNumber || (mappingNumberList != null && mappingNumberList.Count() > 0 && mappingNumberList.Contains(generatedCDR.CalledNumber)))
                                                                {
                                                                    rcvdcdr.IsCorrelated = true;
                                                                    generatedCDR.IsCorrelated = true;
                                                                    updateMappedCDRs.MappedCDRsToUpdate.Add(rcvdcdr);
                                                                    updateMappedCDRs.MappedCDRsToUpdate.Add(generatedCDR);
                                                                    correlatedCDR.GeneratedCalledNumber = generatedCDR.CalledNumber;
                                                                    correlatedCDR.GeneratedCallingNumber = generatedCDR.CallingNumber;
                                                                    correlatedCDR.OrigGeneratedCalledNumber = generatedCDR.OrigCalledNumber;
                                                                    correlatedCDR.OrigGeneratedCallingNumber = generatedCDR.OrigCallingNumber;
                                                                    correlatedCDR.GeneratedId = generatedCDR.MappedCDRId;
                                                                    correlatedCDRs.Add(correlatedCDR);
                                                                    break;
                                                                }
                                                            }
                                                            if (rcvdcdr.IsCorrelated)
                                                                break;
                                                        }
                                                    }
                                                    if (!rcvdcdr.IsCorrelated && DateTime.Now.Subtract(rcvdcdr.CreatedTime) > timeOutMargin)
                                                    {
                                                        correlatedCDR.GeneratedCalledNumber = null;
                                                        correlatedCDR.GeneratedCallingNumber = null;
                                                        correlatedCDR.OrigGeneratedCalledNumber = null;
                                                        correlatedCDR.OrigGeneratedCallingNumber = null;
                                                        correlatedCDR.GeneratedId = null;
                                                        rcvdcdr.IsCorrelated = true;
                                                        updateMappedCDRs.MappedCDRsToUpdate.Add(rcvdcdr);
                                                        correlatedCDRs.Add(correlatedCDR);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Insert Correlations
                                Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
                                var casesBatch = new CDRCaseBatch();
                                casesBatch.CaseCDRsToInsert = new List<TCAnalCorrelatedCDR>();
                                if (correlatedCDRs != null && correlatedCDRs.Count > 0)
                                {
                                    long correlatedCDRStartingId = correlatedCDRManager.ReserveIDRange(correlatedCDRs.Count());

                                    foreach(var correlatedCDR in correlatedCDRs)
                                    {
                                        correlatedCDR.CorrelatedCDRId = correlatedCDRStartingId++;
                                        casesBatch.CaseCDRsToInsert.Add(correlatedCDR);
                                    }

                                    correlationBatch.OutputRecordsToInsert = correlatedCDRManager.CorrelatedCDRsToRuntime(correlatedCDRs);
                                }
                                inputArgument.OutputCorrelationBatchQueue.Enqueue(correlationBatch);
                                inputArgument.UpdatedCDRCorrelationBatchInput.Enqueue(correlationBatch);
                                inputArgument.OutputCaseQueue.Enqueue(casesBatch);
                                inputArgument.UpdatedMappedCDRsInput.Enqueue(updateMappedCDRs);
                            }
                        });
                    }
                    else
                    {
                        hasItems = false;
                    }
                } while (!ShouldStop(handle) && hasItems);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Correlate CDRs is done.");
        }
    }
}
