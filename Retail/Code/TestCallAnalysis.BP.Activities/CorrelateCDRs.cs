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
                                Dictionary<long, Dictionary<DateTime, List<TCAnalMappedCDR>>> receivedCDRsByOperatorId = new Dictionary<long, Dictionary<DateTime, List<TCAnalMappedCDR>>>();
                                Dictionary<long, Dictionary<DateTime, List<TCAnalMappedCDR>>> generatedCDRsByOperatorId = new Dictionary<long, Dictionary<DateTime, List<TCAnalMappedCDR>>>();
                                Entities.UpdatedMappedCDRs updateMappedCDRs = new Entities.UpdatedMappedCDRs();
                                updateMappedCDRs.MappedCDRsToUpdate = new List<TCAnalMappedCDR>();

                                Dictionary<long,List<DateTime>> attemptDateTimesByOperatorIds = new Dictionary<long, List<DateTime>>();
                                // Divide all CDRs between generated and received
                                foreach (var cdr in recordBatch.Records)
                                {
                                    TCAnalMappedCDR mappedCDR = mappedCDRManager.MappedCDRMapper(cdr);

                                    if (mappedCDR.CDRType.Equals(CDRType.Generated))
                                    {
                                       var generatedCdrsByCalledNumber = generatedCDRsByOperatorId.GetOrCreateItem(mappedCDR.CalledOperatorID.Value);
                                        generatedCdrsByCalledNumber.GetOrCreateItem(mappedCDR.AttemptDateTime.Date).Add(mappedCDR);
                                    }
                                    else
                                    {
                                        var attemptDateTimes = attemptDateTimesByOperatorIds.GetOrCreateItem(mappedCDR.CalledOperatorID.Value);
                                        if (!attemptDateTimes.Contains(mappedCDR.AttemptDateTime.Date))
                                            attemptDateTimes.Add(mappedCDR.AttemptDateTime.Date);

                                        var receivedCDRsByCalledNumber = receivedCDRsByOperatorId.GetOrCreateItem(mappedCDR.CalledOperatorID.Value);
                                        receivedCDRsByCalledNumber.GetOrCreateItem(mappedCDR.AttemptDateTime.Date).Add(mappedCDR);
                                    }
                                }

                                // Create the correlatedCDRs list
                                List<TCAnalCorrelatedCDR> correlatedCDRs = new List<TCAnalCorrelatedCDR>();

                                if (attemptDateTimesByOperatorIds.Count > 0)
                                {
                                    foreach (var attemptDateTimesByOperatorId in attemptDateTimesByOperatorIds)
                                    {
                                        var attemptDateTimes = attemptDateTimesByOperatorId.Value;
                                        var operatorId = attemptDateTimesByOperatorId.Key;
                                        if (attemptDateTimes != null && attemptDateTimes.Count > 0)
                                        {
                                            var receivedCDRsForOperator = receivedCDRsByOperatorId.GetRecord(operatorId);
                                            var generatedCDRsForOperator = generatedCDRsByOperatorId.GetRecord(operatorId);

                                            if(receivedCDRsForOperator != null)
                                            {
                                                foreach (var attemptDateTime in attemptDateTimes)
                                                {
                                                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Start Correlation for day {0:yyyy/MM/dd}.", attemptDateTime));

                                                    var recievedCDRs = receivedCDRsForOperator.GetRecord(attemptDateTime);
                                                    if(recievedCDRs != null)
                                                    {
                                                        List<TCAnalMappedCDR> generatedCDRs = null;
                                                          if(generatedCDRsForOperator != null)
                                                             generatedCDRs= generatedCDRsForOperator.GetRecord(attemptDateTime);

                                                        foreach (var recievedCDR in recievedCDRs)
                                                        {
                                                            TCAnalCorrelatedCDR correlatedCDR = new TCAnalCorrelatedCDR();
                                                            correlatedCDR = correlatedCDRManager.CorrelatedCDRMapper(recievedCDR);

                                                            if (generatedCDRs != null && generatedCDRs.Count > 0)
                                                            {
                                                                IEnumerable<string> mappingNumberList = calledNumberMappingManager.GetMappingNumber(operatorId, recievedCDR.CalledNumber);
                                                                foreach (var generatedCDR in generatedCDRs)
                                                                {
                                                                    if (recievedCDR.AttemptDateTime.Subtract(generatedCDR.AttemptDateTime) <= dateTimeMargin)
                                                                    {
                                                                        if (recievedCDR.CalledNumber == generatedCDR.CalledNumber || (mappingNumberList != null && mappingNumberList.Count() > 0 && mappingNumberList.Contains(generatedCDR.CalledNumber)))
                                                                        {
                                                                            recievedCDR.IsCorrelated = true;
                                                                            generatedCDR.IsCorrelated = true;
                                                                            updateMappedCDRs.MappedCDRsToUpdate.Add(recievedCDR);
                                                                            updateMappedCDRs.MappedCDRsToUpdate.Add(generatedCDR);
                                                                            correlatedCDR.GeneratedCalledNumber = generatedCDR.CalledNumber;
                                                                            correlatedCDR.GeneratedCallingNumber = generatedCDR.CallingNumber;
                                                                            correlatedCDR.OrigGeneratedCalledNumber = generatedCDR.OrigCalledNumber;
                                                                            correlatedCDR.OrigGeneratedCallingNumber = generatedCDR.OrigCallingNumber;
                                                                            correlatedCDR.GeneratedId = generatedCDR.MappedCDRId;
                                                                            correlatedCDR.OriginatedGeneratedZoneId = generatedCDR.OriginatedZoneId;
                                                                            correlatedCDRs.Add(correlatedCDR);
                                                                            generatedCDRs.Remove(generatedCDR);
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                if (recievedCDR.IsCorrelated)
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            if (!recievedCDR.IsCorrelated && DateTime.Now.Subtract(recievedCDR.CreatedTime) > timeOutMargin)
                                                            {
                                                                correlatedCDR.GeneratedCalledNumber = null;
                                                                correlatedCDR.GeneratedCallingNumber = null;
                                                                correlatedCDR.OrigGeneratedCalledNumber = null;
                                                                correlatedCDR.OrigGeneratedCallingNumber = null;
                                                                correlatedCDR.GeneratedId = null;
                                                                correlatedCDR.OriginatedGeneratedZoneId = null;
                                                                recievedCDR.IsCorrelated = true;
                                                                updateMappedCDRs.MappedCDRsToUpdate.Add(recievedCDR);
                                                                correlatedCDRs.Add(correlatedCDR);
                                                            }
                                                        }
                                                    }
                                                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Finish Correlation for day {0:yyyy/MM/dd}.", attemptDateTime));
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
