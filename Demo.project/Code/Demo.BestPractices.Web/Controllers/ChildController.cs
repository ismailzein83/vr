using Demo.BestPractices.Business;
using Demo.BestPractices.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.BestPractices.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Child")]
    [JSONWithTypeAttribute]
    public class ChildController : BaseAPIController
    {
        ChildManager _childManager = new ChildManager();

        [HttpPost]
        [Route("GetFilteredChilds")]
        public object GetFilteredChilds(DataRetrievalInput<ChildQuery> input)
        {
            return GetWebResponse(input, _childManager.GetFilteredChilds(input));
        }

        [HttpGet]
        [Route("GetChildById")]
        public Child GetChildById(long childId)
        {
            return _childManager.GetChildById(childId);
        }

        [HttpPost]
        [Route("AddChild")]
        public InsertOperationOutput<ChildDetail> AddChild(Child child)
        {
            return _childManager.AddChild(child);
        }

        [HttpPost]
        [Route("UpdateChild")]
        public UpdateOperationOutput<ChildDetail> UpdateChild(Child child)
        {
            return _childManager.UpdateChild(child);
        }

        [HttpGet]
        [Route("GetChildShapeConfigs")]
        public IEnumerable<ChildShapeConfig> GetChildShapeConfigs()
        {
            return _childManager.GetChildShapeConfigs();
        }
    }
}