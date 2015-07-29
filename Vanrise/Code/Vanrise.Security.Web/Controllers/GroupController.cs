using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class GroupController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredGroups(Vanrise.Entities.DataRetrievalInput<GroupQuery> input)
        {
            GroupManager manager = new GroupManager();
            return GetWebResponse(input, manager.GetFilteredGroups(input));
        }

        [HttpGet]
        public Group GetGroup(int groupId)
        {
            GroupManager manager = new GroupManager();
            return manager.GetGroup(groupId);
        }

        [HttpGet]
        public List<Group> GetGroups()
        {
            GroupManager manager = new GroupManager();
            return manager.GetGroups();
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<Group> AddGroup(GroupEditorInput groupObj)
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
        public Vanrise.Entities.UpdateOperationOutput<Group> UpdateGroup(GroupEditorInput groupObj)
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