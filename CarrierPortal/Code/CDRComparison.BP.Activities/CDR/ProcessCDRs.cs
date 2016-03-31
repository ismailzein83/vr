using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Queueing;

namespace CDRComparison.BP.Activities
{
    #region Arguments Classes
    public class ProcessCDRsInput
    {
        public TimeSpan TimeOffset { get; set; }
        public long TimeMarginInMilliSeconds { get; set; }
        public long DurationMarginInMilliSeconds { get; set; }
        public BaseQueue<CDRBatch> InputQueue { get; set; }
        public BaseQueue<MissingCDRBatch> OutputQueueMissingCDR { get; set; }
        public BaseQueue<PartialMatchCDRBatch> OutputQueuePartialMatchCDR { get; set; }
        public BaseQueue<DisputeCDRBatch> OutputQueueDisputeCDR { get; set; }

    }

    #endregion
    public class ProcessCDRs : DependentAsyncActivity<ProcessCDRsInput>
    {
        #region Arguments
        [RequiredArgument]
        public InOutArgument<TimeSpan> TimeOffset { get; set; }
        [RequiredArgument]
        public InOutArgument<long> TimeMarginInMilliSeconds { get; set; }
        [RequiredArgument]
        public InOutArgument<long> DurationMarginInMilliSeconds { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<MissingCDRBatch>> OutputQueueMissingCDR { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<PartialMatchCDRBatch>> OutputQueuePartialMatchCDR { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<DisputeCDRBatch>> OutputQueueDisputeCDR { get; set; }
        #endregion
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueueMissingCDR.Get(context) == null)
                this.OutputQueueMissingCDR.Set(context, new MemoryQueue<MissingCDRBatch>());
            if (this.OutputQueuePartialMatchCDR.Get(context) == null)
                this.OutputQueuePartialMatchCDR.Set(context, new MemoryQueue<PartialMatchCDRBatch>());
            if (this.OutputQueueDisputeCDR.Get(context) == null)
                this.OutputQueueDisputeCDR.Set(context, new MemoryQueue<DisputeCDRBatch>());

            base.OnBeforeExecute(context, handle);
        }
        protected override void DoWork(ProcessCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            string currentCDPN = null;
            CDRBatch batch = new CDRBatch();
            batch.CDRs = new List<CDR>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (cdrBatch) =>
                        {
                            var cdrs = cdrBatch.CDRs;
                            foreach (var cdr in cdrs)
                            {
                                if (currentCDPN == null)
                                    currentCDPN = cdr.CDPN;

                                if (cdr.CDPN == currentCDPN)
                                    batch.CDRs.Add(cdr);
                                else
                                {
                                    ProcessCDRBach(inputArgument, batch);
                                    currentCDPN = cdr.CDPN;
                                    batch.CDRs = new List<CDR>();
                                    batch.CDRs.Add(cdr);
                                }
                            }


                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            if (batch.CDRs.Count() > 0)
            {
                ProcessCDRBach(inputArgument, batch);
            }

        }
        private void ProcessCDRBach(ProcessCDRsInput inputArgument, CDRBatch cdrBatch)
        {

            if (cdrBatch != null)
            {
                SetTimeOffsetConfiguration(inputArgument.TimeOffset, cdrBatch);
                var systemCDRs = cdrBatch.CDRs.Where(x => !x.IsPartnerCDR);
                var partnerCDRs = cdrBatch.CDRs.Where(x => x.IsPartnerCDR);

                if (systemCDRs == null || systemCDRs.Count() == 0 || partnerCDRs == null || partnerCDRs.Count() == 0)
                {
                    inputArgument.OutputQueueMissingCDR.Enqueue(MapToMissingCDRBatch(cdrBatch));
                }
                else if (systemCDRs.Count() != partnerCDRs.Count())
                {
                    inputArgument.OutputQueuePartialMatchCDR.Enqueue(MapToPartialMatchCDRBatch(systemCDRs, partnerCDRs));
                }
                else
                {
                    foreach (var cdr in partnerCDRs)
                    {
                        var systemCDR = systemCDRs.FirstOrDefault(x => x.CDPN == cdr.CDPN && x.CGPN == cdr.CGPN);
                        decimal duration = cdr.DurationInSec - systemCDR.DurationInSec;
                        TimeSpan timeDifference = cdr.Time - systemCDR.Time;
                        if (Math.Abs(duration) > inputArgument.DurationMarginInMilliSeconds || Math.Abs(timeDifference.TotalMilliseconds) > inputArgument.TimeMarginInMilliSeconds)
                        {
                            inputArgument.OutputQueueDisputeCDR.Enqueue(MapToDisputeCDRBatch(systemCDRs, partnerCDRs));
                            break;
                        }
                    }
                }

            }
        }
        private void SetTimeOffsetConfiguration(TimeSpan timeOffset, CDRBatch cdrBatch)
        {
            foreach (var cdr in cdrBatch.CDRs)
            {
                cdr.Time = cdr.Time + timeOffset;
            }
        }
        private MissingCDRBatch MapToMissingCDRBatch(CDRBatch cdrBatch)
        {
            List<MissingCDR> missingCDRs = new List<MissingCDR>();
            foreach (var cdr in cdrBatch.CDRs)
            {
                missingCDRs.Add(new MissingCDR
                {
                    OriginalCDPN = cdr.OriginalCDPN,
                    OriginalCGPN = cdr.OriginalCGPN,
                    CDPN = cdr.CDPN,
                    CGPN = cdr.CGPN,
                    IsPartnerCDR = cdr.IsPartnerCDR,
                    DurationInSec = cdr.DurationInSec,
                    Time = cdr.Time
                });
            }
            return new MissingCDRBatch
            {
                MissingCDRs = missingCDRs
            };
        }
        private PartialMatchCDRBatch MapToPartialMatchCDRBatch(IEnumerable<CDR> systemCDRs, IEnumerable<CDR> partnerCDRs)
        {
            List<PartialMatchCDR> partialMatchCDR = new List<PartialMatchCDR>();
            foreach (var cdr in systemCDRs)
            {
                var partnerCDR = partnerCDRs.FirstOrDefault(x => x.CDPN == cdr.CDPN && x.CGPN == cdr.CGPN && x.Time == cdr.Time);
                partialMatchCDR.Add(new PartialMatchCDR
                {
                    OriginalSystemCDPN = cdr.OriginalCDPN,
                    OriginalSystemCGPN = cdr.OriginalCGPN,
                    SystemCDPN = cdr.CDPN,
                    SystemCGPN = cdr.CGPN,
                    SystemTime = cdr.Time,
                    SystemDurationInSec = cdr.DurationInSec,
                    OriginalPartnerCDPN = partnerCDR != null ? partnerCDR.OriginalCDPN : null,
                    OriginalPartnerCGPN = partnerCDR != null ? partnerCDR.OriginalCGPN : null,
                    PartnerCDPN = partnerCDR != null ? partnerCDR.CDPN : null,
                    PartnerCGPN = partnerCDR != null ? partnerCDR.CGPN : null,
                    PartnerTime = partnerCDR != null ? partnerCDR.Time : default(DateTime),
                    PartnerDurationInSec = partnerCDR != null ? partnerCDR.DurationInSec : default(decimal)
                });
            }
            foreach (var cdr in partnerCDRs)
            {
                var systemCDR = systemCDRs.FirstOrDefault(x => x.CDPN == cdr.CDPN && x.CGPN == cdr.CGPN && x.Time == cdr.Time);
                if (systemCDR == null)
                {
                    partialMatchCDR.Add(new PartialMatchCDR
                    {
                        OriginalSystemCDPN = null,
                        OriginalSystemCGPN = null,
                        SystemCDPN = null,
                        SystemCGPN = null,
                        SystemTime = default(DateTime),
                        SystemDurationInSec = default(decimal),
                        OriginalPartnerCDPN = cdr.OriginalCDPN,
                        OriginalPartnerCGPN = cdr.OriginalCGPN,
                        PartnerCDPN = cdr.CDPN,
                        PartnerCGPN = cdr.CGPN,
                        PartnerTime = cdr.Time,
                        PartnerDurationInSec = cdr.DurationInSec
                    });
                }
            }

            return new PartialMatchCDRBatch
            {
                PartialMatchCDRs = partialMatchCDR
            };
        }

        private DisputeCDRBatch MapToDisputeCDRBatch(IEnumerable<CDR> systemCDRs, IEnumerable<CDR> partnerCDRs)
        {
            List<DisputeCDR> disputeCDR = new List<DisputeCDR>();
            foreach (var cdr in systemCDRs)
            {
                var partnerCDR = partnerCDRs.FirstOrDefault(x => x.CDPN == cdr.CDPN && x.CGPN == cdr.CGPN && x.Time == cdr.Time);
                if (partnerCDR == null)
                    throw new NullReferenceException("MapToDisputeCDRBatch");
                disputeCDR.Add(new DisputeCDR
                {
                    OriginalSystemCDPN = cdr.OriginalCDPN,
                    OriginalSystemCGPN = cdr.OriginalCGPN,
                    SystemCDPN = cdr.CDPN,
                    SystemCGPN = cdr.CGPN,
                    SystemTime = cdr.Time,
                    SystemDurationInSec = cdr.DurationInSec,
                    OriginalPartnerCDPN = partnerCDR.OriginalCDPN,
                    OriginalPartnerCGPN = partnerCDR.OriginalCGPN,
                    PartnerCDPN = partnerCDR.CDPN,
                    PartnerCGPN = partnerCDR.CGPN,
                    PartnerTime = partnerCDR.Time,
                    PartnerDurationInSec = partnerCDR.DurationInSec
                });
            }
            return new DisputeCDRBatch
            {
                DisputeCDRs = disputeCDR
            };
        }

        protected override ProcessCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueueMissingCDR = this.OutputQueueMissingCDR.Get(context),
                OutputQueuePartialMatchCDR = this.OutputQueuePartialMatchCDR.Get(context),
                OutputQueueDisputeCDR = this.OutputQueueDisputeCDR.Get(context),
                TimeOffset = this.TimeOffset.Get(context),
                DurationMarginInMilliSeconds = this.DurationMarginInMilliSeconds.Get(context),
                TimeMarginInMilliSeconds = this.TimeMarginInMilliSeconds.Get(context),
            };
        }
    }
}
