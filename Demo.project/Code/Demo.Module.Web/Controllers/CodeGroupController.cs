using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeGroup")]
    public class Demo_CodeGroupController : BaseAPIController
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
        public Vanrise.Entities.InsertOperationOutput<CodeGroupDetail> AddCodeGroup(CodeGroup codeGroup)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.AddCodeGroup(codeGroup);
        }

        [HttpPost]
        [Route("UpdateCodeGroup")]
        public Vanrise.Entities.UpdateOperationOutput<CodeGroupDetail> UpdateCodeGroup(CodeGroup codeGroup)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.UpdateCodeGroup(codeGroup);
        }

        [HttpGet]
        [Route("UploadCodeGroupList")]
        public UploadCodeGroupLog UploadCodeGroupList(int fileId)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.UploadCodeGroupList(fileId);

        }

        [HttpGet]
        [Route("DownloadCodeGroupListTemplate")]
        public object DownloadCodeGroupListTemplate()
        {
            CodeGroupManager manager = new CodeGroupManager();
            byte[] bytes = manager.DownloadCodeGroupListTemplate();
            return GetExcelResponse(bytes, "Code Group Template.xls");
        }

        [HttpGet]
        [Route("DownloadCodeGroupLog")]
        public object DownloadCodeGroupLog(long fileID)
        {
            CodeGroupManager manager = new CodeGroupManager();
            byte[] bytes = manager.DownloadCodeGroupLog(fileID);
            return GetExcelResponse(bytes, "ImportedCodeGroupResults.xls");
        }
       
    }
}