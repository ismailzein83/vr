﻿using System;
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
            return GetExcelResponse(manager.GenerateMNPReport(filter), string.Format("MNP Report{0}.xls", filter.FromDate.ToString("MMyy")));
        }

        [HttpPost]
        [Route("DownloadTCRReport")]
        public object DownloadTCRReport(TCRRingoReportFilter filter)
        {
            RingoReportSheetManager manager = new RingoReportSheetManager();
            return GetExcelResponse(manager.GenerateTCRReport(filter), string.Format("TCR Report{0}.zip", filter.From.Value.ToString("MMyy")));
        }
    }
}