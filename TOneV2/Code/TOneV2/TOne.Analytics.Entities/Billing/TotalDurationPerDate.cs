using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TotalDurationPerDate
    {
        public DateTime CallDate { get; set; }

        public decimal TotalDuration { get; set; }

        public TotalDurationPerDate(DateTime callDate,decimal totalDuration) {

            this.CallDate = callDate;
            this.TotalDuration = totalDuration;
        }
    }
}
