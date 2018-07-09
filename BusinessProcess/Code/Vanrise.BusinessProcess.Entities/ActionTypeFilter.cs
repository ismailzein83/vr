using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class ActionTypeFilter : IActionTypesFilter
    {
        public List<Guid> ActionTypesIds { get; set;}

        public bool IsExcluded(IActionTypeFilterContext context)
        {
            return !ActionTypesIds.Contains(context.ActionTypeId);
        }
    }
}
