using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchMapping")]
    [JSONWithTypeAttribute]
    public class SwitchMappingController : BaseAPIController
    {
        SwitchMappingManager _manager = new SwitchMappingManager();

        [HttpPost]
        [Route("GetFilteredSwitchMappings")]
        public object GetFilteredSwitchMappings(Vanrise.Entities.DataRetrievalInput<SwitchMappingQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSwitchMappings(input));
        }

    }
}