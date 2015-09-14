using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.CDRProcess.Arguments
{
    public class TimeRangeRepricingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public TimeRange Range { get; set; }
        public Guid CacheManagerID { get; set; }
        public bool GenerateTrafficStatistic { get; set; }
        public List<string> CustomersIds { get; set; }
        public List<string> SupplierIds { get; set; }
        public override string GetTitle()
        {
            return String.Format("Time Range Repricing Process for range {0:HH:mm} - {1:HH:mm} in date {2:dd-MMM-yyyy}", Range.From, Range.To, Range.From);
        }
    }
}
