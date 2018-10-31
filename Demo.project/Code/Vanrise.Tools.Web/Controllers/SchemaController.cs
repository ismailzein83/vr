using Vanrise.Tools.Business;
using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Vanrise.Tools.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Schema")]
    [JSONWithTypeAttribute]
    public class SchemaController : BaseAPIController
    {
        SchemaManager schemaManager = new SchemaManager();
       
       
        [HttpGet]
        [Route("GetSchemasInfo")]
        public IEnumerable<SchemaInfo> GetSchemasInfo(string filter = null)
        {
            SchemaInfoFilter schemaInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<SchemaInfoFilter>(filter) : null;
            return schemaManager.GetSchemasInfo(schemaInfoFilter);
        }
        

    }
}