using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileDelayChecker
{
    public class SizeBasedFileDelayChecker : FileDelayCheckerSettings
    {
        public override Guid ConfigId { get { throw new NotImplementedException(); } }

        public TimeSpan MaxPeakDelayPeriod { get; set; }

        public TimeSpan MaxOffPeakDelayPeriod { get; set; }

        public override bool IsDelayed(IFileDelayCheckerIsDelayedContext context)
        {
            if (!context.LastRetrievedFileTime.HasValue)
                return false;

            DateTime now = DateTime.Now;
            List<PeakDateTimeRange> peakDateTimeRanges = new List<PeakDateTimeRange>(); //GetDataFromConfigManager

            if (IsPeak(now, peakDateTimeRanges))
                return now - context.LastRetrievedFileTime.Value > this.MaxPeakDelayPeriod;
            else
                return now - context.LastRetrievedFileTime.Value > this.MaxOffPeakDelayPeriod;
        }

        public bool IsPeak(DateTime date, List<PeakDateTimeRange> peakDateTimeRanges)
        {
            PeakDateTimeRange peakDateTimeRange = peakDateTimeRanges.First(itm => itm.From >= date && itm.To < date);
            return peakDateTimeRange != null;
        }
    }
}