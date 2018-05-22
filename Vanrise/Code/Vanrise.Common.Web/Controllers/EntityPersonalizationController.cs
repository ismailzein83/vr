using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "EntityPersonalization")]
    [JSONWithTypeAttribute]
    public class EntityPersonalizationController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCurrentUserEntityPersonalization")]
        public EntityPersonalizationData GetCurrentUserEntityPersonalization([FromUri] List<string> entityUniqueNames)
        {
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            return manager.GetCurrentUserEntityPersonalization(entityUniqueNames); 
        }

        [HttpPost]
        [Route("UpdateCurrentUserEntityPersonalization")]
        public void UpdateCurrentUserEntityPersonalization(List<EntityPersonalizationInput> inputs)
        {
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            manager.UpdateCurrentUserEntityPersonalization(inputs);
        }

        [HttpPost]
        [Route("UpdateGlobalEntityPersonalization")]
        public void UpdateGlobalEntityPersonalization(List<EntityPersonalizationInput> inputs)
        {
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            manager.UpdateGlobalEntityPersonalization(inputs);
        }

        [HttpPost]
        [Route("DeleteCurrentUserEntityPersonalization")]
        public void DeleteCurrentUserEntityPersonalization(List<string> entityUniqueNames)
        {
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            manager.DeleteCurrentUserEntityPersonalization(entityUniqueNames); 
        }

        [HttpPost]
        [Route("DeleteGlobalEntityPersonalization")]
        public void DeleteGlobalEntityPersonalization(List<string> entityUniqueNames)
        {
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            manager.DeleteGlobalEntityPersonalization(entityUniqueNames); 
        }
        
    }
    
}
