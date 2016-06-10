﻿using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public ServiceType GetServiceType(int serviceTypeId)
        {
            return _manager.GetServiceType(serviceTypeId);
        }

        [HttpPost]
        [Route("UpdateServiceType")]
        public Vanrise.Entities.UpdateOperationOutput<ServiceTypeDetail> UpdateServiceType(ServiceTypeToEdit serviceType)
        {
            return _manager.UpdateServiceType(serviceType);
        }
    }
}