﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CarrierAccount")]
    public class WhSBE_CarrierAccountController : BaseAPIController
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
        public CarrierAccountDetail GetCarrierAccount(int carrierAccountId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccount(carrierAccountId);
        }
        [HttpGet]
        [Route("GetCarrierAccountsInfo")]
        public IEnumerable<CarrierAccountInfo> GetCarrierAccountsInfo(bool getCustomers, bool getSuppliers)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccounts(getCustomers, getSuppliers);
        }

        [HttpGet]
        [Route("GetSupplierGroupTemplates")]
        public List<TemplateConfig> GetSupplierGroupTemplates()
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetSupplierGroupTemplates();
        }

        [HttpGet]
        [Route("GetCustomerGroupTemplates")]
        public List<TemplateConfig> GetCustomerGroupTemplates()
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCustomerGroupTemplates();
        }


        [HttpPost]
        [Route("AddCarrierAccount")]
        public TOne.Entities.InsertOperationOutput<CarrierAccountDetail> AddCarrierAccount(CarrierAccount carrierAccount)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.AddCarrierAccount(carrierAccount);
        }


        [HttpPost]
        [Route("UpdateCarrierAccount")]
        public TOne.Entities.UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.UpdateCarrierAccount(carrierAccount);
        }
    }
}