using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Manufactory")]
    [JSONWithTypeAttribute]
    public class ManufactoryController : BaseAPIController
    {
        ManufactoryManager _manufactoryManager = new ManufactoryManager();

        [HttpPost]
        [Route("GetFilteredManufactories")]
        public object GetFilteredManufactories(DataRetrievalInput<ManufactoryQuery> input)
        {
            return GetWebResponse(input, _manufactoryManager.GetFilteredManufactories(input));
        }

        [HttpGet]
        [Route("GetManufactoryById")]
        public Manufactory GetManufactoryById(int manufactoryId)
        {
            return _manufactoryManager.GetManufactoryById(manufactoryId);
        }

        [HttpPost]
        [Route("InsertManufactory")]
        public InsertOperationOutput<ManufactoryDetail> AddManufactory(Manufactory manufactory)
        {
            return _manufactoryManager.InsertManufactory(manufactory);
        }

        [HttpPost]
        [Route("UpdateManufactory")]
        public UpdateOperationOutput<ManufactoryDetail> UpdateManufactory(Manufactory manufactory)
        {
            return _manufactoryManager.UpdateManufactory(manufactory);
        }

        [HttpGet]
        [Route("GetManufactoriesInfo")]
        public IEnumerable<ManufactoryInfo> GetManufactoriesInfo(string filter = null)
        {
            ManufactoryInfoFilter manufactoryInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<ManufactoryInfoFilter>(filter) : null;
            return _manufactoryManager.GetManufactoriesInfo(manufactoryInfoFilter);
        }
    }
}