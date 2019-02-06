using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
using System;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TransactionsReport")]
    public class TransactionsReportController : BaseAPIController
    {
        TransactionsReportManager _manager = new TransactionsReportManager();

        [HttpGet]
        [Route("DownloadTransactionsReports")]
        public object DownloadTransactionsReports(long processInstanceId)
        {
            byte[] reports= _manager.DownloadTransactionsReports(processInstanceId);
            return GetExcelResponse(reports, "TransactionsReportsFile.xlsx");
        }

      
    }
}