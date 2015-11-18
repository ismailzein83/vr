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
    public class SupplierPriceListController:BaseAPIController
    {
        [HttpGet]
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
                    CurrencyId = currencyId
                }

            });
        }
        [HttpGet]
        public HttpResponseMessage DownloadSupplierPriceList()
        {

            var obj = HttpContext.Current.Server.MapPath("/Client/Modules/WhS_SupplierPriceList");
            FileStream fstream = new FileStream(obj+"\\Template\\Supplier Price List Sample.xls", FileMode.Open);
            Workbook workbook = new Workbook(fstream);
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
                FileName = String.Format("TemplateExcelReport.xls")
            };
            fstream.Close();
            return response;
        }
    }
}