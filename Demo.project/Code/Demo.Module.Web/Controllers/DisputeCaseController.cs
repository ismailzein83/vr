using Demo.Module.Business;
using Demo.Module.Entities.DisputeCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DisputeCase")]
    public class DisputeCaseController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredDisputeCases")]
        public object GetFilteredDisputeCases(DataRetrievalInput<DisputeCaseQuery> input)
        {
            DisputeCaseManager buildingManager = new DisputeCaseManager();
            return GetWebResponse(input, buildingManager.GetFilteredDisputeCases(input));
        }

       
    }
}