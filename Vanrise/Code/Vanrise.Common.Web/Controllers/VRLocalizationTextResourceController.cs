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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRLocalizationTextResource")]
    [JSONWithTypeAttribute]
    public class VRLocalizationTextResourceController: BaseAPIController
    {
        VRLocalizationTextResourceManager _manager = new VRLocalizationTextResourceManager();

        #region public methods

        [HttpPost]
        [Route("GetFilteredVRLocalizationTextResources")]
        public object GetFilteredVRLocalizationTextResources(Vanrise.Entities.DataRetrievalInput<VRLocalizationTextResourceQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRLocalizationTextResources(input));
        }
       [HttpGet]
        [Route("GetVRLocalizationTextResourceInfo")]
        public IEnumerable<VRLocalizationTextResourceInfo> GetVRLocalizationTextResourceInfo(string filter = null)
        {

            VRLocalizationTextResourceInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRLocalizationTextResourceInfoFilter>(filter) : null;
            return _manager.GetVRLocalizationTextResourceInfo(deserializedFilter);
        }
        [HttpGet]
        [Route("GetVRLocalizationTextResource")]
        public VRLocalizationTextResource GetVRLocalizationTextResource(Guid vrLocalizationTextResource)
        {
            return _manager.GetVRLocalizationTextResource(vrLocalizationTextResource);
        }

        [HttpPost]
        [Route("AddVRLocalizationTextResource")]
        public Vanrise.Entities.InsertOperationOutput<VRLocalizationTextResourceDetail> AddVRLocalizationTextResource(VRLocalizationTextResource vrLocalizationTextResourceItem)
        {
            return _manager.AddVRLocalizationTextResource(vrLocalizationTextResourceItem);
        }

        [HttpPost]
        [Route("UpdateVRLocalizationTextResource")]
        public Vanrise.Entities.UpdateOperationOutput<VRLocalizationTextResourceDetail> UpdateVRLocalizationTextResource(VRLocalizationTextResource vrLocalizationTextResourceItem)
        {
            return _manager.UpdateVRLocalizationTextResource(vrLocalizationTextResourceItem);
        }
        #endregion
    }
}