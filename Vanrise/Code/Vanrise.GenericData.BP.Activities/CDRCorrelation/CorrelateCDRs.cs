using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation;
using Vanrise.Queueing;
using System.Linq;

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
            cdrCorrelationDefinition.ThrowIfNull("cdrCorrelationDefinition");
            cdrCorrelationDefinition.Settings.ThrowIfNull("cdrCorrelationDefinition.Settings");

            DataRecordType inputDataRecordType = new DataRecordTypeManager().GetDataRecordType(cdrCorrelationDefinition.Settings.InputDataRecordTypeId);
            string idFieldName = inputDataRecordType.Settings.IdField;
            string datetimeFieldName = inputDataRecordType.Settings.DateTimeField;

            int originatedCDRRecordType = 0;
            int terminatedCDRRecordType = 1;
            int callForwardCDRRecordType = 100;

            List<MobileCDR> callForwardMobileCDRList = new List<MobileCDR>();
            Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict = new Dictionary<string, List<MobileCDR>>();
            Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCDPNDict = new Dictionary<string, List<MobileCDR>>();
            Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict = new Dictionary<MobileCDR, CallForwardMobileCDRs>();

            List<MobileCDR> uncorrelatedMobileCDRList = new List<MobileCDR>();
            Dictionary<string, List<MobileCDR>> uncorrelatedMobileCDRsByCDPNDict = new Dictionary<string, List<MobileCDR>>();

            TimeSpan totalElapsedTimeToLog = default(TimeSpan);
            int totalNbOfProcessedCDRsToLog = 0;
            int totalNbOfCorrelatedCDRsToLog = 0;

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
                                    MobileCDR mobileCDR = new MobileCDR(cdr, idFieldName, datetimeFieldName, cdrCorrelationDefinition);
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

                                        List<MobileCDR> tempCallForwardMobileCDRsByCGPN = callForwardMobileCDRsByCGPNDict.GetOrCreateItem(mobileCDR.CGPN);
                                        tempCallForwardMobileCDRsByCGPN.Add(mobileCDR);

                                        List<MobileCDR> tempCallForwardMobileCDRsByCDPN = callForwardMobileCDRsByCDPNDict.GetOrCreateItem(mobileCDR.CDPN);
                                        tempCallForwardMobileCDRsByCDPN.Add(mobileCDR);

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

                                CheckCallForwardMatching(uncorrelatedMobileCDRList, uncorrelatedMobileCDRsByCDPNDict, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsByCDPNDict, callForwardMobileCDRsDict,
                                    dateTimeMargin, durationMargin, originatedCDRRecordType, terminatedCDRRecordType);

                                CorrelateAndCleanCallForwardCDRs(callForwardMobileCDRList, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsByCDPNDict, callForwardMobileCDRsDict,
                                    maxDateTime, dateTimeMargin, cdrCorrelationBatch, cdrCorrelationDefinition);

                                CleanUncorrelatedMobileCDRs(uncorrelatedMobileCDRList, uncorrelatedMobileCDRsByCDPNDict, maxDateTime, dateTimeMargin);

                                if (cdrCorrelationBatch.OutputRecordsToInsert.Count > 0)
                                    inputArgument.OutputQueue.Enqueue(cdrCorrelationBatch);

                                double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                totalElapsedTimeToLog = totalElapsedTimeToLog.Add(DateTime.Now - batchStartTime);
                                totalNbOfProcessedCDRsToLog += recordBatch.Records.Count;
                                totalNbOfCorrelatedCDRsToLog += cdrCorrelationBatch.OutputRecordsToInsert.Count;
                                if (totalNbOfProcessedCDRsToLog >= 100000)
                                {
                                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Process {0} CDRs. {1} CDRs correlated. ElapsedTime: {2}",
                                        totalNbOfProcessedCDRsToLog, totalNbOfCorrelatedCDRsToLog, totalElapsedTimeToLog.ToString(@"hh\:mm\:ss\.fff"));

                                    totalElapsedTimeToLog = default(TimeSpan);
                                    totalNbOfProcessedCDRsToLog = 0;
                                    totalNbOfCorrelatedCDRsToLog = 0;
                                }
                            }
                        });
                } while (!ShouldStop(handle) && hasItems);
            });

            if (totalNbOfProcessedCDRsToLog > 0)
            {
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Process {0} CDRs. {1} CDRs correlated. ElapsedTime: {2}",
                    totalNbOfProcessedCDRsToLog, totalNbOfCorrelatedCDRsToLog, totalElapsedTimeToLog.ToString(@"hh\:mm\:ss\.fff"));
            }

            //Finalizing CDRCorrelation
            FinalizingCDRCorrelation(uncorrelatedMobileCDRList, uncorrelatedMobileCDRsByCDPNDict, callForwardMobileCDRList, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsByCDPNDict,
                callForwardMobileCDRsDict, inputArgument, handle, cdrCorrelationDefinition, dateTimeMargin, durationMargin, originatedCDRRecordType, terminatedCDRRecordType);

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

        private void CheckCallForwardMatching(List<MobileCDR> uncorrelatedMobileCDRList, Dictionary<string, List<MobileCDR>> uncorrelatedMobileCDRsByCDPNDict,
            Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCDPNDict,
            Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict, TimeSpan dateTimeMargin, TimeSpan durationMargin, int originatedCDRRecordType, int terminatedCDRRecordType)
        {
            if (uncorrelatedMobileCDRList == null || uncorrelatedMobileCDRList.Count == 0)
                return;

            List<int> uncorrelatedMobileCDRIndexesToRemove = new List<int>();

            for (int i = 0; i < uncorrelatedMobileCDRList.Count; i++)
            {
                MobileCDR uncorrelatedMobileCDR = uncorrelatedMobileCDRList[i];

                if (uncorrelatedMobileCDR.RecordType == originatedCDRRecordType)
                {
                    List<MobileCDR> callForwardMobileCDRsByCGPN = callForwardMobileCDRsByCGPNDict.GetRecord(uncorrelatedMobileCDR.CGPN);
                    if (callForwardMobileCDRsByCGPN != null && callForwardMobileCDRsByCGPN.Count > 0)
                    {
                        for (var j = 0; j < callForwardMobileCDRsByCGPN.Count; j++)
                        {
                            var currentCallForwardMobileCDR = callForwardMobileCDRsByCGPN[j];

                            if (IsOriginatedCallForwardMatching(uncorrelatedMobileCDR, currentCallForwardMobileCDR, dateTimeMargin, durationMargin.Seconds, originatedCDRRecordType))
                            {
                                CallForwardMobileCDRs tempCallForwardMobileCDRs = callForwardMobileCDRsDict.GetOrCreateItem(currentCallForwardMobileCDR, () =>
                                {
                                    return new CallForwardMobileCDRs() { CallForwardMobileCDR = currentCallForwardMobileCDR };
                                });
                                tempCallForwardMobileCDRs.OriginatedMobileCDR = uncorrelatedMobileCDR;

                                uncorrelatedMobileCDRIndexesToRemove.Add(i);
                                RemoveMobileCDR(uncorrelatedMobileCDR, uncorrelatedMobileCDRsByCDPNDict);
                                break;
                            }
                        }
                    }
                }
                else if (uncorrelatedMobileCDR.RecordType == terminatedCDRRecordType)
                {
                    List<MobileCDR> callForwardMobileCDRsByCDPN = callForwardMobileCDRsByCDPNDict.GetRecord(uncorrelatedMobileCDR.CDPN);
                    if (callForwardMobileCDRsByCDPN != null && callForwardMobileCDRsByCDPN.Count > 0)
                    {
                        for (var k = 0; k < callForwardMobileCDRsByCDPN.Count; k++)
                        {
                            var currentCallForwardMobileCDR = callForwardMobileCDRsByCDPN[k];

                            if (IsTerminatedCallForwardMatching(uncorrelatedMobileCDR, currentCallForwardMobileCDR, dateTimeMargin, durationMargin.Seconds, terminatedCDRRecordType))
                            {
                                CallForwardMobileCDRs tempCallForwardMobileCDRs = callForwardMobileCDRsDict.GetOrCreateItem(currentCallForwardMobileCDR, () =>
                                {
                                    return new CallForwardMobileCDRs() { CallForwardMobileCDR = currentCallForwardMobileCDR };
                                });
                                tempCallForwardMobileCDRs.TerminatedMobileCDR = uncorrelatedMobileCDR;

                                uncorrelatedMobileCDRIndexesToRemove.Add(i);
                                RemoveMobileCDR(uncorrelatedMobileCDR, uncorrelatedMobileCDRsByCDPNDict);
                                break;
                            }
                        }
                    }
                }
            }

            foreach(var indexToRemove in uncorrelatedMobileCDRIndexesToRemove.OrderByDescending(itm => itm))
                uncorrelatedMobileCDRList.RemoveAt(indexToRemove);
        }

        private void CorrelateAndCleanCallForwardCDRs(List<MobileCDR> callForwardMobileCDRList, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict,
            Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCDPNDict, Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict,
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
                    CorrelateCallForwardMobileCDR(currentCallForwardMobileCDR, callForwardMobileCDRsDict, cdrCorrelationBatch, cdrCorrelationDefinition);
                    RemoveCallForwardMobileCDR(currentCallForwardMobileCDR, callForwardMobileCDRList, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsByCDPNDict, callForwardMobileCDRsDict);
                    index--;
                }
            }
        }

        private void CorrelateCallForwardMobileCDR(MobileCDR callForwardMobileCDR, Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict,
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

        private void CleanUncorrelatedMobileCDRs(List<MobileCDR> uncorrelatedMobileCDRList, Dictionary<string, List<MobileCDR>> uncorrelatedMobileCDRsByCDPNDict, DateTime maxDateTime, TimeSpan dateTimeMargin)
        {
            DateTime minDateTime = maxDateTime.AddSeconds(-dateTimeMargin.TotalSeconds);
            for (var index = uncorrelatedMobileCDRList.Count - 1; index >= 0; index--)
            {
                MobileCDR currentMobileCDR = uncorrelatedMobileCDRList[index];
                if (currentMobileCDR.AttemptDateTime >= minDateTime)
                    continue;

                while (index >= 0)
                {
                    currentMobileCDR = uncorrelatedMobileCDRList[index];
                    RemoveMobileCDR(currentMobileCDR, uncorrelatedMobileCDRList, uncorrelatedMobileCDRsByCDPNDict);
                    index--;
                }
            }
        }

        private void FinalizingCDRCorrelation(List<MobileCDR> uncorrelatedMobileCDRList, Dictionary<string, List<MobileCDR>> uncorrelatedMobileCDRsByCDPNDict,
            List<MobileCDR> callForwardMobileCDRList, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCDPNDict,
            Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict, CorrelateCDRsInput inputArgument, AsyncActivityHandle handle, CDRCorrelationDefinition cdrCorrelationDefinition,
            TimeSpan dateTimeMargin, TimeSpan durationMargin, int originatedCDRRecordType, int terminatedCDRRecordType)
        {
            DateTime finalizingCDRCorrelationStartTime = DateTime.Now;
            CDRCorrelationBatch finalizingCDRCorrelationBatch = new CDRCorrelationBatch();

            CheckCallForwardMatching(uncorrelatedMobileCDRList, uncorrelatedMobileCDRsByCDPNDict, callForwardMobileCDRsByCGPNDict, callForwardMobileCDRsByCDPNDict, callForwardMobileCDRsDict,
                dateTimeMargin, durationMargin, originatedCDRRecordType, terminatedCDRRecordType);

            foreach (var callForwardMobileCDR in callForwardMobileCDRList)
                CorrelateCallForwardMobileCDR(callForwardMobileCDR, callForwardMobileCDRsDict, finalizingCDRCorrelationBatch, cdrCorrelationDefinition);

            if (finalizingCDRCorrelationBatch.OutputRecordsToInsert.Count > 0)
                inputArgument.OutputQueue.Enqueue(finalizingCDRCorrelationBatch);

            TimeSpan elapsedTime = DateTime.Now - finalizingCDRCorrelationStartTime;
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finalizing Correlate CDRs. {0} CDRs correlated. ElapsedTime: {1}",
                finalizingCDRCorrelationBatch.OutputRecordsToInsert.Count, elapsedTime.ToString(@"hh\:mm\:ss\.fff"));
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

        private void RemoveMobileCDR(MobileCDR mobileCDRToRemove, List<MobileCDR> uncorrelatedMobileCDRList, Dictionary<string, List<MobileCDR>> uncorrelatedMobileCDRsByCDPNDict)
        {
            uncorrelatedMobileCDRList.Remove(mobileCDRToRemove);
            RemoveMobileCDR(mobileCDRToRemove, uncorrelatedMobileCDRsByCDPNDict);
        }

        private void RemoveMobileCDR(MobileCDR mobileCDRToRemove, Dictionary<string, List<MobileCDR>> uncorrelatedMobileCDRsByCDPNDict)
        {
            List<MobileCDR> tempCorrelatedCDRs = uncorrelatedMobileCDRsByCDPNDict.GetOrCreateItem(mobileCDRToRemove.CDPN);
            if (tempCorrelatedCDRs.Count > 1)
                tempCorrelatedCDRs.Remove(mobileCDRToRemove);
            else
                uncorrelatedMobileCDRsByCDPNDict.Remove(mobileCDRToRemove.CDPN);
        }

        private void RemoveCallForwardMobileCDR(MobileCDR callForwardMobileCDRToRemove, List<MobileCDR> callForwardMobileCDRList, Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCGPNDict,
            Dictionary<string, List<MobileCDR>> callForwardMobileCDRsByCDPNDict, Dictionary<MobileCDR, CallForwardMobileCDRs> callForwardMobileCDRsDict)
        {
            callForwardMobileCDRList.Remove(callForwardMobileCDRToRemove);
            callForwardMobileCDRsDict.Remove(callForwardMobileCDRToRemove);

            List<MobileCDR> tempCorrelatedCDRsByCGPN = callForwardMobileCDRsByCGPNDict.GetOrCreateItem(callForwardMobileCDRToRemove.CGPN);
            if (tempCorrelatedCDRsByCGPN.Count > 1)
                tempCorrelatedCDRsByCGPN.Remove(callForwardMobileCDRToRemove);
            else
                callForwardMobileCDRsByCGPNDict.Remove(callForwardMobileCDRToRemove.CGPN);

            List<MobileCDR> tempCorrelatedCDRsByCDPN = callForwardMobileCDRsByCDPNDict.GetOrCreateItem(callForwardMobileCDRToRemove.CDPN);
            if (tempCorrelatedCDRsByCDPN.Count > 1)
                tempCorrelatedCDRsByCDPN.Remove(callForwardMobileCDRToRemove);
            else
                callForwardMobileCDRsByCDPNDict.Remove(callForwardMobileCDRToRemove.CDPN);
        }

        private bool IsNormalCallMatching(MobileCDR firstMobileCDR, MobileCDR secondMobileCDR, TimeSpan dateTimeMargin, int durationMarginInSeconds)
        {
            if (firstMobileCDR.RecordType == secondMobileCDR.RecordType)
                return false;

            if ((firstMobileCDR.AttemptDateTime - secondMobileCDR.AttemptDateTime).Duration() > dateTimeMargin.Duration())
                return false;

            if (Math.Abs(firstMobileCDR.Duration - secondMobileCDR.Duration) > durationMarginInSeconds)
                return false;

            if (string.Compare(firstMobileCDR.CGPN, secondMobileCDR.CGPN) != 0)
                return false;

            //if (string.Compare(firstCorrelatedCDR.CDPN, secondCorrelatedCDR.CDPN) != 0)
            //    return false;

            return true;
        }

        private bool IsOriginatedCallForwardMatching(MobileCDR mobileCDR, MobileCDR callForwardMobileCDR, TimeSpan dateTimeMargin, int durationMarginInSeconds, int originatedCDRRecordType)
        {
            //if (mobileCDR.RecordType != originatedCDRRecordType)
            //    return false;

            if ((mobileCDR.AttemptDateTime - callForwardMobileCDR.AttemptDateTime).Duration() > dateTimeMargin.Duration())
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
            //if (mobileCDR.RecordType != terminatedCDRRecordType)
            //    return false;

            if ((mobileCDR.AttemptDateTime - callForwardMobileCDR.AttemptDateTime).Duration() > dateTimeMargin.Duration())
                return false;

            if (Math.Abs(mobileCDR.Duration - callForwardMobileCDR.Duration) > durationMarginInSeconds)
                return false;

            //if (string.Compare(firstMobileCDR.CGPN, secondMobileCDR.CGPN) != 0)
            //    return false;

            //if (string.Compare(mobileCDR.CDPN, callForwardMobileCDR.CDPN) != 0)
            //    return false;

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

        public MobileCDR(dynamic cdr, string idFieldName, string datetimeFieldName, CDRCorrelationDefinition cdrCorrelationDefinition)
        {
            MobileCDRId = cdr.GetFieldValue(idFieldName);
            RecordType = cdr.GetFieldValue("RecordType");
            AttemptDateTime = cdr.GetFieldValue(datetimeFieldName);
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