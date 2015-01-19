using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TABS;
using TOne.RepricingProcess.Arguments;

namespace TOne.RepricingProcess.Activities
{

    public sealed class DivideDayIntoTimeRanges : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<DateTime> Day { get; set; }
        public InArgument<TimeSpan> Interval { get; set; }
        public OutArgument<List<TimeRange>> Ranges { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {            
            var ranges = new List<TimeRange>();
            TimeSpan interval = this.Interval.Get(context);

            for (DateTime current = this.Day.Get(context); current < this.Day.Get(context).AddDays(1); current = current.Add(interval))
            {
                ranges.Add(
                        new TimeRange
                        {
                            From = current,
                            To = current.Add(interval)
                        }
                    );
            }
            this.Ranges.Set(context, ranges);
        }
    }    
}
