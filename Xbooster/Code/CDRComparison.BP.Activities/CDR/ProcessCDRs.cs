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
        public BaseQueue<CDRBatch> InputQueue { get; set; }
        public decimal DurationMarginInMilliseconds { get; set; }
        public decimal TimeMarginInMilliseconds { get; set; }
        public TimeSpan TimeOffset { get; set; }
        public BaseQueue<MissingCDRBatch> OutputQueueMissingCDR { get; set; }
        public BaseQueue<PartialMatchCDRBatch> OutputQueuePartialMatchCDR { get; set; }
        public BaseQueue<DisputeCDRBatch> OutputQueueDisputeCDR { get; set; }
        public BaseQueue<InvalidCDRBatch> OutputQueueInvalidCDR { get; set; }
        public bool CompareCGPN { get; set; }
    }

    #endregion

    public class ProcessCDRs : DependentAsyncActivity<ProcessCDRsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<decimal> DurationMarginInMilliseconds { get; set; }
        [RequiredArgument]
        public InArgument<decimal> TimeMarginInMilliseconds { get; set; }
        [RequiredArgument]
        public InArgument<TimeSpan> TimeOffset { get; set; }
        [RequiredArgument]
        public InArgument<bool> CompareCGPN { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<MissingCDRBatch>> OutputQueueMissingCDR { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<PartialMatchCDRBatch>> OutputQueuePartialMatchCDR { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<DisputeCDRBatch>> OutputQueueDisputeCDR { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<InvalidCDRBatch>> OutputQueueInvalidCDR { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueueMissingCDR.Get(context) == null)
                this.OutputQueueMissingCDR.Set(context, new MemoryQueue<MissingCDRBatch>());
            if (this.OutputQueuePartialMatchCDR.Get(context) == null)
                this.OutputQueuePartialMatchCDR.Set(context, new MemoryQueue<PartialMatchCDRBatch>());
            if (this.OutputQueueDisputeCDR.Get(context) == null)
                this.OutputQueueDisputeCDR.Set(context, new MemoryQueue<DisputeCDRBatch>());
            if (this.OutputQueueInvalidCDR.Get(context) == null)
                this.OutputQueueInvalidCDR.Set(context, new MemoryQueue<InvalidCDRBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(ProcessCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            string currentCDPN = null;
            CDRBatch batch = new CDRBatch();
            batch.CDRs = new List<CDR>();
            long totalCount = 0;
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
                                if (cdr.CDPN == currentCDPN)
                                    batch.CDRs.Add(cdr);
                                else
                                {
                                    ProcessCDRBatch(inputArgument, batch);
                                    currentCDPN = cdr.CDPN;
                                    batch.CDRs = new List<CDR>();
                                    batch.CDRs.Add(cdr);
                                }
                            }
                            totalCount += cdrs.Count;
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} CDRs processed", totalCount);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            if (batch.CDRs.Count() > 0)
            {
                ProcessCDRBatch(inputArgument, batch);
            }

            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finished Processing CDRs.", totalCount);
        }

        protected override ProcessCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                DurationMarginInMilliseconds = this.DurationMarginInMilliseconds.Get(context),
                TimeMarginInMilliseconds = this.TimeMarginInMilliseconds.Get(context),
                TimeOffset = this.TimeOffset.Get(context),
                OutputQueueMissingCDR = this.OutputQueueMissingCDR.Get(context),
                OutputQueuePartialMatchCDR = this.OutputQueuePartialMatchCDR.Get(context),
                OutputQueueDisputeCDR = this.OutputQueueDisputeCDR.Get(context),
                OutputQueueInvalidCDR = this.OutputQueueInvalidCDR.Get(context),
                CompareCGPN = this.CompareCGPN.Get(context)
            };
        }

        #region Private Methods

        void ProcessCDRBatch(ProcessCDRsInput inputArgument, CDRBatch cdrBatch)
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
                InvalidCDRBatch invalidCDRBatch = new InvalidCDRBatch() { InvalidCDRs = new List<InvalidCDR>() };

                var systemCDRs = new List<CDR>();
                var partnerCDRs = new List<CDR>();
                SetCDRsToProcess(cdrBatch.CDRs, systemCDRs, partnerCDRs, invalidCDRBatch.InvalidCDRs);

                foreach (var cdr in systemCDRs)
                {
                    var partnerCDR = partnerCDRs.FindAllRecords(x => (inputArgument.CompareCGPN ? x.CGPN == cdr.CGPN : true) && Convert.ToDecimal(Math.Abs((cdr.Time - x.Time).TotalMilliseconds)) <= inputArgument.TimeMarginInMilliseconds);
                    if (partnerCDR == null || partnerCDR.Count() == 0)
                    {
                        missingCDRBatch.MissingCDRs.Add(new MissingCDR
                        {
                            CDPN = cdr.CDPN,
                            Time = cdr.Time,
                            DurationInSec = cdr.DurationInSec,
                            CGPN = cdr.CGPN,
                            OriginalCDPN = cdr.OriginalCDPN,
                            IsPartnerCDR = !cdr.IsPartnerCDR,
                            OriginalCGPN = cdr.OriginalCGPN,
                        });
                    }
                    else
                    {
                        var partner = partnerCDR.OrderBy(x => Math.Abs((x.Time - cdr.Time).TotalMilliseconds)).ThenBy(x => Math.Abs(x.DurationInSec - cdr.DurationInSec)).First();
                        partnerCDRs.Remove(partner);
                        if (Math.Abs((partner.DurationInSec - cdr.DurationInSec) * 1000) <= inputArgument.DurationMarginInMilliseconds)
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
                if (partnerCDRs.Count() > 0)
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
                            IsPartnerCDR = !partner.IsPartnerCDR

                        });
                    }
                }

                inputArgument.OutputQueueDisputeCDR.Enqueue(disputeCDRBatch);
                inputArgument.OutputQueuePartialMatchCDR.Enqueue(partialMatchCDRBatch);
                inputArgument.OutputQueueMissingCDR.Enqueue(missingCDRBatch);
                inputArgument.OutputQueueInvalidCDR.Enqueue(invalidCDRBatch);
            }
        }

        void SetTimeOffsetConfiguration(TimeSpan timeOffset, CDRBatch cdrBatch)
        {
            foreach (var cdr in cdrBatch.CDRs)
            {
                if (cdr.IsPartnerCDR)
                {
                    cdr.Time = cdr.Time + timeOffset;
                }
            }
        }

        private void SetCDRsToProcess(IEnumerable<CDR> cdrs, List<CDR> systemCDRs, List<CDR> partnerCDRs, List<InvalidCDR> invalidCDRs)
        {
            foreach (CDR cdr in cdrs)
            {
                if (string.IsNullOrEmpty(cdr.OriginalCDPN) || string.IsNullOrEmpty(cdr.CDPN))
                {
                    invalidCDRs.Add(new InvalidCDR()
                    {
                        OriginalCDPN = cdr.OriginalCDPN,
                        OriginalCGPN = cdr.OriginalCGPN,
                        CDPN = cdr.CDPN,
                        CGPN = cdr.CGPN,
                        Time = cdr.Time,
                        DurationInSec = cdr.DurationInSec,
                        IsPartnerCDR = cdr.IsPartnerCDR
                    });
                }
                else if (cdr.IsPartnerCDR)
                    partnerCDRs.Add(cdr);
                else
                    systemCDRs.Add(cdr);
            }
        }

        #endregion
    }
}
