using QM.CLITester.Business;
using QM.CLITester.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace QM.CLITester.Web.Controllers
{
   
    [RoutePrefix(Constants.ROUTE_PREFIX + "Profile")]

    [JSONWithTypeAttribute]
    public class QMCLITester_ProfileController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredProfiles")]
        public object GetFilteredProfiles(Vanrise.Entities.DataRetrievalInput<ProfileQuery> input)
        {
            ProfileManager manager = new ProfileManager();
            return GetWebResponse(input, manager.GetFilteredProfiles(input));
        }

        [HttpGet]
        [Route("GetProfile")]
        public Profile GetProfile(int profileId)
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetProfile(profileId);
        }

        [HttpGet]
        [Route("GetProfilesInfo")]
        public IEnumerable<ProfileInfo> GetProfilesInfo()
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetProfilesInfo();
        }

        [HttpPost]
        [Route("UpdateProfile")]
        public UpdateOperationOutput<ProfileDetail> UpdateProfile(Profile profile)
        {
            ProfileManager manager = new ProfileManager();
            return manager.UpdateProfile(profile);
        }


        [HttpGet]
        [Route("GetProfileSourceTemplates")]
        public IEnumerable<SourceProfileReaderConfig> GetProfileSourceTemplates()
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetProfileSourceTemplates();
        }

    }
}