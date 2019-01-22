using Vanrise.DevTools.Web;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.DevTools.Entities;
using Vanrise.DevTools.Business;

namespace Vanrise.DevTools.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Schema")]
    [JSONWithTypeAttribute]
    public class SchemaController : BaseAPIController
    {
        VRGeneratedScriptSchemaManager schemaManager = new VRGeneratedScriptSchemaManager();


        [HttpGet]
        [Route("GetSchemasInfo")]
        public IEnumerable<VRGeneratedScriptSchemaInfo> GetSchemasInfo(string filter = null)
        {
            VRGeneratedScriptSchemaInfoFilter schemaInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<VRGeneratedScriptSchemaInfoFilter>(filter) : null;
            return schemaManager.GetSchemasInfo(schemaInfoFilter);
        }
    }
}