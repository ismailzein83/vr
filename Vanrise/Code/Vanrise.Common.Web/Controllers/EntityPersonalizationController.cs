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
        [Route("UpdateEntityPersonalization")]
        public object UpdateEntityPersonalization(UpdateEntityPersonalizationInput input)
        {
            if (!DosesUserHaveUpdateGlobalEntityPersonalization() && input.AllUsers)
                return GetUnauthorizedResponse();
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            manager.UpdateEntityPersonalization(input.EntityPersonalizationInputs, input.AllUsers);
            return null;
        }

        [HttpPost]
        [Route("DeleteEntityPersonalization")]
        public object DeleteEntityPersonalization(DeleteEntityPersonalizationInput input)
        {
            if (!DosesUserHaveUpdateGlobalEntityPersonalization() && input.AllUsers)
                return GetUnauthorizedResponse();
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            manager.DeleteEntityPersonalization(input.EntityUniqueNames, input.AllUsers);
            return null;
        }

        [HttpGet]
        [Route("DosesUserHaveUpdateGlobalEntityPersonalization")]
        public bool DosesUserHaveUpdateGlobalEntityPersonalization()
        {
            EntityPersonalizationManager manager = new EntityPersonalizationManager();
            return manager.DosesUserHaveUpdateGlobalEntityPersonalization();
        }
        public class UpdateEntityPersonalizationInput
        {
            public List<EntityPersonalizationInput> EntityPersonalizationInputs { get; set; }

            public bool AllUsers { get; set; }
        }

        public class DeleteEntityPersonalizationInput
        {
            public List<string> EntityUniqueNames { get; set; }

            public bool AllUsers { get; set; }
        }

    }

}
