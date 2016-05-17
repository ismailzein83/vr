using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityManager
    {
        public List<dynamic> GetAllEntities(int businessEntityDefinitionId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);

            var getAllEntitiesContext = new BusinessEntityGetAllContext(businessEntityDefinitionId);
            return beManager.GetAllEntities(getAllEntitiesContext);
        }

        public dynamic GetEntity(int businessEntityDefinitionId, dynamic entityId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            var getByIdContext = new BusinessEntityGetByIdContext(businessEntityDefinitionId);
            getByIdContext.EntityId = entityId;
            return beManager.GetEntity(getByIdContext);
        }

        private Entities.IBusinessEntityManager GetBEManager(int businessEntityDefinitionId)
        {
            var beManager = (new BusinessEntityDefinitionManager()).GetBusinessEntityManager(businessEntityDefinitionId);
            if (beManager == null)
                throw new NullReferenceException(String.Format("beManager. BusinessEntityDefinitionId '{0}'", businessEntityDefinitionId));
            return beManager;
        }

    }
}
