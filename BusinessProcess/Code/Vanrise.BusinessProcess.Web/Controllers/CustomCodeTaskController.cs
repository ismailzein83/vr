using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomCodeTask")]
    public class CustomCodeTaskController : BaseAPIController
    {
        [HttpPost]
        [Route("TryCompileCustomCodeTask")]
        public  CustomCodeTaskCompilationOutput TryCompileCustomCodeTask(CustomCodeTaskCompileInput input)
        {
            return new CustomCodeTaskManager().TryCompileCustomCodeTask(input.TaskCode, input.ClassDefinitions);
        }

        public class CustomCodeTaskCompileInput
        {
            public string TaskCode { get; set; }

            public string ClassDefinitions { get; set; }

        }
    }
}