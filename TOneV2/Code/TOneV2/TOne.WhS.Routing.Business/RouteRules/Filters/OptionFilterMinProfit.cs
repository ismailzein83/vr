using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Filters
{
    public class OptionFilterMinProfit: RouteOptionFilterSettings
    {
        public int Profit { get; set; }

        public override void Execute(IRouteOptionFilterExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
