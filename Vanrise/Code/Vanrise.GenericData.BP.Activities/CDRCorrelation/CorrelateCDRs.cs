using System;
using System.Activities;
using System.Collections.Generic;
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

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;
            TimeSpan durationMargin = inputArgument.DurationMargin;
            CDRCorrelationDefinition cdrCorrelationDefinition = inputArgument.CDRCorrelationDefinition;

            int originatedCDRRecordType = 0;
            int terminatedCDRRecordType = 1;
            int callForwardCDRRecordType = 100;

            List<MobileCDR> callForwardMobileCDRList = new List<MobileCDR>();
            Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict = new Dictionary<string, List<MobileCDR>>();
            Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict = new Dictionary<MobileCDR, CallForwardMobileCDRs>();

            List<MobileCDR> uncorrelatedMobileCDRList = new List<MobileCDR>();
            Dictionary<string, List<MobileCDR>> uncorrelatedMobileCDRsByCDPNDict = new Dictionary<string, List<MobileCDR>>();

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
                                    MobileCDR mobileCDR = new MobileCDR(cdr, cdrCorrelationDefinition);
                                    maxDateTime = mobileCDR.AttemptDateTime;

                                    if (string.IsNullOrEmpty(mobileCDR.CGPN) || string.IsNullOrEmpty(mobileCDR.CDPN))
                                        continue;

                                    //International
                                    if (CorrelateIfInternational(mobileCDR, cdrCorrelationBatch, cdrCorrelationDefinition))
                                        continue;

                                    //CallForward
                                    if (mobileCDR.RecordType == callForwardCDRRecordType)
                                    {
                                        callForwardMobileCDRList.Add(mobileCDR);
                                        List<MobileCDR> tempCallForwardMobileCDRs = callForwardMobileCDRsByCGPNDict.GetOrCreateItem(mobileCDR.CGPN);
                                        tempCallForwardMobileCDRs.Add(mobileCDR);
                                        continue;
                                    }

                                    //NormalCall
                                    MobileCDR matchingMobileCDR = null;
                                    List<MobileCDR> uncorrelatedMobileCDRsByCDPN = uncorrelatedMobileCDRsByCDPNDict.GetRecord(mobileCDR.CDPN);
                                    if (uncorrelatedMobileCDRsByCDPN != null && uncorrelatedMobileCDRsByCDPN.Count > 0)
                                    {
                                        for (var index = 0; index < uncorrelatedMobileCDRsByCDPN.Count; index++)
                                        {
                                            var currentUncorrelatedCDR = uncorrelatedMobileCDRsByCDPN[index];

                                            if (IsNormalCallMatching(mobileCDR, currentUncorrelatedCDR, dateTimeMargin, durationMargin.Seconds))
                                            {
                                                matchingMobileCDR = currentUncorrelatedCDR;

                                                uncorrelatedMobileCDRList.Remove(matchingMobileCDR);
                                                if (uncorrelatedMobileCDRsByCDPN.Count > 1)
                                                    uncorrelatedMobileCDRsByCDPN.Remove(matchingMobileCDR);
                                                else
                                                    uncorrelatedMobileCDRsByCDPNDict.Remove(matchingMobileCDR.CDPN);
                                                break;
                                            }
                                        }
                                    }

                                    if (matchingMobileCDR != null)
                                    {
                                        var mergeDataTransformationOutput = new DataTransformer().ExecuteDataTransformation(cdrCorrelationDefinition.Settings.MergeDataTransformationDefinitionId, (context) =>
                                        {
                                            context.SetRecordValue("InputMobileCDRs", new List<dynamic> { mobileCDR.CDR, matchingMobileCDR.CDR });
                                        });

                                        List<dynamic> correlatedCDRs = mergeDataTransformationOutput.GetRecordValue("OutputCorrelatedCDRs");
                                        AddCorrelatedCDR(cdrCorrelationBatch, correlatedCDRs, new List<MobileCDR> { mobileCDR, matchingMobileCDR });
                                    }
                                    else
                                    {
                                        uncorrelatedMobileCDRList.Add(mobileCDR);
                                        List<MobileCDR> tempUncorrelatedMobileCDRs = uncorrelatedMobileCDRsByCDPNDict.GetOrCreateItem(mobileCDR.CDPN);
                                        tempUncorrelatedMobileCDRs.Add(mobileCDR);
                                    }
                                }

                                List<MobileCDR> outdatedMobileCDRs = GetAndCleanOutdatedMobileCDRs(uncorrelatedMobileCDRList, uncorrelatedMobileCDRsByCDPNDict, maxDateTime, dateTimeMargin);

                                CheckCallForwardMatching(outdatedMobileCDRs, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsDict, dateTimeMargin, durationMargin,
                                    originatedCDRRecordType, terminatedCDRRecordType);

                                CorrelateAndCleanOutdatedCallForwardCDRs(callForwardMobileCDRList, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsDict, maxDateTime, dateTimeMargin,
                                    cdrCorrelationBatch, cdrCorrelationDefinition);

                                if (cdrCorrelationBatch.OutputRecordsToInsert.Count > 0)
                                    inputArgument.OutputQueue.Enqueue(cdrCorrelationBatch);

                                double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Process {0} CDRs. {1} CDRs correlated. ElapsedTime: {2} (s)",
                                    recordBatch.Records.Count, cdrCorrelationBatch.OutputRecordsToInsert.Count, elapsedTime.ToString());
                            }
                        });
                } while (!ShouldStop(handle) && hasItems);
            });

            //Finalizing CDRCorrelation
            FinalizingCDRCorrelation(uncorrelatedMobileCDRList, callForwardMobileCDRList, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsDict, inputArgument, handle, cdrCorrelationDefinition,
                dateTimeMargin, durationMargin, originatedCDRRecordType, terminatedCDRRecordType);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Correlate CDRs is done.");
        }

        private bool CorrelateIfInternational(MobileCDR mobileCDR, CDRCorrelationBatch cdrCorrelationBatch, CDRCorrelationDefinition cdrCorrelationDefinition)
        {
            var correlateSingleCDRDataTransformationOutput = new DataTransformer().ExecuteDataTransformation(cdrCorrelationDefinition.Settings.CorrelateSingleCDRDataTransformationDefinitionId, (context) =>
            {
                context.SetRecordValue("InputMobileCDRs", new List<dynamic>() { mobileCDR.CDR });
            });

            List<dynamic> correlatedCDRs = correlateSingleCDRDataTransformationOutput.GetRecordValue("OutputCorrelatedCDRs");
            if (correlatedCDRs != null && correlatedCDRs.Count > 0)
            {
                AddCorrelatedCDR(cdrCorrelationBatch, correlatedCDRs, new List<MobileCDR> { mobileCDR });
                return true;
            }

            return false;
        }

        private List<MobileCDR> GetAndCleanOutdatedMobileCDRs(List<MobileCDR> mobileCDRs, Dictionary<string, List<MobileCDR>> mobileCDRsByCDPN, DateTime maxDateTime, TimeSpan dateTimeMargin)
        {
            List<MobileCDR> outdatedMobileCDRs = new List<MobileCDR>();

            DateTime minDateTime = maxDateTime.AddSeconds(-dateTimeMargin.TotalSeconds);
            for (var index = mobileCDRs.Count - 1; index >= 0; index--)
            {
                MobileCDR currentMobileCDR = mobileCDRs[index];
                if (currentMobileCDR.AttemptDateTime >= minDateTime)
                    continue;

                while (index >= 0)
                {
                    currentMobileCDR = mobileCDRs[index];

                    outdatedMobileCDRs.Add(currentMobileCDR);
                    mobileCDRs.Remove(currentMobileCDR);

                    List<MobileCDR> tempCorrelatedCDRs = mobileCDRsByCDPN.GetOrCreateItem(currentMobileCDR.CDPN);
                    if (tempCorrelatedCDRs.Count > 1)
                        tempCorrelatedCDRs.Remove(currentMobileCDR);
                    else
                        mobileCDRsByCDPN.Remove(currentMobileCDR.CDPN);

                    index--;
                }
            }

            return outdatedMobileCDRs.Count > 0 ? outdatedMobileCDRs : null;
        }

        private void CheckCallForwardMatching(List<MobileCDR> outdatedMobileCDRs, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict,
            Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict, TimeSpan dateTimeMargin, TimeSpan durationMargin, int originatedCDRRecordType, int terminatedCDRRecordType)
        {
            if (outdatedMobileCDRs == null || outdatedMobileCDRs.Count == 0)
                return;

            foreach (var outdatedMobileCDR in outdatedMobileCDRs)
            {
                List<MobileCDR> callForwardMobileCDRsByCGPN = callForwardMobileCDRsByCGPNDict.GetRecord(outdatedMobileCDR.CGPN);
                if (callForwardMobileCDRsByCGPN != null && callForwardMobileCDRsByCGPN.Count > 0)
                {
                    for (var index = 0; index < callForwardMobileCDRsByCGPN.Count; index++)
                    {
                        var currentCallForwardMobileCDR = callForwardMobileCDRsByCGPN[index];

                        if (IsOriginatedCallForwardMatching(outdatedMobileCDR, currentCallForwardMobileCDR, dateTimeMargin, durationMargin.Seconds, originatedCDRRecordType))
                        {
                            CallForwardMobileCDRs tempCallForwardMobileCDRs = callForwardMobileCDRsDict.GetOrCreateItem(currentCallForwardMobileCDR, () =>
                            {
                                return new CallForwardMobileCDRs() { CallForwardMobileCDR = currentCallForwardMobileCDR };
                            });
                            tempCallForwardMobileCDRs.OriginatedMobileCDR = outdatedMobileCDR;
                            break;
                        }

                        if (IsTerminatedCallForwardMatching(outdatedMobileCDR, currentCallForwardMobileCDR, dateTimeMargin, durationMargin.Seconds, terminatedCDRRecordType))
                        {
                            CallForwardMobileCDRs tempCallForwardMobileCDRs = callForwardMobileCDRsDict.GetOrCreateItem(currentCallForwardMobileCDR, () =>
                            {
                                return new CallForwardMobileCDRs() { CallForwardMobileCDR = currentCallForwardMobileCDR };
                            });
                            tempCallForwardMobileCDRs.TerminatedMobileCDR = outdatedMobileCDR;
                            break;
                        }
                    }
                }
            }
        }

        private void CorrelateAndCleanOutdatedCallForwardCDRs(List<MobileCDR> callForwardMobileCDRList, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict, Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict,
             DateTime maxDateTime, TimeSpan dateTimeMargin, CDRCorrelationBatch cdrCorrelationBatch, CDRCorrelationDefinition cdrCorrelationDefinition)
        {
            DateTime minDateTime = maxDateTime.AddSeconds(-dateTimeMargin.TotalSeconds);
            for (var index = callForwardMobileCDRList.Count - 1; index >= 0; index--)
            {
                MobileCDR currentCallForwardMobileCDR = callForwardMobileCDRList[index];
                if (currentCallForwardMobileCDR.AttemptDateTime >= minDateTime)
                    continue;

                while (index >= 0)
                {
                    currentCallForwardMobileCDR = callForwardMobileCDRList[index];

                    CorrelateCallForwardCDR(currentCallForwardMobileCDR, callForwardMobileCDRsDict, cdrCorrelationBatch, cdrCorrelationDefinition);

                    callForwardMobileCDRList.Remove(currentCallForwardMobileCDR);
                    callForwardMobileCDRsDict.Remove(currentCallForwardMobileCDR);

                    List<MobileCDR> tempCorrelatedCDRs = callForwardMobileCDRsByCGPNDict.GetOrCreateItem(currentCallForwardMobileCDR.CGPN);
                    if (tempCorrelatedCDRs.Count > 1)
                        tempCorrelatedCDRs.Remove(currentCallForwardMobileCDR);
                    else
                        callForwardMobileCDRsByCGPNDict.Remove(currentCallForwardMobileCDR.CDPN);

                    index--;
                }
            }
        }

        private void CorrelateCallForwardCDR(MobileCDR callForwardMobileCDR, Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict,
            CDRCorrelationBatch cdrCorrelationBatch, CDRCorrelationDefinition cdrCorrelationDefinition)
        {
            CallForwardMobileCDRs callForwardMobileCDRs;
            if (callForwardMobileCDRsDict.TryGetValue(callForwardMobileCDR, out callForwardMobileCDRs) && callForwardMobileCDRs.CanBeCorrelated)
            {
                //A => B (0 and 100)
                var correlateSingleCDRDataTransformationOutput = new DataTransformer().ExecuteDataTransformation(cdrCorrelationDefinition.Settings.CorrelateSingleCDRDataTransformationDefinitionId, (context) =>
                {
                    context.SetRecordValue("InputMobileCDRs", new List<dynamic>() { callForwardMobileCDRs.OriginatedMobileCDR.CDR, callForwardMobileCDRs.CallForwardMobileCDR.CDR });
                });
                List<dynamic> aToB_CorrelatedCDRs = correlateSingleCDRDataTransformationOutput.GetRecordValue("OutputCorrelatedCDRs");
                AddCorrelatedCDR(cdrCorrelationBatch, aToB_CorrelatedCDRs, new List<MobileCDR> { callForwardMobileCDRs.OriginatedMobileCDR, callForwardMobileCDRs.CallForwardMobileCDR });

                //A => C (0, 1 and 100)
                var mergeDataTransformationOutput = new DataTransformer().ExecuteDataTransformation(cdrCorrelationDefinition.Settings.MergeDataTransformationDefinitionId, (context) =>
                {
                    List<dynamic> inputList = new List<dynamic> { callForwardMobileCDRs.OriginatedMobileCDR.CDR, callForwardMobileCDRs.CallForwardMobileCDR.CDR, callForwardMobileCDRs.TerminatedMobileCDR.CDR };
                    context.SetRecordValue("InputMobileCDRs", inputList);
                });
                List<dynamic> aToC_CorrelatedCDRs = mergeDataTransformationOutput.GetRecordValue("OutputCorrelatedCDRs");
                List<MobileCDR> mobileCDRsToDelete = new List<MobileCDR>() { callForwardMobileCDRs.OriginatedMobileCDR, callForwardMobileCDRs.CallForwardMobileCDR, callForwardMobileCDRs.TerminatedMobileCDR };
                AddCorrelatedCDR(cdrCorrelationBatch, aToC_CorrelatedCDRs, mobileCDRsToDelete);
            }
        }

        private void FinalizingCDRCorrelation(List<MobileCDR> uncorrelatedMobileCDRList, List<MobileCDR> callForwardMobileCDRList, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict,
            Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict, CorrelateCDRsInput inputArgument, AsyncActivityHandle handle, CDRCorrelationDefinition cdrCorrelationDefinition,
            TimeSpan dateTimeMargin, TimeSpan durationMargin, int originatedCDRRecordType, int terminatedCDRRecordType)
        {
            DateTime finalizingCDRCorrelationStartTime = DateTime.Now;
            CDRCorrelationBatch finalizingCDRCorrelationBatch = new CDRCorrelationBatch();

            CheckCallForwardMatching(uncorrelatedMobileCDRList, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsDict, dateTimeMargin, durationMargin, originatedCDRRecordType, terminatedCDRRecordType);

            foreach (var callForwardMobileCDR in callForwardMobileCDRList)
                CorrelateCallForwardCDR(callForwardMobileCDR, callForwardMobileCDRsDict, finalizingCDRCorrelationBatch, cdrCorrelationDefinition);

            if (finalizingCDRCorrelationBatch.OutputRecordsToInsert.Count > 0)
                inputArgument.OutputQueue.Enqueue(finalizingCDRCorrelationBatch);

            double finalizingCDRCorrelationElapsedTime = Math.Round((DateTime.Now - finalizingCDRCorrelationStartTime).TotalSeconds);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finalizing Correlate CDRs. {0} CDRs correlated. ElapsedTime: {1} (s)",
                finalizingCDRCorrelationBatch.OutputRecordsToInsert.Count, finalizingCDRCorrelationElapsedTime.ToString());
        }

        private void AddCorrelatedCDR(CDRCorrelationBatch cdrCorrelationBatch, List<dynamic> correlatedCDRs, List<MobileCDR> mobileCDRsToDelete)
        {
            cdrCorrelationBatch.OutputRecordsToInsert.AddRange(correlatedCDRs);

            foreach (var mobileCDRToDelete in mobileCDRsToDelete)
            {
                cdrCorrelationBatch.InputIdsToDelete.Add(mobileCDRToDelete.MobileCDRId);

                DateTime attemptDateTime = mobileCDRToDelete.AttemptDateTime;

                DateTime from = cdrCorrelationBatch.DateTimeRange.From;
                if (from == default(DateTime) || from > attemptDateTime)
                    cdrCorrelationBatch.DateTimeRange.From = attemptDateTime;

                DateTime to = cdrCorrelationBatch.DateTimeRange.To;
                if (to == default(DateTime) || to <= attemptDateTime)
                    cdrCorrelationBatch.DateTimeRange.To = attemptDateTime.AddSeconds(1);
            }
        }

        private bool IsNormalCallMatching(MobileCDR firstMobileCDR, MobileCDR secondMobileCDR, TimeSpan dateTimeMargin, int durationMarginInSeconds)
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

        private bool IsOriginatedCallForwardMatching(MobileCDR mobileCDR, MobileCDR callForwardMobileCDR, TimeSpan dateTimeMargin, int durationMarginInSeconds, int originatedCDRRecordType)
        {
            if (mobileCDR.RecordType != originatedCDRRecordType)
                return false;

            if ((mobileCDR.AttemptDateTime - callForwardMobileCDR.AttemptDateTime) > dateTimeMargin)
                return false;

            if (Math.Abs(mobileCDR.Duration - callForwardMobileCDR.Duration) > durationMarginInSeconds)
                return false;

            //if (string.Compare(firstMobileCDR.CGPN, secondMobileCDR.CGPN) != 0)
            //    return false;

            //if (string.Compare(mobileCDR.CDPN, callForwardMobileCDR.CDPN) != 0)
            //    return false;

            return true;
        }

        private bool IsTerminatedCallForwardMatching(MobileCDR mobileCDR, MobileCDR callForwardMobileCDR, TimeSpan dateTimeMargin, int durationMarginInSeconds, int terminatedCDRRecordType)
        {
            if (mobileCDR.RecordType != terminatedCDRRecordType)
                return false;

            if ((mobileCDR.AttemptDateTime - callForwardMobileCDR.AttemptDateTime) > dateTimeMargin)
                return false;

            if (Math.Abs(mobileCDR.Duration - callForwardMobileCDR.Duration) > durationMarginInSeconds)
                return false;

            //if (string.Compare(firstMobileCDR.CGPN, secondMobileCDR.CGPN) != 0)
            //    return false;

            if (string.Compare(mobileCDR.CDPN, callForwardMobileCDR.CDPN) != 0)
                return false;

            return true;
        }
    }

    public class MobileCDR
    {
        public long MobileCDRId { get; set; }

        public int RecordType { get; set; }

        public DateTime AttemptDateTime { get; set; }

        public int Duration { get; set; }

        public string CGPN { get; set; }

        public string CDPN { get; set; }

        public dynamic CDR { get; set; }

        public MobileCDR(dynamic cdr, CDRCorrelationDefinition cdrCorrelationDefinition)
        {
            MobileCDRId = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.IdFieldName);
            RecordType = cdr.GetFieldValue("RecordType");
            AttemptDateTime = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.DatetimeFieldName);
            Duration = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.DurationFieldName);
            CGPN = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.CallingNumberFieldName);
            CDPN = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.CalledNumberFieldName);
            CDR = cdr;
        }
    }

    public class CallForwardMobileCDRs
    {
        public MobileCDR CallForwardMobileCDR { get; set; }

        public MobileCDR OriginatedMobileCDR { get; set; }

        public MobileCDR TerminatedMobileCDR { get; set; }

        public bool CanBeCorrelated
        {
            get
            {
                return (this.CallForwardMobileCDR != null && this.OriginatedMobileCDR != null && this.TerminatedMobileCDR != null) ? true : false;
            }
        }
    }
}