using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities
{
    public class CorrelateCDRsInput
    {
        public BaseQueue<RecordBatch> InputQueue { get; set; }

        public TimeSpan DateTimeMargin { get; set; }

        public TimeSpan DurationMargin { get; set; }

        public CDRCorrelationDefinition CDRCorrelationDefinition { get; set; }

        public BaseQueue<CDRCorrelationBatch> OutputQueue { get; set; }
    }

    public sealed class CorrelateCDRs : DependentAsyncActivity<CorrelateCDRsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<RecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DurationMargin { get; set; }

        [RequiredArgument]
        public InArgument<CDRCorrelationDefinition> CDRCorrelationDefinition { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRCorrelationBatch>> OutputQueue { get; set; }

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;
            TimeSpan durationMargin = inputArgument.DurationMargin;
            CDRCorrelationDefinition cdrCorrelationDefinition = inputArgument.CDRCorrelationDefinition;

            List<MobileCDR> uncorrelatedCDRs = new List<MobileCDR>();
            Dictionary<string, List<MobileCDR>> uncorrelatedCDRsByCDPNDict = new Dictionary<string, List<MobileCDR>>();

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
                                DateTime maxDateTime = DateTime.MinValue;
                                CDRCorrelationBatch cdrCorrelationBatch = new CDRCorrelationBatch();

                                foreach (var cdr in recordBatch.Records)
                                {
                                    MobileCDR matchingMobileCDR = null;
                                    MobileCDR mobileCDR = new MobileCDR(cdr, cdrCorrelationDefinition);

                                    maxDateTime = mobileCDR.AttemptDateTime;
                                    List<MobileCDR> uncorrelatedCDRsByCDPN = uncorrelatedCDRsByCDPNDict.GetRecord(mobileCDR.CDPN);
                                    if (uncorrelatedCDRsByCDPN != null && uncorrelatedCDRsByCDPN.Count > 0)
                                    {
                                        for (var index = 0; index < uncorrelatedCDRsByCDPN.Count; index++)
                                        {
                                            var uncorrelatedCDR = uncorrelatedCDRsByCDPN[index];

                                            if (IsMatching(mobileCDR, uncorrelatedCDR, cdrCorrelationDefinition, dateTimeMargin, durationMargin.Seconds))
                                            {
                                                matchingMobileCDR = uncorrelatedCDR;

                                                uncorrelatedCDRs.Remove(matchingMobileCDR);
                                                if (uncorrelatedCDRsByCDPN.Count > 1)
                                                    uncorrelatedCDRsByCDPN.Remove(matchingMobileCDR);
                                                else
                                                    uncorrelatedCDRsByCDPNDict.Remove(matchingMobileCDR.CDPN);
                                                break;
                                            }
                                        }
                                    }

                                    if (matchingMobileCDR != null)
                                    {
                                        var output = new DataTransformer().ExecuteDataTransformation(cdrCorrelationDefinition.Settings.MergeDataTransformationDefinitionId, (context) =>
                                        {
                                            context.SetRecordValue("InputList", new List<dynamic> { mobileCDR.CDR, matchingMobileCDR.CDR });
                                        });

                                        DateTime recordDateTime = output.GetRecordValue("RecordDateTime");
                                        List<dynamic> correlatedCDRList = output.GetRecordValue("OutputList");

                                        long cdrId = mobileCDR.CDR.GetFieldValue(cdrCorrelationDefinition.Settings.IdFieldName);
                                        long correlatedCDRId = matchingMobileCDR.CDR.GetFieldValue(cdrCorrelationDefinition.Settings.IdFieldName);

                                        cdrCorrelationBatch.OutputRecordsToInsert.Add(correlatedCDRList.First());
                                        cdrCorrelationBatch.InputIdsToDelete.AddRange(new List<long> { cdrId, correlatedCDRId });

                                        DateTime from = cdrCorrelationBatch.DateTimeRange.From;
                                        if (from == default(DateTime) || from > recordDateTime)
                                            cdrCorrelationBatch.DateTimeRange.From = recordDateTime;

                                        DateTime to = cdrCorrelationBatch.DateTimeRange.To;
                                        if (to == default(DateTime) || to < recordDateTime)
                                            cdrCorrelationBatch.DateTimeRange.To = recordDateTime.AddSeconds(1);
                                    }
                                    else
                                    {
                                        uncorrelatedCDRs.Add(mobileCDR);
                                        List<MobileCDR> tempCorrelatedCDRs = uncorrelatedCDRsByCDPNDict.GetOrCreateItem(mobileCDR.CDPN);
                                        tempCorrelatedCDRs.Add(mobileCDR);
                                    }
                                }

                                //Cleaning List And Dict
                                DateTime minDateTime = maxDateTime.AddSeconds(-dateTimeMargin.Seconds);
                                for (var index = uncorrelatedCDRs.Count - 1; index >= 0; index--)
                                {
                                    MobileCDR currentMobileCDR = uncorrelatedCDRs[index];
                                    if (currentMobileCDR.AttemptDateTime >= minDateTime)
                                        continue;

                                    while (index >= 0)
                                    {
                                        currentMobileCDR = uncorrelatedCDRs[index];
                                        uncorrelatedCDRs.Remove(currentMobileCDR);

                                        List<MobileCDR> tempCorrelatedCDRs = uncorrelatedCDRsByCDPNDict.GetOrCreateItem(currentMobileCDR.CDPN);
                                        if (tempCorrelatedCDRs.Count > 1)
                                            tempCorrelatedCDRs.Remove(currentMobileCDR);
                                        else
                                            uncorrelatedCDRsByCDPNDict.Remove(currentMobileCDR.CDPN);

                                        index--;
                                    }
                                    break;
                                }

                                if (cdrCorrelationBatch.OutputRecordsToInsert.Count > 0)
                                    inputArgument.OutputQueue.Enqueue(cdrCorrelationBatch);

                                double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Process {0} CDRs. {1} CDRs correlated. ElapsedTime: {2} (s)",
                                    recordBatch.Records.Count, cdrCorrelationBatch.OutputRecordsToInsert.Count, elapsedTime.ToString());
                            }
                        });
                } while (!ShouldStop(handle) && hasItems);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Correlate CDRs is done.");
        }

        private bool IsMatching(MobileCDR firstMobileCDR, MobileCDR secondMobileCDR, CDRCorrelationDefinition cdrCorrelationDefinition, TimeSpan dateTimeMargin, int durationMarginInSeconds)
        {
            if ((firstMobileCDR.AttemptDateTime - secondMobileCDR.AttemptDateTime) > dateTimeMargin)
                return false;

            if (Math.Abs(firstMobileCDR.Duration - secondMobileCDR.Duration) > durationMarginInSeconds)
                return false;

            if (string.Compare(firstMobileCDR.CGPN, secondMobileCDR.CGPN) != 0)
                return false;

            //if (string.Compare(firstCorrelatedCDR.CDPN, secondCorrelatedCDR.CDPN) != 0)
            //    return false;

            if (firstMobileCDR.RecordType == secondMobileCDR.RecordType)
                return false;

            return true;
        }

        protected override CorrelateCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CorrelateCDRsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                DateTimeMargin = this.DateTimeMargin.Get(context),
                DurationMargin = this.DurationMargin.Get(context),
                CDRCorrelationDefinition = this.CDRCorrelationDefinition.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }

    public class MobileCDR
    {
        public int RecordType { get; set; }

        public DateTime AttemptDateTime { get; set; }

        public int Duration { get; set; }

        public string CGPN { get; set; }

        public string CDPN { get; set; }

        public dynamic CDR { get; set; }

        public MobileCDR(dynamic cdr, CDRCorrelationDefinition cdrCorrelationDefinition)
        {
            RecordType = cdr.GetFieldValue("RecordType");
            AttemptDateTime = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.DatetimeFieldName);
            Duration = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.DurationFieldName);
            CGPN = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.CallingNumberFieldName);
            CDPN = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.CalledNumberFieldName);
            CDR = cdr;
        }
    }
}