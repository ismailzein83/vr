using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class GroupManager
    {

        private Dictionary<int, Group> GetCachedGroups()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGroups",
               () =>
               {
                   IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
                   IEnumerable<Group> groups = dataManager.GetGroups();
                   return groups.ToDictionary(kvp => kvp.GroupId, kvp => kvp);
               });
        }

        public IDataRetrievalResult<GroupDetail> GetFilteredGroups(DataRetrievalInput<GroupQuery> input)
        {
            var allItems = GetCachedGroups();

            Func<Group, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, GroupDetailMapper));
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IUserDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreUsersUpdated(ref _updateHandle);
            }
        }


        private GroupDetail GroupDetailMapper(Group groupObject)
        {
            GroupDetail groupDetail = new GroupDetail();
            groupDetail.Entity = groupObject;
            return groupDetail;
        }


        private GroupInfo GroupInfoMapper(Group groupObject)
        {
            GroupInfo groupInfo = new GroupInfo();
            groupInfo.Name = groupObject.Name;
            groupInfo.Description = groupObject.Description;
            groupInfo.GroupId = groupObject.GroupId;
            return groupInfo;
        }

        public IEnumerable<GroupInfo> GetGroups()
        {
            var groups = GetCachedGroups();
            return groups.MapRecords(GroupInfoMapper);
        }

        public Group GetGroupbyId(int groupId)
        {
            var groups = GetCachedGroups();
            return groups.GetRecord(groupId);
        }

        public Vanrise.Entities.InsertOperationOutput<GroupDetail> AddGroup(Group groupObj, int[] members)
        {
            InsertOperationOutput<GroupDetail> insertOperationOutput = new InsertOperationOutput<GroupDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int groupId = -1;

            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            bool insertActionSucc = dataManager.AddGroup(groupObj, out groupId);

            if (insertActionSucc)
            {
                dataManager.AssignMembers(groupId, members);

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


        public Vanrise.Entities.UpdateOperationOutput<GroupDetail> UpdateGroup(Group groupObj, int[] members)
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            bool updateActionSucc = dataManager.UpdateGroup(groupObj);
            UpdateOperationOutput<GroupDetail> updateOperationOutput = new UpdateOperationOutput<GroupDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                dataManager.AssignMembers(groupObj.GroupId, members);

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GroupDetailMapper(groupObj);
            }
            return updateOperationOutput;
        }


        public List<int> GetUserGroups(int userId)
        {
            IGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IGroupDataManager>();
            return dataManager.GetUserGroups(userId);
        }



    }
}
