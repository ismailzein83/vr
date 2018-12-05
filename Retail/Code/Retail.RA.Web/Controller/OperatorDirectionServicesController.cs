using System.Web.Http;
using Vanrise.Web.Base;
using System.Collections.Generic;
using Retail.RA.Business;
using Retail.RA.Entities;

namespace Retail.RA.Web.Controller
{
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