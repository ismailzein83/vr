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
        public override Guid ConfigId { get { return  new Guid("c47176fb-32eb-430c-b92d-d34dfadcddf9"); } }
        public SummaryBatchIntervalType IntervalType { get; set; }

        public int IntervalOffset { get; set; }

        public override void GetRawItemBatchTimeRange(dynamic rawItem, DateTime rawItemTime, out DateTime batchStart, out DateTime batchEnd)
        {
            switch (this.IntervalType)
            {
                case SummaryBatchIntervalType.Minutes:
                    batchStart = new DateTime(rawItemTime.Year, rawItemTime.Month, rawItemTime.Day, rawItemTime.Hour, ((int)(rawItemTime.Minute / this.IntervalOffset)) * this.IntervalOffset, 0);
                    batchEnd = batchStart.AddMinutes(this.IntervalOffset);
                    break;
                case SummaryBatchIntervalType.Hours:
                    batchStart = new DateTime(rawItemTime.Year, rawItemTime.Month, rawItemTime.Day, ((int)(rawItemTime.Hour / this.IntervalOffset)) * this.IntervalOffset, 0, 0);
                    batchEnd = batchStart.AddHours(this.IntervalOffset);
                    break;
                case SummaryBatchIntervalType.Days:
                    batchStart = rawItemTime.Date;
                    batchEnd = batchStart.AddDays(1);
                    break;
                default:
                    throw new NotImplementedException();

            }
        }
    }
}

