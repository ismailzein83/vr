using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityManager
    {
        #region Public Methods

        public List<dynamic> GetAllEntities(Guid businessEntityDefinitionId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);

            var getAllEntitiesContext = new BusinessEntityGetAllContext(businessEntityDefinitionId);
            return beManager.GetAllEntities(getAllEntitiesContext);
        }

        public dynamic GetEntity(Guid businessEntityDefinitionId, dynamic entityId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            return GetEntity(businessEntityDefinitionId, entityId, beManager);
        }

        public string GetEntityDescription(Guid businessEntityDefinitionId, dynamic entityId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            var beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(businessEntityDefinitionId);
            return beManager.GetEntityDescription(new BusinessEntityDescriptionContext { EntityDefinition = beDefinition, EntityId = entityId });
        }

        public dynamic GetEntityId(Guid businessEntityDefinitionId, dynamic entity)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            var beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(businessEntityDefinitionId);
            return beManager.GetEntityId(new BusinessEntityIdContext { EntityDefinition = beDefinition, Entity = entity });
        }
         
        public dynamic GetEntityInfo(Guid businessEntityDefinitionId, string infoType, dynamic entityId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            var entity = GetEntity(businessEntityDefinitionId, entityId, beManager);
            if (entity == null)
                throw new NullReferenceException(String.Format("entity '{0}'. businessEntityDefinitionId '{1}'", entityId, businessEntityDefinitionId));
            return MapEntityToInfo(businessEntityDefinitionId, infoType, entity, beManager);
        }

        public dynamic GetParentEntityId(Guid businessEntityDefinitionId, Guid parentBusinessEntityDefinitionId, dynamic entityId)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            var context = new BusinessEntityGetParentEntityIdContext(businessEntityDefinitionId, parentBusinessEntityDefinitionId, entityId);
            return beManager.GetParentEntityId(context);
        }

        public IEnumerable<dynamic> GetChildEntitiesIds(Guid businessEntityDefinitionId, Guid childBusinessEntityDefinitionId, IEnumerable<dynamic> parentEntitiesIds)
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

        public IEnumerable<BusinessEntityInfo> GetBusinessEntitiesInfo(Guid businessEntityDefinitionId)
        {
            List<BusinessEntityInfo> results = new List<BusinessEntityInfo>();
            var allBusinessEntities = this.GetAllEntities(businessEntityDefinitionId);

            foreach (var businessEntity in allBusinessEntities)
            {
                object businessEntityId = this.GetEntityId(businessEntityDefinitionId, businessEntity);
                results.Add(new BusinessEntityInfo()
                {
                    BusinessEntityId = businessEntityId,
                    Description = this.GetEntityDescription(businessEntityDefinitionId, businessEntityId)
                   
                });
            }
            return results;
        }

        #endregion

        #region Private Methods

        private dynamic MapEntityToInfo(Guid businessEntityDefinitionId, string infoType, dynamic entity)
        {
            var beManager = GetBEManager(businessEntityDefinitionId);
            return MapEntityToInfo(businessEntityDefinitionId, infoType, entity, beManager);
        }

        private BaseBusinessEntityManager GetBEManager(Guid businessEntityDefinitionId)
        {
            var beManager = (new BusinessEntityDefinitionManager()).GetBusinessEntityManager(businessEntityDefinitionId);
            if (beManager == null)
                throw new NullReferenceException(String.Format("beManager. BusinessEntityDefinitionId '{0}'", businessEntityDefinitionId));
            return beManager;
        }

        private dynamic GetEntity(Guid businessEntityDefinitionId, dynamic entityId, BaseBusinessEntityManager beManager)
        {
            var getByIdContext = new BusinessEntityGetByIdContext(businessEntityDefinitionId);
            getByIdContext.EntityId = entityId;
            return beManager.GetEntity(getByIdContext);
        }

        private dynamic MapEntityToInfo(Guid businessEntityDefinitionId, string infoType, dynamic entity, BaseBusinessEntityManager beManager)
        {
            var mapEntityToInfoContext = new BusinessEntityMapToInfoContext(businessEntityDefinitionId);
            mapEntityToInfoContext.InfoType = infoType;
            mapEntityToInfoContext.Entity = entity;
            return beManager.MapEntityToInfo(mapEntityToInfoContext);
        }

        #endregion
    }
}
