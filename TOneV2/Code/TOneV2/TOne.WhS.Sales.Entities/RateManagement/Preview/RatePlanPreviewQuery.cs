using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class RatePlanPreviewQuery
    {
        public long ProcessInstanceId { get; set; }
    }

    public class RatePreviewQuery : RatePlanPreviewQuery
    {
        public string ZoneName { get; set; }
    }
}
