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
    public class RestGroupManager
    {
        #region Public Methods

        public IEnumerable<GroupInfo> GetRemoteGroupInfo(Guid connectionId, GroupFilter filter)
        {

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRemoteGroupInfo",
         () =>
         {
             VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
             string serializedFilter = null;
             if (filter != null)
             {
                 serializedFilter = Vanrise.Common.Serializer.Serialize(filter);
             }


         return connectionSettings.Get<IEnumerable<GroupInfo>>(string.Format("/api/VR_Sec/Group/GetGroupInfo?filter={0}", serializedFilter));
         });
        }

        public List<int> GetRemoteAssignedUserGroups(Guid connectionId,int userId)
        {
            VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
            return connectionSettings.Get<List<int>>(string.Format("/api/VR_Sec/Group/GetAssignedUserGroups?userId={0}",userId));

        }
        #endregion

        #region Private Methods

        private VRInterAppRestConnection GetVRInterAppRestConnection(Guid connectionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get { return true; }
            }
        }
        #endregion
    }
   
}