using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Reprocess.Entities;
using System.Linq;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class GenerateDateTimeRanges : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        [RequiredArgument]
        public InArgument<ChunkTime> ChunkTime { get; set; }

        [RequiredArgument]
        public OutArgument<List<DateTimeRange>> DateTimeRanges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime from = this.From.Get(context);
            DateTime to = this.To.Get(context);
            ChunkTime chunkTime = this.ChunkTime.Get(context);

            var chunkTimeAttribute = Vanrise.Common.Utilities.GetEnumAttribute<ChunkTime, ChunkTimeAttribute>(chunkTime);
            int intervalInMinutes = chunkTimeAttribute.Value;
            List<DateTimeRange> dateTimeRanges = new List<DateTimeRange>();

            DateTime endDate = from;
            DateTime startDate = endDate;

            while (endDate != to)
            {
                startDate = endDate;

                DateTime tempToDate = endDate.AddMinutes(intervalInMinutes);
                
                if (tempToDate > to)
                    endDate = to;
                else
                    endDate = tempToDate;


                dateTimeRanges.Add(new DateTimeRange() { From = startDate, To = endDate });
            }
            dateTimeRanges = dateTimeRanges.OrderBy(itm => itm.From).ToList();
            this.DateTimeRanges.Set(context, dateTimeRanges);
        }
    }
}