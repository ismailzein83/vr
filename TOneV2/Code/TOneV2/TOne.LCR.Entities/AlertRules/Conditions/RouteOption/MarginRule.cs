using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class MarginRule : BaseRouteOptionAlertCondition
    {
        public decimal MinMargin { get; set; }

        public override bool IsViolated(RouteSupplierOption option, RouteDetail routeDetail)
        {
            decimal margin = routeDetail.Rate - option.Rate;
            return (margin < this.MinMargin);
        }
        
    }
}
