using Retail.BusinessEntity.Business;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Rules;

namespace TestCallAnalysis.BP.Activities
{
    public class CorrelateCDRsInput
    {
        public MemoryQueue<RecordBatch> InputQueue { get; set; }
        public TimeSpan DateTimeMargin { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> OutputQueue { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> OutputCaseQueue { get; set; }

    }
  
  
    public sealed class CorrelateCDRs : DependentAsyncActivity<CorrelateCDRsInput>
    {
        [RequiredArgument]
        public InArgument<MemoryQueue<RecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> OutputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> OutputCaseQueue { get; set; }

        protected override CorrelateCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CorrelateCDRsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                DateTimeMargin = this.DateTimeMargin.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                OutputCaseQueue = this.OutputCaseQueue.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Entities.CDRCorrelationBatch>());
            if (this.OutputCaseQueue.Get(context) == null)
                this.OutputCaseQueue.Set(context, new MemoryQueue<Entities.CDRCorrelationBatch>());
            base.OnBeforeExecute(context, handle);
        }
     

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItems = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                        {
                            DateTime batchStartTime = DateTime.Now;

                            if (recordBatch.Records != null && recordBatch.Records.Count > 0)
                            {
                                Dictionary<long, List<TCAnalMappedCDR>> receivedCDRs = new Dictionary<long, List<TCAnalMappedCDR>>();
                                Dictionary<long, List<TCAnalMappedCDR>> generatedCDRs = new Dictionary<long, List<TCAnalMappedCDR>>();

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
                                    };
                                    if (mappedCDR.CDRType.Equals(CDRType.Generated))
                                    {
                                        generatedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);

                                    }
                                    else
                                    {
                                        receivedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);
                                    }
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
                                                var generatedCDR = generatedCDRs.GetRecord(recievedCDR.Key);
                                                if (generatedCDR != null && generatedCDR.Count > 0)
                                                {
                                                    foreach (var gnrtdCDR in generatedCDR)
                                                    {
                                                        if (rcvdcdr.CallingNumber == gnrtdCDR.CalledNumber && rcvdcdr.AttemptDateTime.Subtract(gnrtdCDR.AttemptDateTime) <= dateTimeMargin)
                                                        {
                                                            var account = accountBEManager.GetAccount(new Guid("d4028716-97aa-4664-8eaa-35b99603b2e7"), rcvdcdr.OperatorID);
                                                            var identificationTarget = new GenericRuleTarget()
                                                            {
                                                                EffectiveOn = DateTime.Now
                                                            };
                                                            var numberTypeTarget = new GenericRuleTarget()
                                                            {
                                                                EffectiveOn = DateTime.Now,
                                                            };
                                                            if (account != null)
                                                                identificationTarget.Objects = new Dictionary<string, dynamic> { { "Operator", account } };
                                                            if (rcvdcdr.CallingNumber != null)
                                                            {
                                                                identificationTarget.TargetFieldValues = new Dictionary<string, object> { { "NumberLength", rcvdcdr.CallingNumber.Length }, { "NumberPrefix", rcvdcdr.CallingNumber } };
                                                                numberTypeTarget.TargetFieldValues = new Dictionary<string, object> { { "NumberLength", rcvdcdr.CallingNumber.Length }, { "NumberPrefix", rcvdcdr.CallingNumber } };
                                                            }

                                                            var correlatedCDR = new TCAnalCorrelatedCDR
                                                            {
                                                                ID = rcvdcdr.ID,
                                                                AttemptDateTime = rcvdcdr.AttemptDateTime,
                                                                DurationInSeconds = rcvdcdr.DurationInSeconds,
                                                                CalledNumber = rcvdcdr.CalledNumber,
                                                                GeneratedCallingNumber = gnrtdCDR.CallingNumber,
                                                                ReceivedCallingNumber = rcvdcdr.CallingNumber,
                                                                OperatorID = rcvdcdr.OperatorID,
                                                                OrigCallingNumber = rcvdcdr.OrigCallingNumber,
                                                                OrigCalledNumber = rcvdcdr.OrigCalledNumber,
                                                            };
                                                            var identificationRule = mappingRuleManager.GetMatchRule(identificationRuleDefinitionId, identificationTarget);
                                                            var numberTypeRule = mappingRuleManager.GetMatchRule(numberTypeRuleDefinitionId, numberTypeTarget);
                                                            if (numberTypeRule != null && numberTypeRule.Settings != null)
                                                            {
                                                                if (Convert.ToInt32(numberTypeRule.Settings.Value)== (int)GeneratedCallingNumberType.International)
                                                                {
                                                                    correlatedCDR.ReceivedCallingNumberOperatorID = null;
                                                                    correlatedCDR.ReceivedCallingNumberType = ReceivedCallingNumberType.International;
                                                                }
                                                                else
                                                                {
                                                                    if (identificationRule!=null && identificationRule.Settings.Value != null)
                                                                    {
                                                                        if ((long)identificationRule.Settings.Value == rcvdcdr.OperatorID)
                                                                        {
                                                                            correlatedCDR.ReceivedCallingNumberType = ReceivedCallingNumberType.Onnet;
                                                                        }
                                                                        else
                                                                        {
                                                                            correlatedCDR.ReceivedCallingNumberType = ReceivedCallingNumberType.Offnet;
                                                                        }
                                                                        correlatedCDR.ReceivedCallingNumberOperatorID = (long)identificationRule.Settings.Value;
                                                                    }
                                                                    else
                                                                    {
                                                                        correlatedCDR.ReceivedCallingNumberOperatorID = null;
                                                                        correlatedCDR.ReceivedCallingNumberType = ReceivedCallingNumberType.Offnet;
                                                                    }
                                                                }
                                                                correlatedCDRs.Add(correlatedCDR);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
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
                                        runtimeCDR.ReceivedCallingNumberType = (int)correlatedCDR.ReceivedCallingNumberType;
                                        runtimeCDR.ReceivedCallingNumberOperatorID = correlatedCDR.ReceivedCallingNumberOperatorID;
                                        correlationBatch.OutputRecordsToInsert.Add(runtimeCDR);
                                        casesBatch.OutputRecordsToInsert.Add(correlatedCDR);
                                    }
                                }
                                inputArgument.OutputQueue.Enqueue(correlationBatch);
                                inputArgument.OutputCaseQueue.Enqueue(correlationBatch);
                            }
                        });
                    }
                } while (!ShouldStop(handle) && hasItems);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Correlate CDRs is done.");
        }
    }
}
