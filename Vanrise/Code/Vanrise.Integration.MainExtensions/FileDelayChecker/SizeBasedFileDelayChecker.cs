using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileDelayChecker
{
    public class SizeBasedFileDelayChecker : Vanrise.Integration.Entities.FileDelayChecker
    {
        public override Guid ConfigId { get { return new Guid("07C4303E-2D53-4062-BEF6-9B033CABB692"); } }

        public TimeSpan PeakDelayInterval { get; set; }

        public TimeSpan OffPeakDelayInterval { get; set; }

        public override bool IsDelayed(IFileDelayCheckerIsDelayedContext context)
        {
            if (!context.LastRetrievedFileTime.HasValue)
                return false;

            DateTime nowDateTime = DateTime.Now;
            Time currentTime = new Time(nowDateTime);
            List<PeakTimeRange> peakTimeRanges = new ConfigManager().GetPeakTimeRanges();

            if (IsPeak(currentTime, peakTimeRanges))
                return nowDateTime - context.LastRetrievedFileTime.Value > this.PeakDelayInterval;
            else
                return nowDateTime - context.LastRetrievedFileTime.Value > this.OffPeakDelayInterval;
        }

        private bool IsPeak(Time currentTime, List<PeakTimeRange> peakTimeRanges)
        {
            if (peakTimeRanges == null || peakTimeRanges.Count == 0)
                return false;

            PeakTimeRange peakDateTimeRange = peakTimeRanges.FirstOrDefault(itm => itm.From.LessThanOrEqual(currentTime) && itm.To.GreaterThanOrEqual(currentTime));
            return peakDateTimeRange != null;
        }
    }
}