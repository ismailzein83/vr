using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    public class RoutingProductController : BaseAPIController
    {
        [HttpPost]
        public object GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<RoutingProductQuery> input)
        {
            //UserManager manager = new UserManager();
            //return GetWebResponse(input, manager.GetFilteredUsers(input));
            return null;
        }
    }
}