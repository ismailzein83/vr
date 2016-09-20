using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteOptionOrderSettings
    {
        public virtual  Guid ConfigId { get; set; }

        public decimal? PercentageValue { get; set; }

        public abstract void Execute(IRouteOptionOrderExecutionContext context);
    }

    public class OptionOrderTemplateConfigSettings
    {
        public bool IsRequired { get; set; }
    }
}
