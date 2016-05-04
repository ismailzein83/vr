using Aspose.Cells;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierPriceList")]
    public class WhS_SupplierPriceListController : BaseAPIController
    {
        [HttpGet]
        [Route("DownloadSupplierPriceListTemplate")]
        public HttpResponseMessage DownloadSupplierPriceListTemplate()
        {
            string obj = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ImportPriceListTemplatePath"]);
            Workbook workbook = new Workbook(obj);
            Vanrise.Common.Utilities.ActivateAspose();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = workbook.SaveToStream();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            memoryStream.Position = 0;
            response.Content = new StreamContent(memoryStream);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format("ImportPriceListTemplate.xls")
            };
            return response;
        }
    }
}