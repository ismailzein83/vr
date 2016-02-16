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
     [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorProfile")]
    public class Demo_OperatorProfileController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredOperatorProfiles")]
         public object GetFilteredOperatorProfiles(Vanrise.Entities.DataRetrievalInput<OperatorProfileQuery> input)
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return GetWebResponse(input, manager.GetFilteredOperatorProfiles(input));
        }

        [HttpGet]
        [Route("GetOperatorProfile")]
        public OperatorProfile GetOperatorProfile(int operatorProfileId)
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.GetOperatorProfile(operatorProfileId);
        }

        [HttpGet]
        [Route("GetOperatorProfilesInfo")]
        public IEnumerable<OperatorProfileInfo> GetOperatorProfilesInfo()
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.GetOperatorProfilesInfo();
        }

        [HttpPost]
        [Route("AddOperatorProfile")]
        public Vanrise.Entities.InsertOperationOutput<OperatorProfileDetail> AddOperatorProfile(OperatorProfile operatorProfile)
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.AddOperatorProfile(operatorProfile);
        }
        [HttpPost]
        [Route("UpdateOperatorProfile")]
        public Vanrise.Entities.UpdateOperationOutput<OperatorProfileDetail> UpdateOperatorProfile(OperatorProfile operatorProfile)
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.UpdateOperatorProfile(operatorProfile);
        }
    }
}