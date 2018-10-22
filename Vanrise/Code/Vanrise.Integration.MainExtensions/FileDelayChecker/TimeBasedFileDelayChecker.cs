using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileDelayChecker
{
    public class TimeBasedFileDelayChecker : FileDelayCheckerSettings
    {
        public override Guid ConfigId { get { throw new NotImplementedException(); } }

        public TimeSpan MaxDelayPeriod { get; set; }

        public override bool IsDelayed(IFileDelayCheckerIsDelayedContext context)
        {
            if (!context.LastRetrievedFileTime.HasValue)
                return false;

            return DateTime.Now - context.LastRetrievedFileTime.Value > this.MaxDelayPeriod;
        }
    }
}
