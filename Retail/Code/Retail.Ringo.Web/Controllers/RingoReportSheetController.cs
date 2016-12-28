using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Retail.Ringo.Business;
using Retail.Ringo.Entities;
using Vanrise.Web.Base;

namespace Retail.Ringo.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RingoReportSheet")]
    public class RingoReportSheetController : BaseAPIController
    {
        [HttpPost]
        [Route("DownloadMNPReport")]
        public object DownloadMNPReport(RingoReportFilter filter)
        {
            RingoReportSheetManager manager = new RingoReportSheetManager();
            return GetExcelResponse(manager.GenerateMNPReport(filter), "MNP Report.xls");
        }
    }
}