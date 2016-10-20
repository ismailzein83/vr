using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Deal")]
    public class DealController : Vanrise.Web.Base.BaseAPIController
    {
        //DealManager _manager = new DealManager();
        //[HttpPost]
        [Route("AddDeal")]
        public Vanrise.Entities.InsertOperationOutput<TOne.WhS.Deal.Entities.Deal> AddDeal(TOne.WhS.Deal.Entities.Deal deal)
        {
            return null;
        }
    }
}