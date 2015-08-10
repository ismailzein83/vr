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
        [HttpGet]
        public IEnumerable<CarrierAccount> GetCarrierAccountsByGroup(int groupId)
        {
            CarrierGroupManager manager = new CarrierGroupManager();
            return manager.GetCarrierGroupMembers(groupId,false);
        }

        [HttpGet]
        public IEnumerable<CarrierInfo> GetCarrierGroupMembers(int groupId)
        {
            List<CarrierAccount> lstCarrierAccount = GetCarrierAccountsByGroup(groupId).ToList<CarrierAccount>();
            CarrierGroupManager manager = new CarrierGroupManager();
            return manager.GetCarriersInfoByGroup(lstCarrierAccount);
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
        













        //[HttpGet]
        //public List<CarrierInfo> GetCarriers(CarrierType carrierType)
        //{
        //    CarrierAccountManager manager = new CarrierAccountManager();
        //    return manager.GetCarriers(carrierType);
        //}

        //[HttpPost]
        //public int insertCarrierTest(CarrierInfo carrierInfo)
        //{
        //    CarrierAccountManager manager = new CarrierAccountManager();
        //    return manager.InsertCarrierTest(carrierInfo.CarrierAccountID, carrierInfo.Name);
        //}

        //[HttpPost]
        //public object GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        //{
        //    CarrierAccountManager manager = new CarrierAccountManager();
        //    return GetWebResponse(input, manager.GetFilteredCarrierAccounts(input));
        //}

        //[HttpGet]
        //public List<CarrierAccount> GetCarrierAccounts(string ProfileName, string ProfileCompanyName, int from, int to)
        //{
        //    CarrierAccountManager manager = new CarrierAccountManager();
        //    return manager.GetCarrierAccounts(ProfileName, ProfileCompanyName, from, to);
        //}

        //[HttpGet]
        //public CarrierAccount GetCarrierAccount(string carrierAccountId)
        //{
        //    CarrierAccountManager manager = new CarrierAccountManager();
        //    return manager.GetCarrierAccount(carrierAccountId);
        //}

        //[HttpPost]
        //public int UpdateCarrierAccount(CarrierAccount carrierAccount)
        //{
        //    CarrierAccountManager manager = new CarrierAccountManager();
        //    return manager.UpdateCarrierAccount(carrierAccount);
        //}
    }


    public class GroupEditorInput : CarrierGroup
    {
        public string[] Members { get; set; }
    }
}