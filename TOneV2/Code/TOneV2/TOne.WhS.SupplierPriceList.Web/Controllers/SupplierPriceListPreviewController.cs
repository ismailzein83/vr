using Aspose.Cells;
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
       [Route("GetFilteredCountryPreview")]
       public object GetFilteredCountryPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierCountryPreviewManager manager = new SupplierCountryPreviewManager();
           return GetWebResponse(input, manager.GetFilteredCountryPreview(input), "Supplier Country Preview");
       }

       [HttpPost]
       [Route("GetFilteredZonePreview")]
       public object GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierZonePreviewManager manager = new SupplierZonePreviewManager();
           return GetWebResponse(input, manager.GetFilteredZonePreview(input),"Supplier Zone Preview");
       }

       [HttpPost]
       [Route("GetFilteredCodePreview")]
       public object GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierCodePreviewManager manager = new SupplierCodePreviewManager();
           return GetWebResponse(input, manager.GetFilteredCodePreview(input),"Supplier Code Preview");
       }

       [HttpPost]
       [Route("GetFilteredOtherRatePreview")]
       public object GetFilteredOtherRatePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierOtherRatesPreviewManager manager = new SupplierOtherRatesPreviewManager();
           return GetWebResponse(input, manager.GetFilteredOtherRatesPreview(input),"Supplier Code Preview");
       }

       [HttpPost]
       [Route("GetFilteredZoneServicesPreview")]
       public object GetFilteredZoneServicesPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
       {
           SupplierZonesServicesPreviewManager manager = new SupplierZonesServicesPreviewManager();
           return GetWebResponse(input, manager.GetFilteredZoneServicesPreview(input), "Supplier Zone Services Preview");
       }

       [HttpGet]
       [Route("GetSupplierPricelistPreviewSummary")]
       public PreviewSummary GetSupplierPricelistPreviewSummary(int processInstanceId)
       {
           SupplierPricelistPreviewSummaryManager manager = new SupplierPricelistPreviewSummaryManager();
           return manager.GetSupplierPricelistPreviewSummary(processInstanceId);
       }


    }
}