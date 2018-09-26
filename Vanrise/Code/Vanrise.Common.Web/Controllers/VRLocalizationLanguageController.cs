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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRLocalizationLanguage")]
    [JSONWithTypeAttribute]
    public class VRLocalizationLanguageController: BaseAPIController
    {
        VRLocalizationLanguageManager _manager = new VRLocalizationLanguageManager();

        [HttpPost]
        [Route("GetFilteredVRLocalizationLanguages")]
        public object GetFilteredVRLocalizationLanguages(Vanrise.Entities.DataRetrievalInput<VRLocalizationLanguageQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRLocalizationLanguages(input), "Localization Languages");
        }

        [HttpGet]
        [Route("GetVRLocalizationLanguage")]
        public VRLocalizationLanguage GetVRLocalizationLanguage(Guid vrLocalizationLanguage)
        {
            return _manager.GetVRLocalizationLanguage(vrLocalizationLanguage);
        }

        [HttpPost]
        [Route("AddVRLocalizationLanguage")]
        public Vanrise.Entities.InsertOperationOutput<VRLocalizationLanguageDetail> AddVRLocalizationLanguage(VRLocalizationLanguage vrLocalizationLanguageItem)
        {
            return _manager.AddVRLocalizationLanguage(vrLocalizationLanguageItem);
        }

        [HttpPost]
        [Route("UpdateVRLocalizationLanguage")]
        public Vanrise.Entities.UpdateOperationOutput<VRLocalizationLanguageDetail> UpdateVRLocalizationLanguage(VRLocalizationLanguage vrLocalizationLanguageItem)
        {
            return _manager.UpdateVRLocalizationLanguage(vrLocalizationLanguageItem);
        }

        [HttpGet]
        [Route("GetVRLocalizationLanguageInfo")]
        public IEnumerable<VRLocalizationLanguageInfo> GetVRLocalizationLanguageInfo(string filter = null)
        {

            VRLocalizationLanguageInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRLocalizationLanguageInfoFilter>(filter) : null;
            return _manager.GetVRLocalizationLanguagesInfo(deserializedFilter);
        }

    }
}