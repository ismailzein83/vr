﻿using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.DevTools.Web;
using Vanrise.DevTools.Entities;
using Vanrise.DevTools.Business;
using Vanrise.Entities;
namespace Vanrise.DevTools.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "TableData")]
    [JSONWithTypeAttribute]
    public class TableDataController : BaseAPIController
    {
        VRGeneratedScriptTableDataManager tableDataManager = new VRGeneratedScriptTableDataManager();


        [HttpPost]
        [Route("GetFilteredTableData")]
        public IEnumerable<VRGeneratedScriptTableDataDetails> GetFilteredTableData(VRGeneratedScriptTableDataQuery query)
        {
            return tableDataManager.GetFilteredTableData(query);

        }

        [HttpPost]
        [Route("GetSelectedTableData")]
        public IEnumerable<GeneratedScriptItemTableRow> GetSelectedTableData(VRGeneratedScriptTableDataQuery query)
        {
            return tableDataManager.GetSelectedTableData(query);
        }


    }
}