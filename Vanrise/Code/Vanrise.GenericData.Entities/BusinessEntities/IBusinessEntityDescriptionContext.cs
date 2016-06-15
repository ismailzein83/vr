using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IBusinessEntityDescriptionContext
    {
        BusinessEntityDefinition EntityDefinition { get; }

        object EntityId { get; }
    }

    public interface IBusinessEntityGetByIdContext
    {
        dynamic EntityId { get; }

        int EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }
    }

    public interface IBusinessEntityGetAllContext
    {
        int EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }
    }

    public interface IBusinessEntityIsCacheExpiredContext
    {
        int EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }
    }
}
