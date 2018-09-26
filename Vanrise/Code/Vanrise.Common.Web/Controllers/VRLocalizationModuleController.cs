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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRLocalizationModule")]
    [JSONWithTypeAttribute]
    public class VRLocalizationModuleController : BaseAPIController
    {
        VRLocalizationModuleManager _manager = new VRLocalizationModuleManager();

        [HttpPost]
        [Route("GetFilteredVRLocalizationModules")]
        public object GetFilteredVRLocalizationModules(Vanrise.Entities.DataRetrievalInput<VRLocalizationModuleQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRLocalizationModules(input), "Localization Modules");
        }

        [HttpGet]
        [Route("GetVRLocalizationModule")]
        public VRLocalizationModule GetVRLocalizationModule(Guid vrLocalizationModule)
        {
            return _manager.GetVRLocalizationModule(vrLocalizationModule);
        }

        [HttpPost]
        [Route("AddVRLocalizationModule")]
        public Vanrise.Entities.InsertOperationOutput<VRLocalizationModuleDetail> AddVRLocalizationModule(VRLocalizationModule vrLocalizationModuleItem)
        {
            return _manager.AddVRLocalizationModule(vrLocalizationModuleItem);
        }

        [HttpPost]
        [Route("UpdateVRLocalizationModule")]
        public Vanrise.Entities.UpdateOperationOutput<VRLocalizationModuleDetail> UpdateVRLocalizationModule(VRLocalizationModule vrLocalizationModuleItem)
        {
            return _manager.UpdateVRLocalizationModule(vrLocalizationModuleItem);
        }

        [HttpGet]
        [Route("GetVRLocalizationModuleInfo")]
        public IEnumerable<VRLocalizationModuleInfo> GetVRLocalizationModuleInfo(string filter = null)
        {

            VRLocalizationModuleInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRLocalizationModuleInfoFilter>(filter) : null;
            return _manager.GetVRLocalizationModulesInfo(deserializedFilter);
        }

    }
}