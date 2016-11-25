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
    [RoutePrefix(Constants.ROUTE_PREFIX + "EndPoint")]
    [JSONWithTypeAttribute]
    public class EndPointController : BaseAPIController
    {
        EndPointManager _manager = new EndPointManager();

        [HttpPost]
        [Route("GetFilteredEndPoints")]
        public object GetFilteredEndPoints(Vanrise.Entities.DataRetrievalInput<EndPointQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredEndPoints(input));
        }

        [HttpGet]
        [Route("GetEndPoint")]
        public EndPoint GetEndPoint(int endPointId)
        {
            return _manager.GetEndPoint(endPointId);
        }

        [HttpPost]
        [Route("AddEndPoint")]
        public Vanrise.Entities.InsertOperationOutput<EndPointDetail> AddEndPoint(EndPointToAdd endPointItem)
        {
            return _manager.AddEndPoint(endPointItem);
        }

        [HttpPost]
        [Route("UpdateEndPoint")]
        public Vanrise.Entities.UpdateOperationOutput<EndPointDetail> UpdateEndPoint(EndPointToAdd endPointItem)
        {
            return _manager.UpdateEndPoint(endPointItem);
        }
    }
}