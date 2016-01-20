using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.Security.Business
{
    public class UserGroupManager
    {
        #region Public Methods

        public IEnumerable<int> GetMembers(int groupId)
        {
            IEnumerable<UserGroup> allUserGroupEntities = GetCachedUserGroupEntities();
            return allUserGroupEntities.MapRecords(UserIdsMapper, x => x.GroupId == groupId);
        }

        #endregion

        #region Private Methods

        private IEnumerable<UserGroup> GetCachedUserGroupEntities()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetUserGroup",
               () =>
               {
                   IUserGroupDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserGroupDataManager>();
                   return dataManager.GetAllUserGroupEntities();
               });
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IUserGroupDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IUserGroupDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreUserGroupUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mapper

        private int UserIdsMapper(UserGroup userGroup)
        {
            return userGroup.UserId;
        }

        #endregion
    }
    
}
