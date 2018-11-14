using Demo.BestPractices.Business;
using Demo.BestPractices.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.BestPractices.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Parent")]
    [JSONWithTypeAttribute]
    public class Demo_ParentController : BaseAPIController
    {
        ParentManager _parentManager = new ParentManager();

        [HttpPost]
        [Route("GetFilteredParents")]
        public object GetFilteredParents(DataRetrievalInput<ParentQuery> input)
        {
            return GetWebResponse(input, _parentManager.GetFilteredParents(input));
        }

        [HttpGet]
        [Route("GetParentById")]
        public Parent GetParentById(long parentId)
        {
            return _parentManager.GetParentById(parentId);
        }

        [HttpGet]
        [Route("GetParentsInfo")]
        public IEnumerable<ParentInfo> GetParentsInfo(string filter = null)
        {
            ParentInfoFilter parentInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<ParentInfoFilter>(filter) : null;
            return _parentManager.GetParentsInfo(parentInfoFilter);
        }

        [HttpPost]
        [Route("AddParent")]
        public InsertOperationOutput<ParentDetail> AddParent(Parent parent)
        {
            return _parentManager.AddParent(parent);
        }

        [HttpPost]
        [Route("UpdateParent")]
        public UpdateOperationOutput<ParentDetail> UpdateParent(Parent parent)
        {
            return _parentManager.UpdateParent(parent);
        }
    }
}