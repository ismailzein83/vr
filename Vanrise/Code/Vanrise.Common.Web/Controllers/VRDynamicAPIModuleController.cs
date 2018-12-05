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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRDynamicAPIModule")]
    [JSONWithTypeAttribute]
    public class VRDynamicAPIModuleController : BaseAPIController
    {

        VRDynamicAPIModuleManager vrDynamicAPIModuleManager = new VRDynamicAPIModuleManager();
        [HttpPost]
        [Route("GetFilteredVRDynamicAPIModules")]
        public object GetFilteredVRDynamicAPIModules(DataRetrievalInput<VRDynamicAPIModuleQuery> input)
        {
            return GetWebResponse(input, vrDynamicAPIModuleManager.GetFilteredVRDynamicAPIModules(input));
        }

        [HttpGet]
        [Route("GetVRDynamicAPIModuleById")]
        public VRDynamicAPIModule GetVRDynamicAPIModuleById(int vrDynamicAPIModuleId)
        {
            return vrDynamicAPIModuleManager.GetVRDynamicAPIModuleById(vrDynamicAPIModuleId);
        }

        [HttpPost]
        [Route("UpdateVRDynamicAPIModule")]
        public UpdateOperationOutput<VRDynamicAPIModuleDetails> UpdateVRDynamicAPIModule(VRDynamicAPIModule vrDynamicAPIModule)
        {
            return vrDynamicAPIModuleManager.UpdateVRDynamicAPIModule(vrDynamicAPIModule);
        }

        [HttpPost]
        [Route("AddVRDynamicAPIModule")]
        public InsertOperationOutput<VRDynamicAPIModuleDetails> AddVRDynamicAPIModule(VRDynamicAPIModule vrDynamicAPIModule)
        {
            return vrDynamicAPIModuleManager.AddVRDynamicAPIModule(vrDynamicAPIModule);
        }


    }
}