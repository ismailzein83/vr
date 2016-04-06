﻿using CDRComparison.Entities;
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

                MissingCDRBatch missingCDRBatch = new Entities.MissingCDRBatch();
                missingCDRBatch.MissingCDRs = new List<MissingCDR>();
                PartialMatchCDRBatch partialMatchCDRBatch = new Entities.PartialMatchCDRBatch();
                partialMatchCDRBatch.PartialMatchCDRs = new List<PartialMatchCDR>();
                DisputeCDRBatch disputeCDRBatch = new Entities.DisputeCDRBatch();
                disputeCDRBatch.DisputeCDRs = new List<DisputeCDR>();
                var systemCDRs = cdrBatch.CDRs.Where(x => !x.IsPartnerCDR);
                var partnerCDRs = cdrBatch.CDRs.Where(x => x.IsPartnerCDR).ToList();
                    foreach (var cdr in systemCDRs)
                    {
                        var partnerCDR = partnerCDRs.FindAllRecords(x => x.CGPN == cdr.CGPN && Math.Abs((cdr.Time - x.Time).TotalMilliseconds) <= inputArgument.TimeMarginInMilliSeconds);
                        if (partnerCDR == null || partnerCDR.Count() == 0)
                        {
                            missingCDRBatch.MissingCDRs.Add(new MissingCDR
                            {
                                CDPN = cdr.CDPN,
                                Time = cdr.Time,
                                DurationInSec = cdr.DurationInSec,
                                CGPN = cdr.CGPN,
                                OriginalCDPN = cdr.OriginalCDPN,
                                IsPartnerCDR = cdr.IsPartnerCDR,
                                OriginalCGPN = cdr.OriginalCGPN,
                            });
                        }
                        else
                        {
                            var partner = partnerCDR.OrderBy(x => Math.Abs((x.Time - cdr.Time).TotalMilliseconds)).First();
                            partnerCDRs.Remove(partner);
                            if (Math.Abs(partner.DurationInSec - cdr.DurationInSec) <= inputArgument.DurationMarginInMilliSeconds)
                            {
                                disputeCDRBatch.DisputeCDRs.Add(new DisputeCDR
                                {
                                    SystemCDPN = cdr.CDPN,
                                    SystemTime = cdr.Time,
                                    SystemDurationInSec = cdr.DurationInSec,
                                    SystemCGPN = cdr.CGPN,
                                    PartnerTime = partner.Time,
                                    OriginalPartnerCDPN = partner.OriginalCDPN,
                                    OriginalSystemCDPN = cdr.OriginalCDPN,
                                    PartnerCDPN = partner.CDPN,
                                    PartnerDurationInSec = partner.DurationInSec,
                                    OriginalPartnerCGPN = partner.OriginalCGPN,
                                    OriginalSystemCGPN = cdr.OriginalCGPN,
                                    PartnerCGPN = partner.CGPN
                                });
                            }
                            else
                            {
                                partialMatchCDRBatch.PartialMatchCDRs.Add(new PartialMatchCDR
                                {
                                    SystemCDPN = cdr.CDPN,
                                    SystemTime = cdr.Time,
                                    SystemDurationInSec = cdr.DurationInSec,
                                    SystemCGPN = cdr.CGPN,
                                    PartnerTime = partner.Time,
                                    OriginalPartnerCDPN = partner.OriginalCDPN,
                                    OriginalSystemCDPN = cdr.OriginalCDPN,
                                    PartnerCDPN = partner.CDPN,
                                    PartnerDurationInSec = partner.DurationInSec,
                                    OriginalPartnerCGPN = partner.OriginalCGPN,
                                    OriginalSystemCGPN = cdr.OriginalCGPN,
                                    PartnerCGPN = partner.CGPN

                                });
                            }
                        }
                    }
                    if(partnerCDRs.Count() >0)
                    {
                        foreach (var partner in partnerCDRs)
                        {
                            missingCDRBatch.MissingCDRs.Add(new MissingCDR
                            {
                                Time = partner.Time,
                                OriginalCDPN = partner.OriginalCDPN,
                                CDPN = partner.CDPN,
                                DurationInSec = partner.DurationInSec,
                                OriginalCGPN = partner.OriginalCGPN,
                                CGPN = partner.CGPN,
                                IsPartnerCDR = partner.IsPartnerCDR

                            });
                        }
                    }

                inputArgument.OutputQueueDisputeCDR.Enqueue(disputeCDRBatch);
                inputArgument.OutputQueuePartialMatchCDR.Enqueue(partialMatchCDRBatch);
                inputArgument.OutputQueueMissingCDR.Enqueue(missingCDRBatch);
            }
        }
        private void SetTimeOffsetConfiguration(TimeSpan timeOffset, CDRBatch cdrBatch)
        {
            foreach (var cdr in cdrBatch.CDRs)
            {
                if(cdr.IsPartnerCDR)
                {
                    cdr.Time = cdr.Time + timeOffset;
                }
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
