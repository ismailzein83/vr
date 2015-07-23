using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class ProfileController : ApiController
    {
        [HttpGet]
        public List<CarrierProfile> GetAllProfiles(string name, string companyName, string billingEmail, int from, int to)
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetAllProfiles(name, companyName, billingEmail, from, to);
        }
        [HttpGet]
        public CarrierProfile GetCarrierProfile(int profileId)
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetCarrierProfile(profileId);
        }
    }
}