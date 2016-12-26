using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IBusinessEntityDefinitionFilter
    {
        bool IsMatched(IBusinessEntityDefinitionFilterContext context);
    }

    public interface IBusinessEntityDefinitionFilterContext
    {
        BusinessEntityDefinition entityDefinition { get; }
    }
}
