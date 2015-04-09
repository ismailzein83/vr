using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Business
{
    public class GenericEntityManager
    {
        public IEnumerable<T> GetEntities<T>(bool isBaseType, int? ownerId, int? linkedToEntityId, int? parentId) where T : GenericEntity
        {
            return null;
        }

        public IEnumerable<T> GetEntitiesByOwner<T>(bool isBaseType, int ownerId) where T : GenericEntity
        {
            return null;
        }

        public IEnumerable<T> GetEntitiesByOwners<T>(bool isBaseType, IEnumerable<int> ownerIds) where T : GenericEntity
        {
            return null;
        }

        public IEnumerable<T> GetEntitiesByLinkedEntity<T>(bool isBaseType, int linkedToEntityId) where T : GenericEntity
        {
            return null;
        }

    }
}