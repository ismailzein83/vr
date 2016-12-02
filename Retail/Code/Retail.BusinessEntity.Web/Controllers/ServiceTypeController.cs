using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ServiceType")]
    public class ServiceTypeController : BaseAPIController
    {
        ServiceTypeManager _manager = new ServiceTypeManager();

        [HttpPost]
        [Route("GetFilteredServiceTypes")]
        public object GetFilteredServiceTypes(Vanrise.Entities.DataRetrievalInput<ServiceTypeQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredServiceTypes(input));
        }

        [HttpGet]
        [Route("GetServiceType")]
        public ServiceType GetServiceType(Guid serviceTypeId)
        {
            return _manager.GetServiceType(serviceTypeId);
        }

        [HttpPost]
        [Route("UpdateServiceType")]
        public Vanrise.Entities.UpdateOperationOutput<ServiceTypeDetail> UpdateServiceType(ServiceTypeToEdit serviceType)
        {
            return _manager.UpdateServiceType(serviceType);
        }

        [HttpGet]
        [Route("GetServiceTypesInfo")]
        public IEnumerable<ServiceTypeInfo> GetServiceTypesInfo(string serializedFilter = null)
        {
            ServiceTypeInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<ServiceTypeInfoFilter>(serializedFilter) : null;
            return _manager.GetServiceTypesInfo(filter);
        }

        [HttpGet]
        [Route("GetServiceTypeChargingPolicyDefinitionSettings")]
        public ChargingPolicyDefinitionSettings GetServiceTypeChargingPolicyDefinitionSettings(Guid serviceTypeId)
        {
            return _manager.GetServiceTypeChargingPolicyDefinitionSettings(serviceTypeId);
        }

        [HttpGet]
        [Route("GetServiceTypeExtendedSettingsTemplateConfigs")]
        public IEnumerable<ServiceTypeExtendedSettingsConfig> GetServiceTypeExtendedSettingsTemplateConfigs()
        {
            return _manager.GetServiceTypeExtendedSettingsTemplateConfigs();
        }
    }
}