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
    [RoutePrefix(Constants.ROUTE_PREFIX + "TableData")]
    [JSONWithTypeAttribute]
    public class TableDataController : BaseAPIController
    {
        TableDataManager tableDataManager = new TableDataManager();


        [HttpPost]
        [Route("GetFilteredTableData")]
        public object GetFilteredTableData(DataRetrievalInput<TableDataQuery> input)
        {
            return GetWebResponse(input, tableDataManager.GetFilteredTableData(input));

        }

        [HttpPost]
        [Route("GetSelectedTableData")]
        public IEnumerable<GeneratedScriptItemTableRow> GetSelectedTableData(TableDataQuery query)
        {
            return tableDataManager.GetSelectedTableData(query);
        }


    }
}