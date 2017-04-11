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
        [Route("GetCarrierAccountHistoryDetailbyHistoryId")]
        public CarrierAccount GetCarrierAccountHistoryDetailbyHistoryId(int carrierAccountHistoryId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccountHistoryDetailbyHistoryId(carrierAccountHistoryId);
        }
        [HttpGet]
        [Route("GetCarrierAccount")]
        public CarrierAccount GetCarrierAccount(int carrierAccountId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccount(carrierAccountId);
        }

        [HttpGet]
        [Route("GetCarrierAccountCurrencyId")]
        public int GetCarrierAccountCurrencyId(int carrierAccountId)
        {
            var manager = new CarrierAccountManager();
            return manager.GetCarrierAccountCurrencyId(carrierAccountId);
        }

        [HttpGet]
        [Route("GetCustomersBySellingNumberPlanId")]
        public IEnumerable<CarrierAccountInfo> GetCustomersBySellingNumberPlanId(int sellingNumberPlanId)
        {
            var manager = new CarrierAccountManager();
            return manager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId);
        }


        [HttpGet]
        [Route("GetCarrierAccountInfo")]
        public IEnumerable<CarrierAccountInfo> GetCarrierAccountInfo(string serializedFilter)
        {
            CarrierAccountInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<CarrierAccountInfoFilter>(serializedFilter) : null;
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccountInfo(filter);
        }

        [HttpGet]
        [Route("GetSupplierGroupTemplates")]
        public IEnumerable<SupplierGroupConfig> GetSupplierGroupTemplates()
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetSupplierGroupTemplates();
        }

        [HttpGet]
        [Route("GetCustomerGroupTemplates")]
        public IEnumerable<CustomerGroupConfig> GetCustomerGroupTemplates()
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCustomerGroupTemplates();
        }

        [HttpPost]
        [Route("AddCarrierAccount")]
        public InsertOperationOutput<CarrierAccountDetail> AddCarrierAccount(CarrierAccount carrierAccount)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.AddCarrierAccount(carrierAccount);
        }

        [HttpPost]
        [Route("UpdateCarrierAccount")]
        public UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccountToEdit carrierAccount)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.UpdateCarrierAccount(carrierAccount);
        }
        
        [HttpGet]
        [Route("GetSuppliersWithZonesGroupsTemplates")]
        public IEnumerable<SuppliersWithZonesGroupSettingsConfig> GetSuppliersWithZonesGroupsTemplates()
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetSuppliersWithZonesGroupsTemplates();
        }
        [HttpGet]
        [Route("GetCarrierAccountName")]
        public string GetCarrierAccountName(int carrierAccountId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccountName(carrierAccountId);
        }
        
    }
}