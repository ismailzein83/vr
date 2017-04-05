using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Vanrise.DataParser.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ParserType")]
    [JSONWithTypeAttribute]
    public class ParserTypeController : BaseAPIController
    {
    }
}