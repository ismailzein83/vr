using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class NoOptionsRule : BaseRouteAlertCondition
    {
        public override bool IsViolated(RouteDetail routeDetail)
        {
            return (routeDetail.Options == null 
                        || (!routeDetail.Options.IsBlock 
                            && (routeDetail.Options.SupplierOptions == null || routeDetail.Options.SupplierOptions.Count == 0)
                            )
                    );
        }
    }
}
