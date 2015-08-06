using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CarrierAccountController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<CarrierInfo> GetCarriers(CarrierType carrierType)
        {

            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarriers(carrierType);
        }


        [HttpPost]
        public int insertCarrierTest(CarrierInfo carrierInfo)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.InsertCarrierTest(carrierInfo.CarrierAccountID, carrierInfo.Name);
        }

        [HttpPost]
        public object GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return GetWebResponse(input, manager.GetFilteredCarrierAccounts(input));
        }

        [HttpGet]
        public List<CarrierAccount> GetCarrierAccounts(string ProfileName, string ProfileCompanyName, int from, int to)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccounts(ProfileName, ProfileCompanyName, from, to);
        }

        [HttpGet]
        public CarrierAccount GetCarrierAccount(string carrierAccountId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccount(carrierAccountId);
        }

        [HttpPost]
        public int UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.UpdateCarrierAccount(carrierAccount);
        }
    }
}