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
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeGroup")]
    public class WhSBE_CodeGroupController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCodeGroups")]
        public object GetFilteredCodeGroups(Vanrise.Entities.DataRetrievalInput<CodeGroupQuery> input)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return GetWebResponse(input, manager.GetFilteredCodeGroups(input));
        }

        [HttpGet]
        [Route("GetAllCodeGroups")]
        public IEnumerable<CodeGroup> GetAllCodeGroups()
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.GetAllCodeGroups();
        }
        [HttpGet]
        [Route("GetCodeGroup")]
        public CodeGroup GetCodeGroup(int codeGroupId)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.GetCodeGroup(codeGroupId);
        }

        [HttpPost]
        [Route("AddCodeGroup")]
        public TOne.Entities.InsertOperationOutput<CodeGroupDetail> AddCodeGroup(CodeGroup codeGroup)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.AddCodeGroup(codeGroup);
        }
        [HttpPost]
        [Route("UpdateCodeGroup")]
        public TOne.Entities.UpdateOperationOutput<CodeGroupDetail> UpdateCodeGroup(CodeGroup codeGroup)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.UpdateCodeGroup(codeGroup);
        }

        [HttpGet]
        [Route("UploadCodeGroupList")]
        public HttpResponseMessage UploadCodeGroupList(int fileId)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }
        [HttpGet]
        [Route("DownloadCodeGroupListTemplate")]
        public HttpResponseMessage DownloadCodeGroupListTemplate()
        {
            string obj = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadCodeGroupTemplatePath"]);
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