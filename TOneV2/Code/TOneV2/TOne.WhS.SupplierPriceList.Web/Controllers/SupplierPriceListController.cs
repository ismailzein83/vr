using Aspose.Cells;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierPriceList")]
    public class WhS_SupplierPriceListController : BaseAPIController
    {
        [HttpGet]
        [Route("DownloadSupplierPriceListTemplate")]
        public object DownloadSupplierPriceListTemplate()
        {
            SupplierPriceListManager manager = new SupplierPriceListManager();
            byte[] bytes = manager.DownloadImportSupplierPriceListTemplate();
            return GetExcelResponse(bytes, "Supplier Price List Template.xls");  
        }
       
        [HttpPost]
        [Route("ConvertPriceList")]
        public object ConvertPriceList(PriceListInput input)
        {
            SupplierPriceListManager manager = new SupplierPriceListManager();
            return manager.ConvertPriceList(input);
        }
    }
}