using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IBusinessEntityManager
    {
        string GetEntityDescription(IBusinessEntityDescriptionContext context);

        bool IsMatched(IBusinessEntityMatchContext context);

        dynamic GetEntity(IBusinessEntityGetByIdContext context);

        List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context);

        bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime);
    }
}
