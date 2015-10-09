using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "CarrierProfile")]
    public class WhSBE_CarrierProfileController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCarrierProfiles")]
         public object GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return GetWebResponse(input, manager.GetFilteredCarrierProfiles(input));
        }

        [HttpGet]
        [Route("GetCarrierProfile")]
        public CarrierProfile GetCarrierProfile(int carrierProfileId)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.GetCarrierProfile(carrierProfileId);
        }

        [HttpGet]
        [Route("GetAllCarrierProfiles")]
        public List<CarrierProfileInfo> GetAllCarrierProfiles()
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.GetAllCarrierProfiles();
        }

    }
}