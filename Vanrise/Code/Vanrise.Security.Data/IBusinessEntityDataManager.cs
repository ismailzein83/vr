using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IBusinessEntityDataManager : IDataManager
    {
        IEnumerable<Vanrise.Security.Entities.BusinessEntity> GetEntities();

        bool ToggleBreakInheritance(string entityId);
        bool AddBusinessEntity(BusinessEntity businessEntity);
        bool UpdateBusinessEntity(BusinessEntity businessEntity);
        bool AreBusinessEntitiesUpdated(ref object updateHandle);
        bool UpdateBusinessEntityRank(Guid entityId, Guid moduleId);
    }
}
