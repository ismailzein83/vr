using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ExcludedItemsController")]
    public class ExcludedItemsController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredExcludedItems")]
        public object GetFilteredExcludedItems(Vanrise.Entities.DataRetrievalInput<ExcludedItemsQuery> input)
        {
            ExcludedItemsManager manager = new ExcludedItemsManager();
            return GetWebResponse(input, manager.GetFilteredExcludedItems(input), "Excluded Items");
        }
    }
}