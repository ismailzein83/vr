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
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierPriceList")]
    public class WhS_SupplierPriceListController:BaseAPIController
    {
        [HttpGet]
        [Route("UploadSupplierPriceList")]
        public CreateProcessOutput UploadSupplierPriceList(int supplierAccountId,int currencyId, int fileId, DateTime? effectiveDate)
        {
            BPClient bpClient = new BPClient();
            return bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new SupplierPriceListProcessInput
                {
                    EffectiveDate = effectiveDate,
                    FileId = fileId,
                    SupplierAccountId = supplierAccountId,
                    CurrencyId = currencyId,
                    DeletedCodesDate = DateTime.Now
                }

            });
        }
        [HttpGet]
        [Route("DownloadSupplierPriceListTemplate")]
        public HttpResponseMessage DownloadSupplierPriceListTemplate()
        {
           string obj = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ImportPriceListTemplatePath"]);
            Workbook workbook = new Workbook(obj);
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
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