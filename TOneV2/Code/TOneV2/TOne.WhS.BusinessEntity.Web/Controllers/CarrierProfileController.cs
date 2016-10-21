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
    [JSONWithTypeAttribute]
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
        [Route("GetCarrierProfilesInfo")]
        public IEnumerable<CarrierProfileInfo> GetCarrierProfilesInfo()
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.GetCarrierProfilesInfo();
        }

        [HttpPost]
        [Route("AddCarrierProfile")]
        public TOne.Entities.InsertOperationOutput<CarrierProfileDetail> AddCarrierProfile(CarrierProfile carrierProfile)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.AddCarrierProfile(carrierProfile);
        }
        [HttpPost]
        [Route("UpdateCarrierProfile")]
        public TOne.Entities.UpdateOperationOutput<CarrierProfileDetail> UpdateCarrierProfile(CarrierProfileToEdit carrierProfile)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.UpdateCarrierProfile(carrierProfile);
        }
    }
}