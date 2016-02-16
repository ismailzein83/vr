using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CarrierAccount")]
    public class Demo_CarrierAccountController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCarrierAccounts")]
        public object GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return GetWebResponse(input, manager.GetFilteredCarrierAccounts(input));
        }

        [HttpGet]
        [Route("GetCarrierAccount")]
        public CarrierAccount GetCarrierAccount(int carrierAccountId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccount(carrierAccountId);
        }
        
        [HttpPost]
        [Route("AddCarrierAccount")]
        public Vanrise.Entities.InsertOperationOutput<CarrierAccountDetail> AddCarrierAccount(CarrierAccount carrierAccount)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.AddCarrierAccount(carrierAccount);
        }

        [HttpPost]
        [Route("UpdateCarrierAccount")]
        public Vanrise.Entities.UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.UpdateCarrierAccount(carrierAccount);
        }
       
    }
}