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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Child")]
    [JSONWithTypeAttribute]
    public class ChildController:BaseAPIController
    {
        ChildManager childManager = new ChildManager();
        [HttpPost]
        [Route("GetFilteredChilds")]
        public object GetFilteredChilds(DataRetrievalInput<ChildQuery> input)
        {
            return GetWebResponse(input, childManager.GetFilteredChilds(input));
        }

        [HttpGet]
        [Route("GetChildById")]
        public Child GetChildById(long childId)
        {
            return childManager.GetChildById(childId);
        }

        [HttpPost]
        [Route("UpdateChild")]
        public UpdateOperationOutput<ChildDetails> UpdateChild(Child child)
        {
            return childManager.UpdateChild(child);
        }

        [HttpPost]
        [Route("AddChild")]
        public InsertOperationOutput<ChildDetails> AddChild(Child child)
        {
            return childManager.AddChild(child);
        }
    }
}