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
            Func<Group, bool> filterExpression = (itemObject) =>
            {
                if (input.Query.Name != null && !itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };

            VRActionLogger.Current.LogGetFilteredAction(GroupLoggableEntity.Instance, input);
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

        public Group GetGroup(int groupId, bool isViewedFromUI)
        {
            var groups = GetCachedGroups();
            var group = groups.GetRecord(groupId);
            if (group != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(GroupLoggableEntity.Instance, group);
            return group;
        }

        public Group GetGroupHistoryDetailbyHistoryId(int groupHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var group = s_vrObjectTrackingManager.GetObjectDetailById(groupHistoryId);
            return group.CastWithValidate<Group>("Group : historyId ", groupHistoryId);
        }
        public Group GetGroup(int groupId)
        {

            return GetGroup(groupId, false);
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
                groupObj.GroupId = groupId;
                VRActionLogger.Current.TrackAndLogObjectAdded(GroupLoggableEntity.Instance, groupObj);
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(GroupLoggableEntity.Instance, groupObj);
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
                if (group.Settings != null && group.Settings.IsMember(new GroupSettingsContext() { UserId = userId }))
                    assignedGroups.Add(group.GroupId);
            }

            return assignedGroups;
        }

        public bool IsUserMemberInGroups(int userId, List<int> groups)
        {
            IEnumerable<Group> selectedGroups = this.GetCachedGroups().FindAllRecords(x => groups.Contains(x.GroupId));

            foreach (var group in selectedGroups)
            {
                if (group.Settings != null && group.Settings.IsMember(new GroupSettingsContext() { UserId = userId }))
                    return true;
            }

            return false;
        }

        public IEnumerable<GroupSettingsConfig> GetGroupTemplate()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<GroupSettingsConfig>(GroupSettingsConfig.EXTENSION_TYPE);
        }

        public UpdateOperationOutput<UserGroupDetail> AssignUserToGroup(UserGroup userGroup)
        {
            UpdateOperationOutput<UserGroupDetail> updateOperationOutput = new UpdateOperationOutput<UserGroupDetail>();

            if (userGroup == null)
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = "No data received for user and group";
                return updateOperationOutput;
            }

            var cachedGroups = this.GetCachedGroups();
            Group group = cachedGroups.GetRecord(userGroup.GroupId);
            if (group == null)
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = string.Format("Group {0} doesn't exist", userGroup.GroupId);
                return updateOperationOutput;
            }

            bool addUserToGroupResult = group.Settings.TryAddUser(new TryAddUserGroupSettingsContext() { UserId = userGroup.UserId });

            if (addUserToGroupResult)
            {
                UpdateGroup(group);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = new UserGroupDetail() { GroupId = userGroup.GroupId, UserId = userGroup.UserId };
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = string.Format("User Id {0} cannot be assigned to the Group Id {1}", userGroup.UserId, userGroup.GroupId);
                updateOperationOutput.ShowExactMessage = true;
            }
            return updateOperationOutput;
        }


        public UpdateOperationOutput<UserGroupDetail> UnAssignUserToGroup(UserGroup userGroup)
        {
            UpdateOperationOutput<UserGroupDetail> updateOperationOutput = new UpdateOperationOutput<UserGroupDetail>();

            if (userGroup == null)
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = "No data received for user and group";
                return updateOperationOutput;
            }

            var cachedGroups = this.GetCachedGroups();
            Group group = cachedGroups.GetRecord(userGroup.GroupId);
            if (group == null)
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = string.Format("Group {0} doesn't exist", userGroup.GroupId);
                return updateOperationOutput;
            }

            bool removeUserToGroupResult = group.Settings.TryRemoveUser(new TryAddUserGroupSettingsContext() { UserId = userGroup.UserId });

            if (removeUserToGroupResult)
            {
                UpdateGroup(group);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = new UserGroupDetail() { GroupId = userGroup.GroupId, UserId = userGroup.UserId };
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = string.Format("User Id {0} cannot be unassigned from the Group Id {1}", userGroup.UserId, userGroup.GroupId);
                updateOperationOutput.ShowExactMessage = true;
            }
            return updateOperationOutput;
        }

        public List<int> GetAssignedUserGroups(int userId)
        {
            var cachedGroups = this.GetCachedGroups();
            if (cachedGroups == null)
                return null;

            List<int> assignedUserGroups = new List<int>();
            GroupSettingsContext context = new GroupSettingsContext() { UserId = userId };
            foreach (var groupItem in cachedGroups)
            {
                Group group = groupItem.Value;
                if (group.Settings != null && group.Settings.IsMember(context))
                    assignedUserGroups.Add(groupItem.Key);
            }
            return assignedUserGroups.Count > 0 ? assignedUserGroups : null;
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

        private class GroupLoggableEntity : VRLoggableEntityBase
        {
            public static GroupLoggableEntity Instance = new GroupLoggableEntity();

            private GroupLoggableEntity()
            {

            }

            static GroupManager s_groupManager = new GroupManager();

            public override string EntityUniqueName
            {
                get { return "VR_Security_Group"; }
            }

            public override string ModuleName
            {
                get { return "Security"; }
            }

            public override string EntityDisplayName
            {
                get { return "Group"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Security_Group_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Group group = context.Object.CastWithValidate<Group>("context.Object");
                return group.GroupId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Group group = context.Object.CastWithValidate<Group>("context.Object");
                return s_groupManager.GetGroupName(group.GroupId);
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