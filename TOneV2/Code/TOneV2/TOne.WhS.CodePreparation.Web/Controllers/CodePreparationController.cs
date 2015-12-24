using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TOne.WhS.CodePreparation.BP.Arguments;
using TOne.WhS.CodePreparation.Business;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;
using TOne.WhS.CodePreparation.Entities.CP;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodePreparation")]
    public class CodePreparationController : BaseAPIController
    {
        [HttpGet]
        [Route("UploadSaleZonesList")]
        public CreateProcessOutput UploadSaleZonesList(int sellingNumberPlanId, int fileId, DateTime effectiveDate)
        {
            CodePreparationManager manager = new CodePreparationManager();
            BPClient bpClient = new BPClient();
            return bpClient.CreateNewProcess(new CreateProcessInput
             {
                 InputArguments = new CodePreparationProcessInput
                 {
                     EffectiveDate = effectiveDate,
                     FileId = fileId,
                     SellingNumberPlanId = sellingNumberPlanId,
                 }

             });
        }
        [HttpGet]
        [Route("DownloadImportCodePreparationTemplate")]
        public HttpResponseMessage DownloadImportCodePreparationTemplate()
        {
            string obj = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ImportCodePreparationTemplatePath"]);
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

        [HttpGet]
        [Route("GetChanges")]
        public Changes GetChanges(int sellingNumberPlanId)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.GetChanges(sellingNumberPlanId);
        }

        [HttpPost]
        [Route("SaveChanges")]
        public bool SaveChanges(SaveChangesInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.SaveChanges(input);
        }

        [HttpPost]
        [Route("SaveNewZone")]
        public NewZoneOutput SaveNewZone(NewZoneInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.SaveNewZone(input);
        }

        [HttpPost]
        [Route("SaveNewCode")]
        public NewCodeOutput SaveNewCode(NewCodeInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.SaveNewCode(input);
        }

        [HttpPost]
        [Route("MoveCodes")]
        public MoveCodeOutput MoveCodes(MoveCodeInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.MoveCodes(input);
        }

        [HttpPost]
        [Route("CloseCodes")]
        public CloseCodesOutput CloseCodes(CloseCodesInput input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.CloseCodes(input);
        }

        [HttpGet]
        [Route("GetZoneItems")]
        public List<ZoneItem> GetZoneItems(int sellingNumberPlanId, int countryId)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return manager.GetZoneItems(sellingNumberPlanId, countryId);
        }

        [HttpPost]
        [Route("GetCodeItems")]
        public object GetCodeItems(Vanrise.Entities.DataRetrievalInput<GetCodeItemInput> input)
        {
            CodePreparationManager manager = new CodePreparationManager();
            return GetWebResponse(input, manager.GetCodeItems(input));
        }
    }
}