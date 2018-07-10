using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;

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
            return _lookUpBEDefinitionManager.GetLookUpBEDefinitionTitle(context.EntityDefinition.BusinessEntityDefinitionId);
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
            public override bool IsCacheExpired(Guid businessEntityDefinitionId, ref DateTime? lastCheckTime)
            {
                LKUPBusinessEntityDefinitionManager _manager = new LKUPBusinessEntityDefinitionManager();
                var lookUpSettings = _manager.GetLookUpBEDefinitionSettings(businessEntityDefinitionId);
                lookUpSettings.ThrowIfNull("lookUpSettings", businessEntityDefinitionId);
                lookUpSettings.ExtendedSettings.ThrowIfNull("lookUpSettings.ExtendedSettings", businessEntityDefinitionId);
                return lookUpSettings.ExtendedSettings.IsCacheExpired(businessEntityDefinitionId, ref lastCheckTime);
            }
        }
        public class LKUPBusinessEntityExtendedSettingsContext : ILKUPBusinessEntityExtendedSettingsContext
        {
            public BusinessEntityDefinitionSettings BEDefinitionSettings { get; set; }
        }

    }
}
