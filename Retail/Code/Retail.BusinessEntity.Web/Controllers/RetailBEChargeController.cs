using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailBECharge")]
    public class RetailBEChargeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRetailBEChargeSettingsConfigs")]
        public IEnumerable<RetailBEChargeSettingsConfig> GetRetailBEChargeSettingsConfigs()
        {
            RetailBEChargeManager _manager = new RetailBEChargeManager();
            return _manager.GetRetailBEChargeSettingsConfigs();
        }
    }
}