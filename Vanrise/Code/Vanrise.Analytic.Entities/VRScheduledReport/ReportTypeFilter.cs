using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class ReportTypeFilter : IComponentTypeFilter
    {
        public bool IsMatched(IComponentTypeFilterContext context)
        {
            context.ThrowIfNull("context", context);
            var componentType = context.componentType;

            componentType.ThrowIfNull("ComponentType", componentType.VRComponentTypeId);

            if (componentType.Settings is VRReportTypeDefinitionSettings)
                return true;

            else
                return false;
        }
    }
}
