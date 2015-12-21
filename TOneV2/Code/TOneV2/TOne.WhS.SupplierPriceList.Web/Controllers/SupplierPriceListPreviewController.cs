﻿using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using TOne.WhS.SupplierPriceList.BP.Arguments;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierPriceListPreview")]
    public class WhS_SupplierPriceListPreviewController : BaseAPIController
    {
       [HttpPost]
       [Route("GetFilteredZonePreview")]
       public object GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierZonePreviewManager manager = new SupplierZonePreviewManager();
           return GetWebResponse(input, manager.GetFilteredZonePreview(input));
       }

       [HttpPost]
       [Route("GetFilteredCodePreview")]
       public object GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierCodePreviewManager manager = new SupplierCodePreviewManager();
           return GetWebResponse(input, manager.GetFilteredCodePreview(input));
       }

       [HttpPost]
       [Route("GetFilteredRatePreview")]
       public object GetFilteredRatePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierRatePreviewManager manager = new SupplierRatePreviewManager();
           return GetWebResponse(input, manager.GetFilteredRatePreview(input));
       }
    }
}