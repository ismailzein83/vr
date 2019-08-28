using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Business;
using Vanrise.Entities;
using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Zoo") ]
    [JSONWithTypeAttribute]
    public class ZooController : BaseAPIController
    {
        ZooManager _zooManager = new ZooManager();

        [HttpPost]
        [Route("GetFilteredZoos")]
        public object GetFilteredZoos(DataRetrievalInput<ZooQuery> input)
        {
            return GetWebResponse(input, _zooManager.GetFilteredZoos(input));
        }

        [HttpGet]
        [Route("GetZooById")]
        public object GetZooById(long zooId)
        {
            return _zooManager.GetZooById(zooId);
        }

        [HttpGet]
        [Route("GetZoosInfo")]
        public IEnumerable<ZooInfo> GetZoosInfo()
        {
            return _zooManager.GetZoosInfo();
        }

        [HttpPost]
        [Route("AddZoo")]
        public InsertOperationOutput<ZooDetail> AddZoo(Zoo zoo)
        {
            return _zooManager.AddZoo(zoo);
        }

        [HttpPost]
        [Route("UpdateZoo")]
        public UpdateOperationOutput<ZooDetail> UpdateZoo(Zoo zoo)
        {
            return _zooManager.UpdateZoo(zoo);
        }

    }
}