using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using QM.BusinessEntity.Web;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace QM.BusinessEntity.Web.Controllers
{
   
    [RoutePrefix(Constants.ROUTE_PREFIX + "Zone")]

    [JSONWithTypeAttribute]
    public class QMBE_ZoneController : BaseAPIController
    {
       
        [HttpGet]
        [Route("GetZoneSourceTemplates")]
        public List<TemplateConfig> GetZoneSourceTemplates()
        {
            ZoneManager manager = new ZoneManager();
            return manager.GetZoneSourceTemplates();
        }

    }
}