using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "CarrierProfile")]
    public class Demo_CarrierProfileController : BaseAPIController
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
        [Route("GetCarrierProfilesInfo")]
        public IEnumerable<CarrierProfileInfo> GetCarrierProfilesInfo()
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.GetCarrierProfilesInfo();
        }

        [HttpPost]
        [Route("AddCarrierProfile")]
        public Vanrise.Entities.InsertOperationOutput<CarrierProfileDetail> AddCarrierProfile(CarrierProfile carrierProfile)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.AddCarrierProfile(carrierProfile);
        }
        [HttpPost]
        [Route("UpdateCarrierProfile")]
        public Vanrise.Entities.InsertOperationOutput<CarrierProfileDetail> UpdateCarrierProfile(CarrierProfile carrierProfile)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.UpdateCarrierProfile(carrierProfile);
        }
    }
}