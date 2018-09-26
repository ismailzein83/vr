using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.NumberingPlan.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeGroup")]
    public class NP_CodeGroupController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCodeGroups")]
        public object GetFilteredCodeGroups(Vanrise.Entities.DataRetrievalInput<CodeGroupQuery> input)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return GetWebResponse(input, manager.GetFilteredCodeGroups(input), "Code Groups");
        }

        [HttpGet]
        [Route("GetAllCodeGroups")]
        public IEnumerable<CodeGroupInfo> GetAllCodeGroups()
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
        public InsertOperationOutput<CodeGroupDetail> AddCodeGroup(CodeGroup codeGroup)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return manager.AddCodeGroup(codeGroup);
        }
      
        [HttpPost]
        [Route("UpdateCodeGroup")]
        public UpdateOperationOutput<CodeGroupDetail> UpdateCodeGroup(CodeGroupToEdit codeGroup)
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