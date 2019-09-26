using System;
using NetworkProvision.Business;
using NetworkProvision.Entities;
using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;

namespace NetworkProvision.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "HandlerType")]
    [JSONWithTypeAttribute]
    public class NetworkProvisionHandlerTypeController : BaseAPIController
    {
        //NetworkProvisionHandlerTypeManager manager = new NetworkProvisionHandlerTypeManager();

        //[HttpGet]
        //[Route("GetHandlerTypeExtendedSettingsConfigs")]
        //public IEnumerable<NetworkProvisionHandlerTypeExtendedSettingsConfig> GetHandlerTypeExtendedSettingsConfigs()
        //{
        //    return manager.GetHandlerTypeExtendedSettingsConfigs();
        //}

    }
}