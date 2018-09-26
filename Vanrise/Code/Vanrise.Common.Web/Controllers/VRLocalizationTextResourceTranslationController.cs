using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Web.Base;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Entities.VRLocalization;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRLocalizationTextResourceTranslation")]
    [JSONWithTypeAttribute]
    public class VRLocalizationTextResourceTranslationController : BaseAPIController
    {
        VRLocalizationTextResourceTranslationManager _textResourceTranslationManager = new VRLocalizationTextResourceTranslationManager();
        [HttpPost]
        [Route("GetFilteredVRLocalizationTextResourcesTranslation")]
        public object GetFilteredVRLocalizationTextResourcesTranslation(Vanrise.Entities.DataRetrievalInput<VRLocalizationTextResourceTranslationQuery> input)
        {
            return GetWebResponse(input, _textResourceTranslationManager.GetFilteredVRLocalizationTextResourcesTranslation(input), "Localization Text Resources Translation");
        }

        [HttpPost]
        [Route("AddVRLocalizationTextResourceTranslation")]
        public InsertOperationOutput<VRLocalizationTextResourceTranslationDetail> AddVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslationItem)
        {
            return _textResourceTranslationManager.AddVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslationItem);
        }

        [HttpPost]
        [Route("UpdateVRLocalizationTextResourceTranslation")]
        public UpdateOperationOutput<VRLocalizationTextResourceTranslationDetail> UpdateVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslationItem)
        {
            return _textResourceTranslationManager.UpdateVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslationItem);
        }
        [HttpGet]
        [Route("GetVRLocalizationTextResourceTranslation")]
        public VRLocalizationTextResourceTranslation GetVRLocalizationTextResourceTranslation(Guid vrLocalizationTextResourceTranslationId)
        {
            return _textResourceTranslationManager.GetVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslationId);
        }
    }
}