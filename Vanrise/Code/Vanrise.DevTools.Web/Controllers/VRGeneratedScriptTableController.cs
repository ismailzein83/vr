using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.DevTools.Web;
using Vanrise.DevTools.Entities;
using Vanrise.DevTools.Business;

namespace Vanrise.DevTools.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Table")]
    [JSONWithTypeAttribute]
    public class TableController : BaseAPIController
    {
        VRGeneratedScriptTableManager tableManager = new VRGeneratedScriptTableManager();


        [HttpGet]
        [Route("GetTablesInfo")]
        public IEnumerable<VRGeneratedScriptTableInfo> GetTablesInfo(string filter = null)
        {
            VRGeneratedScriptTableInfoFilter tableInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<VRGeneratedScriptTableInfoFilter>(filter) : null;
            return tableManager.GetTablesInfo(tableInfoFilter);
        }
    }
}