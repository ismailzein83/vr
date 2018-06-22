using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public interface IGenericBusinessEntityManager : IBusinessManager
    {
        R GetCachedOrCreate<R>(Object cacheName, Guid businessEntityDefinitionId, Func<R> createObject);
        List<GenericBusinessEntity> GetAllGenericBusinessEntities(Guid businessEntityDefinitionId);
        InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntityToAdd genericBusinessEntityToAdd);
        //void SetCacheExpired(Guid businessEntityDefinitionId);
    }
}
