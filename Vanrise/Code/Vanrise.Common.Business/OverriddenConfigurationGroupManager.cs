using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class OverriddenConfigurationGroupManager
    {
        public IEnumerable<OverriddenConfigurationGroupInfo> GetOverriddenConfigurationGroupInfo(OverriddenConfigurationGroupInfoFilter filter)
        {
            Func<OverriddenConfigurationGroup, bool> filterExpression = null;
            if (filter != null)
            {
            }
            return GetCachedOverriddenConfigurationGroups().MapRecords(VROverriddenConfigurationGroupInfoMapper, filterExpression);
        }
        public OverriddenConfigurationGroup GetOverriddenConfigurationGroup(Guid overriddenConfigurationGroupId)
        {
            return GetCachedOverriddenConfigurationGroups().GetRecord(overriddenConfigurationGroupId);
        }
        public IEnumerable<Guid> GetEffectiveOverriddenConfigurationGroupIds()
        {
            var configuration = ConfigurationManager.AppSettings["EffectiveOverriddenConfigurationGroupIds"];
            if (configuration == null)
                return null;
            var cachedGroups = GetCachedOverriddenConfigurationGroups();
            var groupIds = configuration.Split(',');
            List<Guid> effectiveGroupIds = new List<Guid>();
            foreach(var item in groupIds)
            {
                Guid groupId;
                if(Guid.TryParse(item, out groupId) && cachedGroups.ContainsKey(groupId))
                {
                    effectiveGroupIds.Add(groupId);
                }
            }
            return effectiveGroupIds;
        }
        public void GenerateScript(List<OverriddenConfigurationGroup> groups, Action<string, string> addEntityScript)
        {
            IOverriddenConfigurationGroupDataManager dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationGroupDataManager>();
            dataManager.GenerateScript(groups, addEntityScript);
        }
        public Vanrise.Entities.InsertOperationOutput<OverriddenConfigGroupDetail> AddOverriddenConfigurationGroup(OverriddenConfigurationGroup overriddenConfigurationGroup)
        {
            Vanrise.Entities.InsertOperationOutput<OverriddenConfigGroupDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OverriddenConfigGroupDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IOverriddenConfigurationGroupDataManager dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationGroupDataManager>();
            overriddenConfigurationGroup.OverriddenConfigurationGroupId = Guid.NewGuid();
            if (dataManager.Insert(overriddenConfigurationGroup))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = OverriddenConfigGroupDetailMapper(overriddenConfigurationGroup);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOverriddenConfigurationGroupDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationGroupDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOverriddenConfigurationGroupUpdated(ref _updateHandle);
            }
        }
        Dictionary<Guid, OverriddenConfigurationGroup> GetCachedOverriddenConfigurationGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOverriddenConfigurationGroups",
               () =>
               {
                   IOverriddenConfigurationGroupDataManager dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationGroupDataManager>();
                   return dataManager.GetOverriddenConfigurationGroup().ToDictionary(x => x.OverriddenConfigurationGroupId, x => x);
               });
        }
        private OverriddenConfigurationGroupInfo VROverriddenConfigurationGroupInfoMapper(OverriddenConfigurationGroup overriddenConfigurationGroup)
        {
            OverriddenConfigurationGroupInfo overriddenConfigurationGroupInfo = new OverriddenConfigurationGroupInfo()
            {
                OverriddenConfigurationGroupId = overriddenConfigurationGroup.OverriddenConfigurationGroupId,
                Name = overriddenConfigurationGroup.Name
            };
            return overriddenConfigurationGroupInfo;
        }
        private OverriddenConfigGroupDetail OverriddenConfigGroupDetailMapper(OverriddenConfigurationGroup overriddenConfigurationGroup)
        {
            return new OverriddenConfigGroupDetail
            {
                Name = overriddenConfigurationGroup.Name,
                OverriddenConfigurationGroupId = overriddenConfigurationGroup.OverriddenConfigurationGroupId
            };
        }
    }
}
