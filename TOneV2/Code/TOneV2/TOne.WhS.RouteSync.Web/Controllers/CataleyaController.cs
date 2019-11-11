using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;
using TOne.WhS.RouteSync.Cataleya.Business;
using TOne.WhS.RouteSync.Cataleya.Entities;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Cataleya")]
    public class CataleyaController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCataleyaDataManagerConfigs")]
        public IEnumerable<CataleyaDataManagerConfig> GetCataleyaDataManagerConfigs()
        {
            CataleyaManager manager = new CataleyaManager();
            return manager.GetCataleyaDataManagerConfigs();
        }
    }
}