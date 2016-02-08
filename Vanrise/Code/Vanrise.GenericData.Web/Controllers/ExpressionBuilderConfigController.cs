using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities.ExpressionBuilder;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ExpressionBuilderConfig")]
    public class ExpressionBuilderConfigController:BaseAPIController
    {
        [HttpGet]
        [Route("GetExpressionBuilderTemplates")]
        public IEnumerable<ExpressionBuilderConfig> GetExpressionBuilderTemplates()
        {
            ExpressionBuilderConfigManager manager = new ExpressionBuilderConfigManager();
            return manager.GetExpressionBuilderTemplates();
        }
    }
}