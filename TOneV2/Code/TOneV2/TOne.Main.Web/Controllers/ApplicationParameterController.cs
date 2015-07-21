using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Business;
using TOne.Entities;

namespace TOne.Main.Web.Controllers
{
    public class ApplicationParameterController : ApiController
    {
        [HttpGet]
        public ApplicationParameter GetApplicationParameterById(int parameterId)
        {
            ApplicationParameterManager manager = new ApplicationParameterManager();
            return manager.GetApplicationParameterById(parameterId);
        }

        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<ApplicationParameter> UpdateApplicationParameter(ApplicationParameter appParamObj)
        {
            ApplicationParameterManager manager = new ApplicationParameterManager();
            return manager.UpdateApplicationParameter(appParamObj);
        }
    }
}