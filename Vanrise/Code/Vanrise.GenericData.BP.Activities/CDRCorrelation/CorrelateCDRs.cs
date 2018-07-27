using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
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
        public OutArgument<BaseQueue<CDRCorrelationBatch>> OutputQueue { get; set; }

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;
            TimeSpan durationMargin = inputArgument.DurationMargin;
            CDRCorrelationDefinition cdrCorrelationDefinition = inputArgument.CDRCorrelationDefinition;

            List<dynamic> uncorrelatedCDRs = new List<dynamic>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                        {
                            if (recordBatch.Records != null && recordBatch.Records.Count > 0)
                            {
                                CDRCorrelationBatch cdrCorrelationBatch = new CDRCorrelationBatch();

                                foreach (var cdr in recordBatch.Records)
                                {
                                    dynamic correlatedCDR = null;
                                    DateTime cdrAttemptDateTime = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.DatetimeFieldName);

                                    for (var index = uncorrelatedCDRs.Count - 1; index >= 0; index--)
                                    {
                                        var uncorrelatedCDR = uncorrelatedCDRs[index];

                                        DateTime uncorrelatedCDRAttemptDateTime = uncorrelatedCDR.GetFieldValue(cdrCorrelationDefinition.Settings.DatetimeFieldName);
                                        if ((cdrAttemptDateTime - uncorrelatedCDRAttemptDateTime) > dateTimeMargin)
                                        {
                                            while (index >= 0)
                                            {
                                                uncorrelatedCDR = uncorrelatedCDRs[index];
                                                uncorrelatedCDRs.Remove(uncorrelatedCDR);
                                                index--;
                                            }
                                            break;
                                        }

                                        if (IsMatching(cdr, uncorrelatedCDR, cdrCorrelationDefinition, durationMargin.Seconds))
                                        {
                                            correlatedCDR = uncorrelatedCDR;
                                            uncorrelatedCDRs.Remove(uncorrelatedCDR);
                                            break;
                                        }
                                    }

                                    if (correlatedCDR != null)
                                    {
                                        var output = new DataTransformer().ExecuteDataTransformation(cdrCorrelationDefinition.Settings.MergeDataTransformationDefinitionId, (context) =>
                                        {
                                            context.SetRecordValue("InputList", new List<dynamic> { cdr, correlatedCDR });
                                        });

                                        DateTime recordDateTime = output.GetRecordValue("RecordDateTime");
                                        List<dynamic> correlatedCDRList = output.GetRecordValue("OutputList");

                                        long cdrId = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.IdFieldName);
                                        long correlatedCDRId = correlatedCDR.GetFieldValue(cdrCorrelationDefinition.Settings.IdFieldName);

                                        cdrCorrelationBatch.OutputRecordsToInsert.Add(correlatedCDRList.First());
                                        cdrCorrelationBatch.InputIdsToDelete.AddRange(new List<long> { cdrId, correlatedCDRId });

                                        DateTime from = cdrCorrelationBatch.DateTimeRange.From;
                                        if (from == default(DateTime) || from > recordDateTime)
                                            cdrCorrelationBatch.DateTimeRange.From = recordDateTime;

                                        DateTime to = cdrCorrelationBatch.DateTimeRange.To;
                                        if (to == default(DateTime) || to < recordDateTime)
                                            cdrCorrelationBatch.DateTimeRange.To = recordDateTime.AddSeconds(1);

                                        inputArgument.OutputQueue.Enqueue(cdrCorrelationBatch);
                                    }
                                    else
                                    {
                                        uncorrelatedCDRs.Add(cdr);
                                    }
                                }
                            }
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
        }

        private bool IsMatching(dynamic firstCDR, dynamic secondCDR, CDRCorrelationDefinition cdrCorrelationDefinition, int durationMarginInSeconds)
        {
            string firstCDRCGPN = firstCDR.GetFieldValue(cdrCorrelationDefinition.Settings.CallingNumberFieldName);
            string secondCDRCGPN = secondCDR.GetFieldValue(cdrCorrelationDefinition.Settings.CallingNumberFieldName);
            if (string.Compare(firstCDRCGPN, secondCDRCGPN) != 0)
                return false;

            string firstCDRCDPN = firstCDR.GetFieldValue(cdrCorrelationDefinition.Settings.CalledNumberFieldName);
            string secondCDRCDPN = secondCDR.GetFieldValue(cdrCorrelationDefinition.Settings.CalledNumberFieldName);
            if (string.Compare(firstCDRCDPN, secondCDRCDPN) != 0)
                return false;

            int firstCDRDuration = firstCDR.GetFieldValue(cdrCorrelationDefinition.Settings.DurationFieldName);
            int secondCDRCDRDuration = secondCDR.GetFieldValue(cdrCorrelationDefinition.Settings.DurationFieldName);
            if ((firstCDRDuration - secondCDRCDRDuration) > durationMarginInSeconds)
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
}