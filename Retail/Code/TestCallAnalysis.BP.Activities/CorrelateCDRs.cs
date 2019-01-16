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
using Vanrise.Integration.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    public class CorrelateCDRsInput
    {
        public MemoryQueue<RecordBatch> InputQueue { get; set; }
        public TimeSpan DateTimeMargin { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> OutputQueue { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> OutputCaseQueue { get; set; }

    }
    public class TCAnalMappedCDR
    {
        public Guid ID { get; set; }
        public DataSource DataSourceId { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string CalledNumber { get; set; }
        public string CallingNumber { get; set; }
        public CDRType CDRType { get; set; }
        public long OperatorID { get; set; }
        public string OrigCallingNumber { get; set; }
        public string OrigCalledNumber { get; set; }
        public string CLI { get; set; }

    }
    public class TCAnalCorrelatedCDR
    {
        public Guid ID { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string CalledNumber { get; set; }
        public string CallingNumber { get; set; }
        public long OperatorID { get; set; }
        public string OrigCallingNumber { get; set; }
        public string OrigCalledNumber { get; set; }
        public string CLI { get; set; }
        public Guid CaseId { get; set; }
    }
    public enum CDRType
    {
        Generated=1,Recieved=2
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

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                    {
                        DateTime batchStartTime = DateTime.Now;

                        if (recordBatch.Records != null && recordBatch.Records.Count > 0)
                        {
                            RecordBatch records = new RecordBatch() { Records = new List<dynamic>() };
                            
                            Dictionary<long, List<TCAnalMappedCDR>> recievedCDRs = new Dictionary<long, List<TCAnalMappedCDR>>();
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
                                    CDRType = cdr.CDRType,
                                    OperatorID = cdr.OperatorID,
                                    OrigCallingNumber = cdr.OrigCallingNumber,
                                    OrigCalledNumber = cdr.OrigCalledNumber,
                                    CLI = cdr.CLI
                                };
                                if (mappedCDR.CDRType.Equals(1))
                                {
                                    generatedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);

                                }
                                else
                                    recievedCDRs.GetOrCreateItem(mappedCDR.OperatorID).Add(mappedCDR);

                            }

                           
                            List<TCAnalCorrelatedCDR> correlatedCDRs = null;
                            if (recievedCDRs != null && recievedCDRs.Count > 0 && generatedCDRs != null && generatedCDRs.Count > 0)
                            {
                                foreach (var recievedCDR in recievedCDRs)
                                {if (recievedCDR.Value != null && recievedCDR.Value.Count > 0)
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
                                                            correlatedCDRs.Add(new TCAnalCorrelatedCDR
                                                            {
                                                                ID = rcvdcdr.ID,
                                                                AttemptDateTime = rcvdcdr.AttemptDateTime,
                                                                DurationInSeconds = rcvdcdr.DurationInSeconds,
                                                                CalledNumber = rcvdcdr.CalledNumber,
                                                                CallingNumber = rcvdcdr.CallingNumber,
                                                                OperatorID = rcvdcdr.OperatorID,
                                                                OrigCallingNumber = rcvdcdr.OrigCallingNumber,
                                                                OrigCalledNumber = rcvdcdr.OrigCalledNumber,
                                                                CLI = rcvdcdr.CLI
                                                            });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
                            if (correlatedCDRs!=null && correlatedCDRs.Count > 0)
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
                                    runtimeCDR.CallingNumber = correlatedCDR.CallingNumber;
                                    runtimeCDR.OperatorID = correlatedCDR.OperatorID;
                                    runtimeCDR.OrigCallingNumber = correlatedCDR.OrigCallingNumber;
                                    runtimeCDR.OrigCalledNumber = correlatedCDR.OrigCalledNumber;
                                    runtimeCDR.CLI = correlatedCDR.CLI;


                                    correlationBatch.OutputRecordsToInsert.Add(runtimeCDR);
                                }

                            }
                            inputArgument.OutputQueue.Enqueue(correlationBatch);
                            inputArgument.OutputCaseQueue.Enqueue(correlationBatch);
                        }

                    });
                } while (!ShouldStop(handle) && hasItems);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Correlate CDRs is done.");
        }
    }
}
