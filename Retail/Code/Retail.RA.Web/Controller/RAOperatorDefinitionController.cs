using Retail.RA.Business;
using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.RA.Web.Controller
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RAOperatorDeclarationController")]
    public class RAOperatorDefinitionController : BaseAPIController
    {
        //[HttpGet]
        //[Route("GetOperatorDefinitionInfo")]
        //public IEnumerable<OperatorDefinitionInfo> GetOperatorDefinitionInfo()
        //{
        //    var manager = new OperatorDefinitionManager();
        //    return manager.GetOperatorDefinitionInfo();
        //}
    }
}