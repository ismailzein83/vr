using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class DestinationGroupManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<DestinationGroupDetail> GetFilteredDestinationGroups(Vanrise.Entities.DataRetrievalInput<DestinationGroupQuery> input)
        {
            var allDestinationGroups = GetCachedDestinationGroups();

            Func<DestinationGroup, bool> filterExpression = (x) =>
                (input.Query.DestinationTypes == null || input.Query.DestinationTypes.Count == 0 || input.Query.DestinationTypes.Contains(x.DestinationType))
                &&
                (input.Query.Name == null || x.Name.Contains(input.Query.Name));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDestinationGroups.ToBigResult(input, filterExpression, DestinationGroupDetailMapper));
        }
        public DestinationGroup GetDestinationGroup(int destinationGroupId)
        {
            var info = GetCachedDestinationGroups();
            return info.GetRecord(destinationGroupId);
        }
        public Vanrise.Entities.InsertOperationOutput<DestinationGroupDetail> AddDestinationGroup(DestinationGroup destinationGroup)
        {
            Vanrise.Entities.InsertOperationOutput<DestinationGroupDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DestinationGroupDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int groupId = -1;

            IDestinationGroupDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IDestinationGroupDataManager>();
            bool insertActionSucc = dataManager.Insert(destinationGroup, out groupId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                destinationGroup.DestinationGroupId = groupId;
                insertOperationOutput.InsertedObject = DestinationGroupDetailMapper(destinationGroup);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<DestinationGroupDetail> UpdateDestinationGroup(DestinationGroup destinationGroup)
        {
            IDestinationGroupDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IDestinationGroupDataManager>();

            bool updateActionSucc = dataManager.Update(destinationGroup);
            Vanrise.Entities.UpdateOperationOutput<DestinationGroupDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DestinationGroupDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DestinationGroupDetailMapper(destinationGroup);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public IEnumerable<DestinationGroupInfo> GetDestinationGroupsInfo()
        {
            var allDestinationGroups = GetCachedDestinationGroups();
            if (allDestinationGroups == null)
                return null;

            return allDestinationGroups.MapRecords(DestinationGroupInfoMapper);
        }

        public List<TemplateConfig> GetGroupTypeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.GroupTypesConfigType);
        }

        #endregion

        #region Private Members
        private Dictionary<int, DestinationGroup> GetCachedDestinationGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDestinationGroups",
               () =>
               {
                   IDestinationGroupDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IDestinationGroupDataManager>();
                   IEnumerable<DestinationGroup> config = dataManager.GetDestinationGroups();
                   return config.ToDictionary(cn => cn.DestinationGroupId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDestinationGroupDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IDestinationGroupDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDestinationGroupsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers

        private DestinationGroupDetail DestinationGroupDetailMapper(DestinationGroup group)
        {
            DestinationGroupDetail groupDetail = new DestinationGroupDetail();
            groupDetail.Entity = group;

            List<TemplateConfig> groupTypeTemplates = GetGroupTypeTemplates();

            var destinationType = groupTypeTemplates.FirstOrDefault(x => x.TemplateConfigID == group.DestinationType);
            if (destinationType != null)
                groupDetail.DestinationTypeName = destinationType.Name;

            return groupDetail;
        }

        private DestinationGroupInfo DestinationGroupInfoMapper(DestinationGroup group)
        {
            DestinationGroupInfo groupInfo = new DestinationGroupInfo();
            groupInfo.DestinationGroupId = group.DestinationGroupId;
            groupInfo.Name = group.Name;
            return groupInfo;
        }
        #endregion
    }

}
