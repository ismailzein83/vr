using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IBusinessEntityIdContext
    {
        BusinessEntityDefinition EntityDefinition { get; }

        dynamic Entity { get; }
    }

    public interface IBusinessEntityDescriptionContext
    {
        BusinessEntityDefinition EntityDefinition { get; }

        object EntityId { get; }
    }
     
    public interface IBusinessEntityGetByIdContext
    {
        dynamic EntityId { get; }

        Guid EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }
    }

    public interface IBusinessEntityMapToInfoContext
    {
        string InfoType { get; }

        dynamic Entity { get; }

        Guid EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }
    }

    public interface IBusinessEntityGetAllContext
    {
        Guid EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }
    }

    public interface IBusinessEntityIsCacheExpiredContext
    {
        Guid EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }
    }

    public interface IBusinessEntityGetParentEntityIdContext
    {
        Guid EntityDefinitionId { get; }
        
        BusinessEntityDefinition EntityDefinition { get; }

        Guid ParentEntityDefinitionId { get; }

        BusinessEntityDefinition ParentEntityDefinition { get; }

        dynamic EntityId { get; }

        dynamic Entity { get; }
    }

    public interface IBusinessEntityGetIdsByParentEntityIdContext
    {
        Guid EntityDefinitionId { get; }

        BusinessEntityDefinition EntityDefinition { get; }

        Guid ParentEntityDefinitionId { get; }

        BusinessEntityDefinition ParentEntityDefinition { get; }

        dynamic ParentEntityId { get; }

        dynamic ParentEntity { get; }
    }
    public interface IBusinessEntityGetIdByDescriptionContext
    {
         Object FieldDescription { get; }

         string ErrorMessage { set; }

         Object FieldValue { set; }

         DataRecordFieldType FieldType { get; }

         BERuntimeSelectorFilter BERuntimeSelectorFilter { get; }
    }
}
