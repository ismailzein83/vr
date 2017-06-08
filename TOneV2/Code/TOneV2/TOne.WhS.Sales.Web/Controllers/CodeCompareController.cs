using System;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeCompare")]
    public class CodeCompareController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCodeCompare")]
        public object GetFilteredCodeCompare(Vanrise.Entities.DataRetrievalInput<CodeCompareQuery> input)
        {
            CodeCompareManager manager = new CodeCompareManager();
            return GetWebResponse(input, manager.GetFilteredCodeCompare(input));
        }

    }
}