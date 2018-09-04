using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;
namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntityConfiguration")]
  
    public class BusinessEntityConfigurationController : BaseAPIController
    {


        [HttpGet]
        [Route("GetCodeListResolverSettingsTemplates")]
        public IEnumerable<CodeListResolverConfig> GetCodeListResolverSettingsTemplates()
        {
            BusinessEntityConfigurationManager _manager = new BusinessEntityConfigurationManager();
            return _manager.GetCodeListResolverSettingsTemplates();
        }

        [HttpGet]
        [Route("GetExcludedDestinationsTemplates")]
        public IEnumerable<ExcludedDestinationsConfig> GetExcludedDestinationsTemplates()
        {
            BusinessEntityConfigurationManager _manager = new BusinessEntityConfigurationManager();
            return _manager.GetExcludedDestinationsTemplates();
        }
    }
}