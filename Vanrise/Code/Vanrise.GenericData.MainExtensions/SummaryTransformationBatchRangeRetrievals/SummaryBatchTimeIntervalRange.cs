using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public enum SummaryBatchIntervalType { Minutes = 0, Hours = 1, Days = 2 }
    public class SummaryBatchTimeIntervalRange : SummaryTransformationBatchRangeRetrieval
    {
        public SummaryBatchIntervalType IntervalType { get; set; }

        public int IntervalOffset { get; set; }

        public override void GetRawItemBatchTimeRange(dynamic rawItem, DateTime rawItemTime, out DateTime batchStart)
        {
            batchStart = new DateTime(rawItemTime.Year, rawItemTime.Month, rawItemTime.Day, rawItemTime.Hour, ((int)(rawItemTime.Minute / this.IntervalOffset)) * this.IntervalOffset, 0);
        }
    }
}

