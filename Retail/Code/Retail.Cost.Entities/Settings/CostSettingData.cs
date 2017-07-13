using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Cost.Entities
{
    public class CostSettingData
    {
        public int MaxBatchDurationInMinutes { get; set; }
         
        public TimeSpan DurationMargin { get; set; }

        public TimeSpan AttemptDateTimeMargin { get; set; }

        public TimeSpan AttemptDateTimeOffset { get; set; } 
    }
}
