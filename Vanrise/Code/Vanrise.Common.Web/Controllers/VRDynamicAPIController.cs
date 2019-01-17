using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRDynamicAPI")]
    [JSONWithTypeAttribute]
    public class VRDynamicAPIController : BaseAPIController
    {

        VRDynamicAPIManager vrDynamicAPIManager = new VRDynamicAPIManager();
        [HttpPost]
        [Route("GetFilteredVRDynamicAPIs")]
        public object GetFilteredVRDynamicAPIs(DataRetrievalInput<VRDynamicAPIQuery> input)
        {
            return GetWebResponse(input, vrDynamicAPIManager.GetFilteredVRDynamicAPIs(input));
        }


        [HttpGet]
        [Route("GetVRDynamicAPIById")]
        public VRDynamicAPI GetVRDynamicAPIById(Guid vrDynamicAPIId)
        {
            return vrDynamicAPIManager.GetVRDynamicAPIById(vrDynamicAPIId);
        }

        [HttpPost]
        [Route("UpdateVRDynamicAPI")]
        public UpdateOperationOutput<VRDynamicAPIDetails> UpdateVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            return vrDynamicAPIManager.UpdateVRDynamicAPI(vrDynamicAPI);
        }

        [HttpPost]
        [Route("AddVRDynamicAPI")]
        public InsertOperationOutput<VRDynamicAPIDetails> AddVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            return vrDynamicAPIManager.AddVRDynamicAPI(vrDynamicAPI);
        }

        [HttpGet]

        [Route("GetVRDynamicAPIMethodSettingsConfigs")]
        public IEnumerable<VRDynamicAPIMethodConfig> GetVRDynamicAPIMethodSettingsConfigs()
        {
            return vrDynamicAPIManager.GetVRDynamicAPIMethodSettingsConfigs();
        }

        [HttpPost]
        [Route("TryCompileVRDynamicAPI")]
        public VRDynamicAPICompilationResult TryCompileVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            return vrDynamicAPIManager.TryCompileVRDynamicAPI(vrDynamicAPI);
        }
        [HttpPost]
        [Route("BuildAllDynamicAPIControllers")]
        public List<Type> BuildAllDynamicAPIControllers()
        {
            return vrDynamicAPIManager.BuildAllDynamicAPIControllers();
        }

    }
}