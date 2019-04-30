using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class WorkflowGetBEActivityFilter : IBusinessEntityDefinitionFilter
    {
        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            if (context.entityDefinition == null || context.entityDefinition.Settings == null || context.entityDefinition.Settings.WorkFlowGetBEActivityEditor == null)
                return false;

            return true;
        }
    }
}
