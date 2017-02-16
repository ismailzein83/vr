using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IBusinessEntityManager
    {
        List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context);

        dynamic GetEntity(IBusinessEntityGetByIdContext context);

        dynamic GetEntityId(IBusinessEntityIdContext context);

        string GetEntityDescription(IBusinessEntityDescriptionContext context);

        dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context);

        bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime);

        dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context);

        IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context);
    }
}
