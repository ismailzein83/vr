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
            return GetExcelResponse(bytes, "CodeGroupListTemplate.xls");  
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