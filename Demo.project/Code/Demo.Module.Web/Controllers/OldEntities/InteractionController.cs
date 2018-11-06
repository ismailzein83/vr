using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Interaction")]
    [JSONWithTypeAttribute]
    public class InteractionController : BaseAPIController
    {
        InteractionManager interactionManager = new InteractionManager();

        [HttpGet]
        [Route("GetMessages")]
        public List<Interaction> GetMessages()
        {
            return interactionManager.GetMessages();
        }
    }
}
