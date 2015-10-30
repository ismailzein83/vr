using System;
using System.Collections.Generic;
using System.Linq;
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
       
    }
}