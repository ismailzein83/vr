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
        ProductDefinitionManager manager = new ProductDefinitionManager();

        [HttpGet]
        [Route("GetProductDefinitionsInfo")]
        public IEnumerable<ProductDefinitionInfo> GetProductDefinitionsInfo(string serializedFilter = null)
        {
            ProductDefinitionFilter deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<ProductDefinitionFilter>(serializedFilter) : null;
            return manager.GetProductDefinitionsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetProductDefinitionExtendedSettingsConfigs")]
        public IEnumerable<ProductDefinitionConfig> GetProductDefinitionExtendedSettingsConfigs()
        {
            return manager.GetProductDefinitionExtendedSettingsConfigs();
        }
    }
}