using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business.Filters
{

    public class BEAlertLevelDefinitionFilter : IVRALertFilter
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public bool IsMatched(IVRALertLevelFilterContext context)
            {
                if (context.VRAlertLevel == null )
                    return false;

                if (context.VRAlertLevel.BusinessEntityDefinitionId != this.BusinessEntityDefinitionId)
                    return false;

                return true;
            }
        }
   
}
