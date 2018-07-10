using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using System.Collections.Concurrent;

namespace Vanrise.GenericData.Business
{
    public class LKUPBusinessEntityManager : BaseBusinessEntityManager
    {
        #region Fields / Constructors
        LKUPBusinessEntityDefinitionManager _lookUpBEDefinitionManager = new LKUPBusinessEntityDefinitionManager();
        #endregion

        #region BaseBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            context.EntityDefinition.ThrowIfNull("context.EntityDefinition");
            return GetLKUPBusinessEntityName(context.EntityDefinition.BusinessEntityDefinitionId, context.EntityId.ToString());
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public LKUPBusinessEntityItem GetLKUPBusinessEntity(Guid businessEntityDefinitionId, string lkupBusinessEntityItemId)
        {
            return this.GetCachedLKUPItems(businessEntityDefinitionId).GetRecord(lkupBusinessEntityItemId);
        }
        public string GetLKUPBusinessEntityName(Guid businessEntityDefinitionId, string lkupBusinessEntityItemId)
        {
            var entity = GetLKUPBusinessEntity( businessEntityDefinitionId,  lkupBusinessEntityItemId);
            if (entity == null)
                return null;
            return entity.Name;
        }
        public IEnumerable<LKUPBusinessEntityItemInfo> GetLKUPBusinessEntityInfo(Guid businessEntityDefinitionId, LKUPBusinessEntityInfoFilter filter)
        {
            Func<LKUPBusinessEntityItem, bool> filterExpression = null;
            
            filterExpression = (lkupBusinessEntityItem) =>
            {
                if (lkupBusinessEntityItem.BusinessEntityDefinitionId != businessEntityDefinitionId)
                    return false;
                return true;
            };
            return this.GetCachedLKUPItems(businessEntityDefinitionId).MapRecords(LKUPBusinessEntityItemInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, LKUPBusinessEntityItem> GetCachedLKUPItems(Guid businessEntityDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedLKUPItems",businessEntityDefinitionId,
               () =>
               {
                   LKUPBusinessEntityDefinitionManager _manager = new LKUPBusinessEntityDefinitionManager();
                   var lookUpSettings = _manager.GetLookUpBEDefinitionSettings(businessEntityDefinitionId);
                   lookUpSettings.ThrowIfNull("lookUpSettings", businessEntityDefinitionId);
                   lookUpSettings.ExtendedSettings.ThrowIfNull("lookUpSettings.ExtendedSettings", businessEntityDefinitionId);
                   return lookUpSettings.ExtendedSettings.GetAllLKUPBusinessEntityItems(new LKUPBusinessEntityExtendedSettingsContext
                   {
                       BEDefinitionSettings = lookUpSettings
                   });
               });
        }
        private LKUPBusinessEntityItemInfo LKUPBusinessEntityItemInfoMapper(LKUPBusinessEntityItem LKUPItem)
        {
            return new LKUPBusinessEntityItemInfo()
            {
                LKUPBusinessEntityItemId = LKUPItem.LKUPBusinessEntityItemId,
                Name = LKUPItem.Name
            };
        }

        #endregion
        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {

            ConcurrentDictionary<Guid, DateTime?> _lastCheckTimeByBEDefinitionId = new ConcurrentDictionary<Guid, DateTime?>();
            LKUPBusinessEntityDefinitionManager _manager = new LKUPBusinessEntityDefinitionManager();
            protected override bool ShouldSetCacheExpired(Guid parameter)
            {
                DateTime? lastCheckTime;
                _lastCheckTimeByBEDefinitionId.TryGetValue(parameter, out lastCheckTime);
                var lookUpSettings = _manager.GetLookUpBEDefinitionSettings(parameter);
                lookUpSettings.ThrowIfNull("lookUpSettings", parameter);
                lookUpSettings.ExtendedSettings.ThrowIfNull("lookUpSettings.ExtendedSettings", parameter);
                bool isCacheExpired = lookUpSettings.ExtendedSettings.IsCacheExpired(ref lastCheckTime);
                _lastCheckTimeByBEDefinitionId.AddOrUpdate(parameter, lastCheckTime, (key, existingCheckTime) => lastCheckTime);
                return isCacheExpired ;
            }

        }
        public class LKUPBusinessEntityExtendedSettingsContext : ILKUPBusinessEntityExtendedSettingsContext
        {
            public BusinessEntityDefinitionSettings BEDefinitionSettings { get; set; }
        }

    }
}
