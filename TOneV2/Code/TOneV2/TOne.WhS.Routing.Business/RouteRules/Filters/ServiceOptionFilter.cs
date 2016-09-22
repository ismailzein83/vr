using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Filters
{
    public class ServiceOptionFilter : RouteOptionFilterSettings
    {
        public override Guid ConfigId { get { return new Guid("DD2B8C20-C9D2-44BE-9EF5-0890B52FCB9C"); } }


        public override void Execute(IRouteOptionFilterExecutionContext context)
        {
            if (context.CustomerServices == null)
                return;

            foreach (int itm in context.CustomerServices)
            {
                if (context.SupplierServices == null || !context.SupplierServices.Contains(itm))
                {
                    context.FilterOption = true;
                    return;
                }
            }
        }
    }
}
