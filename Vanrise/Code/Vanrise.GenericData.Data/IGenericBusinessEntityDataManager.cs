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
        List<GenericBusinessEntity> GetGenericBusinessEntities(int businessDefinitionId);
        bool AreGenericBusinessEntityUpdated(ref object updateHandle);
    }
}
