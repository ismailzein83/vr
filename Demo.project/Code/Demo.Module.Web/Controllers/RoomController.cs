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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Room")]
    [JSONWithTypeAttribute]
    public class Demo_Module_RoomController : BaseAPIController
    {
    }
}
