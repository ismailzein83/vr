using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class VRRestAPIBusinessEntityManager : BaseBusinessEntityManager
    {
        #region Public Methods

        public string GetBusinessEntityIdType(Guid remoteBEDefinitionId, Guid connectionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.Caching.PermanentCacheManager<Guid>>().GetOrCreateObject("GetBusinessEntityIdType", remoteBEDefinitionId,
                () =>
                {
                    VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
                    return connectionSettings.Get<string>(string.Format("/api/VR_GenericData/BusinessEntityDefinition/GetBusinessEntityIdType?remoteBEDefinitionId={0}", remoteBEDefinitionId));
                });
        }

        public IEnumerable<BusinessEntityInfo> GetBusinessEntitiesInfo(Guid businessEntityDefinitionId)
        {
            return GetCachedBusinessEntitiesInfo(businessEntityDefinitionId).Values;
        }

        #endregion

        #region Private Methods

        private Dictionary<object, BusinessEntityInfo> GetCachedBusinessEntitiesInfo(Guid businessEntityDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBusinessEntitiesInfo", businessEntityDefinitionId,
               () =>
               {
                   Dictionary<object, BusinessEntityInfo> results = new Dictionary<object, BusinessEntityInfo>();

                   var businessEntityDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(businessEntityDefinitionId);
                   if (businessEntityDefinition == null)
                       throw new NullReferenceException(string.Format("businessEntityDefinition {0}", businessEntityDefinitionId));

                   var vrRestAPIBEDefinitionSettings = GetVRRestAPIBEDefinitionSettings(businessEntityDefinition);
                   VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(vrRestAPIBEDefinitionSettings.ConnectionId);

                   IEnumerable<BusinessEntityInfo> businessEntitiesInfo =
                       connectionSettings.Get<IEnumerable<BusinessEntityInfo>>(string.Format("/api/VR_GenericData/BusinessEntity/GetBusinessEntitiesInfo?businessEntityDefinitionId={0}", vrRestAPIBEDefinitionSettings.RemoteBEDefinitionId));
                   if (businessEntitiesInfo != null)
                       results = businessEntitiesInfo.ToDictionary(itm => itm.BusinessEntityId);

                   return results;
               });
        }

        private VRRestAPIBEDefinitionSettings GetVRRestAPIBEDefinitionSettings(BusinessEntityDefinition beDefinition)
        {
            var businessEntityDefinitionSettings = beDefinition.Settings;
            if (businessEntityDefinitionSettings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings {0}", beDefinition.BusinessEntityDefinitionId));

            var vrRestAPIBEDefinitionSettings = businessEntityDefinitionSettings as VRRestAPIBEDefinitionSettings;
            if (vrRestAPIBEDefinitionSettings == null)
                throw new NullReferenceException("businessEntityDefinition should be of type VRRestAPIBEDefinitionSettings");

            return vrRestAPIBEDefinitionSettings;
        }

        private VRInterAppRestConnection GetVRInterAppRestConnection(Guid connectionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            protected override bool IsTimeExpirable
            {
                get { return true; }
            }
        }

        #endregion

        #region IBusinessEntityManager

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            Dictionary<object, BusinessEntityInfo> businessEntitiesInfoByEntityId = this.GetCachedBusinessEntitiesInfo(context.EntityDefinition.BusinessEntityDefinitionId);

            BusinessEntityInfo businessEntityInfo;
            if (!businessEntitiesInfoByEntityId.TryGetValue(context.EntityId, out businessEntityInfo))
                throw new NullReferenceException(string.Format("businessEntityInfo. EntityDefinitionId:{0}. EntityId:{1}", context.EntityDefinition.BusinessEntityDefinitionId, context.EntityId));

            return businessEntityInfo.Description;
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
