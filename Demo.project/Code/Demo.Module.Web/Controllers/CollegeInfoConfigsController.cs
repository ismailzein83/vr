using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;
namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CollegeInfoConfigs")]
    [JSONWithType]
    public class Demo_Module_ExtensionsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCollegeInfoTypeTemplateConfigs")]
        public IEnumerable<CollegeInfoConfig> GetCollegeInfoTypeTemplateConfigs()
        {
            CollegeInfoConfigsManager collegeInfoConfigsManager = new CollegeInfoConfigsManager();
            return collegeInfoConfigsManager.GetCollegeInfoTemplateConfigs();
        }

    }

}