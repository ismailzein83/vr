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
    [RoutePrefix(Constants.ROUTE_PREFIX + "ProductDefinition")]
    [JSONWithTypeAttribute]
    public class ProductDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetProductDefinitionsInfo")]
        public IEnumerable<ProductDefinitionInfo> GetProductDefinitionsInfo(string serializedFilter = null)
        {
            ProductDefinitionManager manager = new ProductDefinitionManager();
            return manager.GetProductDefinitionsInfo();
        }

        [HttpGet]
        [Route("GetProductDefinitionExtendedSettingsConfigs")]
        public IEnumerable<ProductDefinitionConfig> GetProductDefinitionExtendedSettingsConfigs()
        {
            ProductDefinitionManager manager = new ProductDefinitionManager();
            return manager.GetProductDefinitionExtendedSettingsConfigs();
        }
    }
}