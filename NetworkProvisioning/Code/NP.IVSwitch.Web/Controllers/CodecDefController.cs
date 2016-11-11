﻿using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
  

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodecDef")]
    [JSONWithTypeAttribute]
    public class CodecDefController : BaseAPIController
    {
        CodecDefManager _manager = new CodecDefManager();
              
        [HttpGet]
        [Route("GetCodecDef")]
        public CodecDef GetCodecDef(int CodecDefId)
        {
            return _manager.GetCodecDef(CodecDefId);
        }

        [HttpGet]
        [Route("GetCodecDefsInfo")]
        public IEnumerable<CodecDefInfo> GetCodecDefsInfo(string filter = null)
        {
            CodecDefFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<CodecDefFilter>(filter) : null;
            return _manager.GetCodecDefsInfo(deserializedFilter);
        }

        [HttpPost]
        [Route("GetCodecDefList")]
        public IEnumerable<CodecDef> GetCodecDefList(List<int> input)
        {
            return _manager.GetCodecDefList(input);
        }

       

    }
}