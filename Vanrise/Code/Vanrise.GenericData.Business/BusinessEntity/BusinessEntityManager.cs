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

        public dynamic GetParentEntityId(int businessEntityDefinitionId, int parentBusinessEntityDefinitionId, dynamic entityId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            var context = new BusinessEntityGetParentEntityIdContext(businessEntityDefinitionId, parentBusinessEntityDefinitionId, entityId);
            return beManager.GetParentEntityId(context);
        }
        
        public IEnumerable<dynamic> GetChildEntitiesIds(int businessEntityDefinitionId, int childBusinessEntityDefinitionId, IEnumerable<dynamic> parentEntitiesIds)
        {
            List<dynamic> childEntitiesIds = new List<dynamic>();
            var childBeManager = GetBEManager(childBusinessEntityDefinitionId);
            foreach(var parentEntityId in parentEntitiesIds)
            {
                var context = new BusinessEntityGetIdsByParentEntityIdContext(childBusinessEntityDefinitionId, businessEntityDefinitionId, parentEntityId);
                var currentChildEntitiesIds = childBeManager.GetIdsByParentEntityId(context);
                if (currentChildEntitiesIds != null)
                    childEntitiesIds.AddRange(currentChildEntitiesIds);
            }
            return childEntitiesIds.Distinct().ToList();            
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
