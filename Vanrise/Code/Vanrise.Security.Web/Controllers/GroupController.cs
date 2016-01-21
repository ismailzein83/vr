using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Groups")]
    public class GroupController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredGroups")]
        public object GetFilteredGroups(Vanrise.Entities.DataRetrievalInput<GroupQuery> input)
        {
            GroupManager manager = new GroupManager();
            return GetWebResponse(input, manager.GetFilteredGroups(input));
        }

        [HttpGet]
        [Route("GetGroup")]
        public Group GetGroup(int groupId)
        {
            GroupManager manager = new GroupManager();
            return manager.GetGroupbyId(groupId);
        }

        [HttpGet]
        [Route("GetGroups")]
        public IEnumerable<GroupInfo> GetGroups()
        {
            GroupManager manager = new GroupManager();
            return manager.GetGroups();
        }

        [HttpPost]
        [Route("AddGroup")]
        public Vanrise.Entities.InsertOperationOutput<GroupDetail> AddGroup(GroupEditorInput groupObj)
        {
            GroupManager manager = new GroupManager();
            Group group = new Group() 
            { 
                Name = groupObj.Name,
                Description = groupObj.Description
            };

            return manager.AddGroup(group, groupObj.Members);
        }

        [HttpPost]
        [Route("UpdateGroup")]
        public Vanrise.Entities.UpdateOperationOutput<GroupDetail> UpdateGroup(GroupEditorInput groupObj)
        {
            GroupManager manager = new GroupManager();
            Group group = new Group()
            {
                GroupId = groupObj.GroupId,
                Name = groupObj.Name,
                Description = groupObj.Description
            };
            return manager.UpdateGroup(groupObj, groupObj.Members);
        }
    }

    public class GroupEditorInput : Group
    {
        public int[] Members { get; set; }
    }

}