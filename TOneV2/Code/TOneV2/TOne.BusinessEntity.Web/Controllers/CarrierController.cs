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
    public class CarrierController : ApiController
    {
        [HttpGet]
        public List<CarrierInfo> GetCarriers(CarrierType carrierType)
        {

            CarrierManager manager = new CarrierManager();
            return manager.GetCarriers(carrierType);
        }

        [HttpPost]
        public int insertCarrierTest(CarrierInfo carrierInfo)
        {
            CarrierManager manager = new CarrierManager();
            return manager.InsertCarrierTest(carrierInfo.CarrierAccountID, carrierInfo.Name);
        }

        [HttpGet]
        public List<CarrierAccount> GetCarrierAccounts(string ProfileName, string ProfileCompanyName, int from, int to)
        {
            CarrierManager manager = new CarrierManager();
            return manager.GetCarrierAccounts(ProfileName, ProfileCompanyName, from, to);
        }

        [HttpGet]
        public CarrierAccount GetCarrierAccount(string carrierAccountId)
        {
            CarrierManager manager = new CarrierManager();
            return manager.GetCarrierAccount(carrierAccountId);
        }

        [HttpPost]
        public int UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            CarrierManager manager = new CarrierManager();
            return manager.UpdateCarrierAccount(carrierAccount);
        }
    }
}