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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Group")]
    public class GroupController : Vanrise.Web.Base.BaseAPIController
    {
        GroupManager _manager;
        public GroupController()
        {
            _manager = new GroupManager();
        }

        [HttpPost]
        [Route("GetFilteredGroups")]
        public object GetFilteredGroups(Vanrise.Entities.DataRetrievalInput<GroupQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredGroups(input), "Groups");
        }

        [HttpGet]
        [Route("GetGroupInfo")]
        public IEnumerable<GroupInfo> GetGroupInfo(string filter = null)
        {
            GroupFilter deserializedFilter = Vanrise.Common.Serializer.Deserialize<GroupFilter>(filter);
            return _manager.GetGroupInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetGroup")]
        public Group GetGroup(int groupId)
        {
            return _manager.GetGroup(groupId,true);
        }

        [HttpGet]
        [Route("GetAssignedUserGroups")]
        public List<int> GetAssignedUserGroups(int userId)
        {
            return _manager.GetAssignedUserGroups(userId);
        }

        [HttpGet]
        [Route("GetGroupHistoryDetailbyHistoryId")]
        public Group GetGroupHistoryDetailbyHistoryId(int groupHistoryId)
        {
            return _manager.GetGroupHistoryDetailbyHistoryId(groupHistoryId);
        }

        [HttpPost]
        [Route("AddGroup")]
        public Vanrise.Entities.InsertOperationOutput<GroupDetail> AddGroup(Group groupObj)
        {
            return _manager.AddGroup(groupObj);
        }

        [HttpPost]
        [Route("UpdateGroup")]
        public Vanrise.Entities.UpdateOperationOutput<GroupDetail> UpdateGroup(Group groupObj)
        {
            return _manager.UpdateGroup(groupObj);
        }

        [HttpGet]
        [Route("GetGroupTemplate")]
        public IEnumerable<GroupSettingsConfig> GetGroupTemplate()
        {

            return _manager.GetGroupTemplate();
        }

        [HttpPost]
        [Route("AssignUserToGroup")]
        public Vanrise.Entities.UpdateOperationOutput<UserGroupDetail> AssignUserToGroup(UserGroup userGroup)
        {
            return _manager.AssignUserToGroup(userGroup);
        }
    }

    public class GroupEditorInput : Group
    {
        public int[] Members { get; set; }
    }

}