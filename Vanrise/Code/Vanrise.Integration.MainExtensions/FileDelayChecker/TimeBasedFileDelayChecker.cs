using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileDelayChecker
{
    public class TimeBasedFileDelayChecker : Vanrise.Integration.Entities.FileDelayChecker
    {
        public override Guid ConfigId { get { return new Guid("A498CF7D-0D01-4384-992C-644238F11C09"); } }

        public TimeSpan FileInterval { get; set; } 

        public override bool IsDelayed(IFileDelayCheckerIsDelayedContext context)
        {
            if (!context.LastRetrievedFileTime.HasValue)
                return false;

            return DateTime.Now - context.LastRetrievedFileTime.Value > this.FileInterval;
        }
    }
}
