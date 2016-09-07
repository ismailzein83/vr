using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRAlertRuleType")]
    [JSONWithTypeAttribute]
    public class VRAlertRuleTypeController : BaseAPIController
    {
        VRAlertRuleTypeManager _manager = new VRAlertRuleTypeManager();

        [HttpPost]
        [Route("GetFilteredVRAlertRuleTypes")]
        public object GetFilteredVRAlertRuleTypes(Vanrise.Entities.DataRetrievalInput<VRAlertRuleTypeQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRAlertRuleTypes(input));
        }

        [HttpGet]
        [Route("GetVRAlertRuleType")]
        public VRAlertRuleType GetVRAlertRuleType(Guid vrAlertRuleTypeId)
        {
            return _manager.GetVRAlertRuleType(vrAlertRuleTypeId);
        }

        [HttpPost]
        [Route("AddVRAlertRuleType")]
        public Vanrise.Entities.InsertOperationOutput<VRAlertRuleTypeDetail> AddVRAlertRuleType(VRAlertRuleType vrAlertRuleTypeItem)
        {
            return _manager.AddVRAlertRuleType(vrAlertRuleTypeItem);
        }

        [HttpPost]
        [Route("UpdateVRAlertRuleType")]
        public Vanrise.Entities.UpdateOperationOutput<VRAlertRuleTypeDetail> UpdateVRAlertRuleType(VRAlertRuleType vrAlertRuleTypeItem)
        {
            return _manager.UpdateVRAlertRuleType(vrAlertRuleTypeItem);
        }

        [HttpGet]
        [Route("GetVRAlertRuleTypeSettingsExtensionConfigs")]
        public IEnumerable<VRAlertRuleTypeConfig> GetVRAlertRuleTypeSettingsExtensionConfigs()
        {
            return _manager.GetVRAlertRuleTypeSettingsExtensionConfigs();
        }

        //[HttpGet]
        //[Route("GetVRAlertRuleTypesInfo")]
        //public IEnumerable<VRAlertRuleTypeInfo> GetVRAlertRuleTypesInfo(string filter = null)
        //{
        //    VRAlertRuleTypeFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRAlertRuleTypeFilter>(filter) : null;
        //    return _manager.GetVRAlertRuleTypesInfo(deserializedFilter);
        //}
    }
}