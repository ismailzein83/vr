using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DemoCity")]
    [JSONWithTypeAttribute]
    public class DemoCityController : BaseAPIController
    {
        DemoCityManager demoCityManager = new DemoCityManager();
       

        [HttpGet]
        [Route("GetDemoCityById")]
        public DemoCity GetDemoCityById(int demoCityId)
        {
            return demoCityManager.GetDemoCityById(demoCityId);
        }

        [HttpGet]
        [Route("GetDemoCitiesInfo")]
        public IEnumerable<DemoCityInfo> DemoCitiesInfo(string filter = null)
        {
            DemoCityInfoFilter DemoCityInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<DemoCityInfoFilter>(filter) : null;
            return demoCityManager.GetDemoCitiesInfo(DemoCityInfoFilter);
        }
        

    }
}