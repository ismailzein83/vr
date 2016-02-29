using InterConnect.BusinessEntity.Business;
using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace InterConnect.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorProfile")]
    public class OperatorProfileController:BaseAPIController
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
        [Route("GetRunTimeExtendedSettings")]
        public GenericEditorRuntime GetRunTimeExtendedSettings(int dataRecordTypeId)
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.GetRunTimeExtendedSettings(dataRecordTypeId);
        }
        [HttpGet]
        [Route("GetDataRecordTypes")]
        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo()
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.GetDataRecordTypesInfo();
        }

        [HttpGet]
        [Route("GetOperatorProfilsInfo")]
        public IEnumerable<OperatorProfileInfo> GetOperatorProfilsInfo(string serializedFilter)
        {
            OperatorProfileInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<OperatorProfileInfoFilter>(serializedFilter) : null;
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.GetOperatorProfilsInfo(filter);
        }


        [HttpPost]
        [Route("AddOperatorProfile")]
        public InsertOperationOutput<OperatorProfileDetail> AddOperatorProfile(OperatorProfile operatorProfile)
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.AddOperatorProfile(operatorProfile);
        }
        [HttpPost]
        [Route("UpdateOperatorProfile")]
        public UpdateOperationOutput<OperatorProfileDetail> UpdateOperatorProfile(OperatorProfile operatorProfile)
        {
            OperatorProfileManager manager = new OperatorProfileManager();
            return manager.UpdateOperatorProfile(operatorProfile);
        }
    }
}