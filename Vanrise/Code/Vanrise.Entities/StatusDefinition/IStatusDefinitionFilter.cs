using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public interface IStatusDefinitionFilter
    {
        bool IsMatched(IStatusDefinitionFilterContext context);
    }
    public interface IStatusDefinitionFilterContext
    {
        StatusDefinition StatusDefinition { get; }
        Guid BusinessEntityDefinitionId { get; }
    }
}
