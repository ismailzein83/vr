using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class BaseBusinessEntityManager
    {
        public abstract List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context);

        public abstract dynamic GetEntity(IBusinessEntityGetByIdContext context);

        public abstract dynamic GetEntityId(IBusinessEntityIdContext context);

        public abstract string GetEntityDescription(IBusinessEntityDescriptionContext context);

        public abstract dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context);

        public abstract bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime);

        public abstract dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context);

        public abstract IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context);

        public virtual void GetIdByDescription(IBusinessEntityGetIdByDescriptionContext context)
        {

        }
    }
}
