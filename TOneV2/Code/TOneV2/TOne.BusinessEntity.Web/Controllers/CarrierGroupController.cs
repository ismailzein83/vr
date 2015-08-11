using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CarrierGroupController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public Object GetCarrierAccountsByGroup(Vanrise.Entities.DataRetrievalInput<CarrierGroupQuery> input)
        {
            CarrierGroupManager manager = new CarrierGroupManager();
            return GetWebResponse(input, manager.GetCarrierGroupMembers(input));
        }

        [HttpGet]
        public IEnumerable<CarrierInfo> GetCarrierGroupMembers(int groupId, bool withDescendants)
        {
            CarrierGroupManager manager = new CarrierGroupManager();
            return manager.GetCarriersInfoByGroup(groupId, withDescendants);
        }

        [HttpGet]
        public IEnumerable<CarrierGroupNode> GetEntityNodes()
        {
            CarrierGroupManager manager = new CarrierGroupManager();
            return manager.GetEntityNodes();
        }

        [HttpGet]
        public CarrierGroup GetCarrierGroup(int groupId)
        {
            CarrierGroupManager manager = new CarrierGroupManager();
            return manager.GetCarrierGroup(groupId);
        }

        [HttpPost]
        public InsertOperationOutput<CarrierGroup> AddGroup(GroupEditorInput groupObj)
        {
            CarrierGroupManager manager = new CarrierGroupManager();
            CarrierGroup group = new CarrierGroup()
            {
                Name = groupObj.Name,
                ParentID = groupObj.ParentID
            };

            return manager.AddGroup(group, groupObj.Members);
        }

        [HttpPost]
        public UpdateOperationOutput<CarrierGroup> UpdateGroup(GroupEditorInput groupObj)
        {
            CarrierGroupManager manager = new CarrierGroupManager();
            CarrierGroup group = new CarrierGroup()
            {
                ID = groupObj.ID,
                Name = groupObj.Name,
                ParentID = groupObj.ParentID
            };

            return manager.UpdateGroup(group, groupObj.Members);
        }
    }


    public class GroupEditorInput : CarrierGroup
    {
        public string[] Members { get; set; }
    }
}