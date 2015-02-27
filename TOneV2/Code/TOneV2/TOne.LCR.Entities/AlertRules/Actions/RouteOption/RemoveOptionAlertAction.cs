using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RemoveOptionAlertAction : BaseRouteOptionAlertAction
    {
        public override void Execute(RouteSupplierOption option, RouteDetail routeDetail)
        {
            routeDetail.Options.SupplierOptions.Remove(option);
        }
    }
}
