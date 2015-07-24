using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class ProfileController : ApiController
    {
        [HttpGet]
        public BigResult<CarrierProfile> GetFilteredProfiles(string resultKey, string name, string companyName, string billingEmail, int from, int to)
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetFilteredProfiles(resultKey, name, companyName, billingEmail, from, to);
        }
        [HttpGet]
        public CarrierProfile GetCarrierProfile(int profileId)
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetCarrierProfile(profileId);
        }
    }
}