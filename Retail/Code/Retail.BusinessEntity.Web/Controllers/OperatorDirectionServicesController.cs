using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorDirectionServices")]
    public class OperatorDirectionServicesController : BaseAPIController
    {
        [HttpGet]
        [Route("GetMappedCellsExtensionConfigs")]
        public IEnumerable<OperatorDirectionServicesMappedCellExtensionConfiguration> GetMappedCellsExtensionConfigs()
        {
            var manager = new OperatorDeclarationServicesManager();
            return manager.GetMappedCellsExtensionConfigs();
        }
    }
}