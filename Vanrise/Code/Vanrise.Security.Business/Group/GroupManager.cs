using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class GroupManager
    {
        #region Public Methods

        public IDataRetrievalResult<GroupDetail> GetFilteredGroups(DataRetrievalInput<GroupQuery> input)
        {
            var allItems = GetCachedGroups();

            Func<Group, bool> filterExpression = (itemObject) => (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, GroupDetailMapper));
        }

        public IEnumerable<GroupInfo> GetGroupInfo(GroupFilter filter)
        {
            var groups = GetCachedGroups();

            if (filter != null)
            {
                if (filter.EntityType != null && filter.EntityId != null)
                {
                    PermissionManager permissionManager = new PermissionManager();
                    IEnumerable<Permission> entityPermissions = permissionManager.GetEntityPermissions((EntityType)filter.EntityType, filter.EntityId);

                    IEnumerable<int> excludedGroupIds = entityPermissions.MapRecords(permission => Convert.ToInt32(permission.HolderId), permission => permission.HolderType == HolderType.GROUP);
                    return groups.MapRecords(GroupInfoMapper, group => !excludedGroupIds.Contains(group.GroupId));
                }
            }
            
            return groups.MapRecords(GroupInfoMapper);
        }

        public Group GetGroup(int groupId)
        {
            var groups = GetCachedGroups();
            return groups.GetRecord(groupId);
        }

        public string GetGroupName(int groupId)
        {
            Group group = GetGroup(groupId);
            return group != null ? group.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<GroupDetail> AddGroup(Group groupObj)
        {
            InsertOperationOutput<GroupDetail> insertOperationOutput = new InsertOperationOutput<GroupDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int groupId = -1;

            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            bool insertActionSucc = dataManager.AddGroup(groupObj, out groupId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                groupObj.GroupId = groupId;
                insertOperationOutput.InsertedObject = GroupDetailMapper(groupObj);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<GroupDetail> UpdateGroup(Group groupObj)
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            bool updateActionSucc = dataManager.UpdateGroup(groupObj);
            UpdateOperationOutput<GroupDetail> updateOperationOutput = new UpdateOperationOutput<GroupDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GroupDetailMapper(groupObj);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public List<int> GetUserGroups(int userId)
        {
            IEnumerable<Group> allGroups = this.GetCachedGroups().Values;
            List<int> assignedGroups = new List<int>();

            foreach (var group in allGroups)
            {
                if (group.Settings != null && group.Settings.IsMember(new GroupSettingsContext() { UserId = userId}))
                    assignedGroups.Add(group.GroupId);
            }

            return assignedGroups;
        }

        public bool IsUserMemberInGroups(int userId, List<int> groups)
        {
            IEnumerable<Group> selectedGroups = this.GetCachedGroups().FindAllRecords(x => groups.Contains(x.GroupId));

            foreach (var group in selectedGroups)
            {
                if (group.Settings != null && group.Settings.IsMember(new GroupSettingsContext() { UserId = userId}))
                    return true;
            }

            return false;
        }

        public IEnumerable<GroupSettingsConfig> GetGroupTemplate()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<GroupSettingsConfig>(GroupSettingsConfig.EXTENSION_TYPE);
        }
        #endregion
        
        #region Private Methods

        Dictionary<int, Group> GetCachedGroups()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGroups",
               () =>
               {
                   IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
                   IEnumerable<Group> groups = dataManager.GetGroups();
                   return groups.ToDictionary(kvp => kvp.GroupId, kvp => kvp);
               });
        }
        
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGroupDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGroupsUpdated(ref _updateHandle);
            }
        }
        
        #endregion

        #region Mappers

        GroupDetail GroupDetailMapper(Group groupObject)
        {
            GroupDetail groupDetail = new GroupDetail();
            groupDetail.Entity = groupObject;
            return groupDetail;
        }

        GroupInfo GroupInfoMapper(Group groupObject)
        {
            GroupInfo groupInfo = new GroupInfo();
            groupInfo.Name = groupObject.Name;
            groupInfo.Description = groupObject.Description;
            groupInfo.GroupId = groupObject.GroupId;
            return groupInfo;
        }

        #endregion
    }
}
