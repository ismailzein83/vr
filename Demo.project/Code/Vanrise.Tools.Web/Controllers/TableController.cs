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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Table")]
    [JSONWithTypeAttribute]
    public class TableController : BaseAPIController
    {
        TableManager tableManager = new TableManager();
       
       
        [HttpGet]
        [Route("GetTablesInfo")]
        public IEnumerable<TableInfo> GetTablesInfo(string filter = null)
        {
            TableInfoFilter tableInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<TableInfoFilter>(filter) : null;
            return tableManager.GetTablesInfo(tableInfoFilter);
        }
    }
}