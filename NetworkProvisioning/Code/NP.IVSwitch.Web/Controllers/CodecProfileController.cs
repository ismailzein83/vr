using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;


namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodecProfile")]
    [JSONWithTypeAttribute]
    public class CodecProfileController : BaseAPIController
    {
        CodecProfileManager _manager = new CodecProfileManager();

        [HttpPost]
        [Route("GetFilteredCodecProfiles")]
        public object GetFilteredCodecProfiles(Vanrise.Entities.DataRetrievalInput<CodecProfileQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredCodecProfiles(input));
        }

        [HttpGet]
        [Route("GetCodecProfile")]
        public CodecProfile GetCodecProfile(int CodecProfileId)
        {
            return _manager.GetCodecProfile(CodecProfileId);
        }

        [HttpPost]
        [Route("AddCodecProfile")]
        public Vanrise.Entities.InsertOperationOutput<CodecProfileDetail> AddCodecProfile(CodecProfile CodecProfileItem)
        {
            return _manager.AddCodecProfile(CodecProfileItem);
        }

        [HttpPost]
        [Route("UpdateCodecProfile")]
        public Vanrise.Entities.UpdateOperationOutput<CodecProfileDetail> UpdateCodecProfile(CodecProfile CodecProfileItem)
        {
            return _manager.UpdateCodecProfile(CodecProfileItem);
        }

        [HttpGet]
        [Route("GetCodecProfileEditorRuntime")]
        public CodecProfileEditorRuntime GetCodecProfileEditorRuntime(int CodecProfileId)
        {
            CodecProfileManager manager = new CodecProfileManager();
            return manager.GetCodecProfileEditorRuntime(CodecProfileId);
        }


    }
}