using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "TextManipulation")]
     [JSONWithTypeAttribute]
    public class TextManipulationController : BaseAPIController
    {
         TextManipulationManager _manager = new TextManipulationManager();
        
         [HttpGet]
         [Route("GetTextManipulationActionSettingsConfigs")]
         public IEnumerable<TextManipulationActionSettingsConfig> GetTextManipulationActionSettingsConfigs()
         {
             return _manager.GetTextManipulationActionSettingsConfigs();
         }
    }
}