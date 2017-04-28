using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class GenerateDateTimeRanges : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> TimeSpan { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<DateTimeRange>> DateTimeRanges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime from = this.From.Get(context);
            DateTime to = this.To.Get(context);
            TimeSpan timeSpan = this.TimeSpan.Get(context);

            IEnumerable<DateTimeRange> dateTimeRanges = Vanrise.Common.Utilities.GenerateDateTimeRanges(from, to, timeSpan);

            this.DateTimeRanges.Set(context, dateTimeRanges);
        }
    }
}