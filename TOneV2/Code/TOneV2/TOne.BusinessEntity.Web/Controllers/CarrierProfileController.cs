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
    public class CarrierProfileController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return GetWebResponse(input, manager.GetFilteredCarrierProfiles(input));
        }
        [HttpGet]
        public CarrierProfile GetCarrierProfile(int profileId)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.GetCarrierProfile(profileId);
        }
        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<CarrierProfile> UpdateCarrierProfile(CarrierProfile carrierProfile)
        {
            CarrierProfileManager manager = new CarrierProfileManager();
            return manager.UpdateCarrierProfile(carrierProfile);
        }
    }
}