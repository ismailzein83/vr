using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefinitionScheduleTaskFilter : IBPDefinitionFilter
    {
        public bool IsMatched(IBPDefinitionFilterContext context)
        {
            return new BPDefinitionManager().DoesUserHaveScheduleTaskAccess(ContextFactory.GetContext().GetLoggedInUserId(),context.BPDefinitionId);
        }
    }
}
