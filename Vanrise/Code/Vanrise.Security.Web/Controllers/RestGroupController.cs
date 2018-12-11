using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RestGroup")]
    public class RestGroupController : Vanrise.Web.Base.BaseAPIController
    {
        RestGroupManager _manager;
        public RestGroupController()
        {
            _manager = new RestGroupManager();
        }

        [HttpGet]
        [Route("GetRemoteGroupInfo")]
        public IEnumerable<GroupInfo> GetRemoteGroupInfo(Guid connectionId,string filter = null)
        {
            GroupFilter deserializedFilter = Vanrise.Common.Serializer.Deserialize<GroupFilter>(filter);
            return _manager.GetRemoteGroupInfo(connectionId,deserializedFilter);
        }
        [HttpGet]
        [Route("GetRemoteAssignedUserGroups")]
        public List<int> GetRemoteAssignedUserGroups(Guid connectionId, int userId)
        {
            return _manager.GetRemoteAssignedUserGroups(connectionId, userId);
        }
    }

}