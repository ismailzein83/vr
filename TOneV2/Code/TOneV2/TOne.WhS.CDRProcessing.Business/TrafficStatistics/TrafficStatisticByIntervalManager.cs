using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public class TrafficStatisticByIntervalManager : BaseTrafficStatisticManager<TrafficStatisticByInterval>
    {
        int _intervalInMinute;

        public TrafficStatisticByIntervalManager(int intervalInMinute)
        {
            this._intervalInMinute = intervalInMinute;
        }

        protected override void GetRawItemBatchTimeRange(BillingCDRBase rawItem, out DateTime batchStart, out DateTime batchEnd)
        {
            DateTime cdrTime = rawItem.Attempt;
            batchStart = new DateTime(cdrTime.Year, cdrTime.Month, cdrTime.Day, cdrTime.Hour, ((int)(cdrTime.Minute / _intervalInMinute)) * _intervalInMinute, 0);
            batchEnd = batchStart.AddMinutes(_intervalInMinute);
        }
    }
}
