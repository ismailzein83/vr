using Retail.RA.Business;
using Retail.RA.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.RA.Web.Controller
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "OnNetOperatorDirectionServices")]
    public class RAOnNetOperatorDirectionServicesController : BaseAPIController
    {
        [HttpGet]
        [Route("GetMappedCellsExtensionConfigs")]
        public IEnumerable<OnNetOperatorDirectionServicesMappedCellExtensionConfiguration> GetMappedCellsExtensionConfigs()
        {
            var manager = new OnNetOperatorDeclarationServicesManager();
            return manager.GetMappedCellsExtensionConfigs();
        }
    }
}