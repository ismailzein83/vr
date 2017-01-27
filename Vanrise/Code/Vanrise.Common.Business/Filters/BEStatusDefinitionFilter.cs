using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class BEStatusDefinitionFilter : IStatusDefinitionFilter
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public bool IsMatched(IStatusDefinitionFilterContext context)
        {
            if (context.BusinessEntityDefinitionId != this.BusinessEntityDefinitionId)
                return false;
            return true;
        }
    }
}
