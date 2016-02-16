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
        
        [HttpGet]
        [Route("GetCarrierAccountInfo")]
        public IEnumerable<CarrierAccountInfo> GetCarrierAccountInfo(string serializedFilter)
        {
            return null;
            //CarrierAccountInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<CarrierAccountInfoFilter>(serializedFilter) : null;
            //CarrierAccountManager manager = new CarrierAccountManager();
            //return manager.GetCarrierAccountInfo(filter);
        }

        [HttpGet]
        [Route("GetSupplierGroupTemplates")]
        public List<TemplateConfig> GetSupplierGroupTemplates()
        {
            return null;
            //CarrierAccountManager manager = new CarrierAccountManager();
            //return manager.GetSupplierGroupTemplates();
        }

        [HttpGet]
        [Route("GetCustomerGroupTemplates")]
        public List<TemplateConfig> GetCustomerGroupTemplates()
        {
            return null;
            //CarrierAccountManager manager = new CarrierAccountManager();
            //return manager.GetCustomerGroupTemplates();
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
        public Vanrise.Entities.InsertOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            return null;
            //CarrierAccountManager manager = new CarrierAccountManager();
            //return manager.UpdateCarrierAccount(carrierAccount);
        }
        [HttpGet]
        [Route("GetSuppliersWithZonesGroupsTemplates")]
        public List<TemplateConfig> GetSuppliersWithZonesGroupsTemplates()
        {
            return null;
            //CarrierAccountManager manager = new CarrierAccountManager();
            //return manager.GetSuppliersWithZonesGroupsTemplates();
        }
    }
}