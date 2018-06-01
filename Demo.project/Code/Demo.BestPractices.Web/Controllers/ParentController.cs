using Demo.BestPractices.Business;
using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.BestPractices.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Parent")]
    [JSONWithTypeAttribute]
    public class Demo_ParentController : BaseAPIController
    {
        ParentManager parentManager = new ParentManager();
        [HttpPost]
        [Route("GetFilteredParents")]
        public object GetFilteredParents(DataRetrievalInput<ParentQuery> input)
        {
            return GetWebResponse(input, parentManager.GetFilteredParents(input));
        }

        [HttpGet]
        [Route("GetParentById")]
        public Parent GetParentById(long parentId)
        {
            return parentManager.GetParentById(parentId);
        }

        [HttpPost]
        [Route("UpdateParent")]
        public UpdateOperationOutput<ParentDetails> UpdateParent(Parent parent)
        {
            return parentManager.UpdateParent(parent);
        }

        [HttpPost]
        [Route("AddParent")]
        public InsertOperationOutput<ParentDetails> AddParent(Parent parent)
        {
            return parentManager.AddParent(parent);
        }
    }
}