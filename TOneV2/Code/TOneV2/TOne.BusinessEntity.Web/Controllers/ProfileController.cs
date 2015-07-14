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
        public List<CarrierProfile> GetAllProfiles()
        {
            ProfileManager manager = new ProfileManager();
            return manager.GetAllProfiles();
        }
    }
}