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
            string physicalFilePath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ImportPriceListTemplatePath"]);
            byte[] bytes = File.ReadAllBytes(physicalFilePath);
            return GetExcelResponse(bytes, "Supplier Price List Template.xls");  
        }

        [HttpPost]
        [Route("ValidateSupplierPriceList")]
        public bool ValidateSupplierPriceList(SupplierPriceListInput input)
        {
            SupplierPriceListManager manager = new SupplierPriceListManager();
            return manager.ValidateSupplierPriceList(input);
        }
    }
}