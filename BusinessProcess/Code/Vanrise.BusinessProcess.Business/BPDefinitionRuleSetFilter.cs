using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefinitionRuleSetFilter : IBPDefinitionFilter
    {
        public bool IsMatched(IBPDefinitionFilterContext context)
        {
            BPDefinition bpDefinition = new BPDefinitionManager().GetBPDefinition(context.BPDefinitionId);
            if (bpDefinition == null)
                throw new NullReferenceException();
            return bpDefinition.Configuration.BusinessRuleSetSupported;
        }
    }
}
