using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IGenericBusinessEntityDataManager:IDataManager
    {
        bool UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity);
        bool AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity, out long genericBusinessEntityId);
        List<GenericBusinessEntity> GetGenericBusinessEntitiesByDefinition(int businessDefinitionId, Type dataRecordRuntimeTime);
        bool AreGenericBusinessEntityUpdated(int parameter,ref object updateHandle);
        bool DeleteGenericBusinessEntity(long genericBusinessEntityId, int businessEntityDefinitionId);
    }
}
