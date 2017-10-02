using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefinitionScheduleTaskFilter : IBPDefinitionFilter
    {
        public bool IsMatched(IBPDefinitionFilterContext context)
        {
            bool hasAccess = new BPDefinitionManager().DoesUserHaveScheduleTaskAccess(ContextFactory.GetContext().GetLoggedInUserId(), context.BPDefinitionId);
            if (!hasAccess)
                return false;

            BPDefinition bpDefinition = new BPDefinitionManager().GetBPDefinition(context.BPDefinitionId);
            bpDefinition.ThrowIfNull("bpDefinition", context.BPDefinitionId);
            bpDefinition.Configuration.ThrowIfNull(" bpDefinition.Configuration", context.BPDefinitionId);

            bool hasScheduledExecEditor = !string.IsNullOrEmpty(bpDefinition.Configuration.ScheduledExecEditor);
            if (!hasScheduledExecEditor)
                return false;

            return true;
        }
    }
}