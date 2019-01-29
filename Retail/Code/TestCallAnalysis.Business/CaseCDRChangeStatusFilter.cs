using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TestCallAnalysis.Business
{
    public class CaseCDRChangeStatusFilter : IStatusDefinitionFilter
    {
        public Guid StatusDefinitionId { get; set; }

        public bool IsMatched(IStatusDefinitionFilterContext context)
        {
            if (context.StatusDefinition == null || context.StatusDefinition.StatusDefinitionId == null)
                return false;

            if (context.StatusDefinition.StatusDefinitionId == StatusDefinitionId)
                return false;

            return true;
        }
    }
}
