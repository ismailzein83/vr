using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntity")]
    public class VR_BusinessEntityController:BaseAPIController
    {
        BusinessEntityManager _manager = new BusinessEntityManager();
        
        [HttpPost]
        [Route("GetFilteredBusinessEntities")]
        public object GetFilteredBusinessEntities(Vanrise.Entities.DataRetrievalInput<BusinessEntityQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredBusinessEntities(input), "Business Entities");
        }
        [HttpPost]
        [Route("GetBusinessEntitiesByIds")]
        public IEnumerable<BusinessEntityInfo> GetBusinessEntitiesByIds(List<Guid> entitiesIds)
        {
            return _manager.GetBusinessEntitiesByIds(entitiesIds);
        }
        [HttpGet]
        [Route("GetBusinessEntity")]
        public BusinessEntity GetBusinessEntity(Guid entityId)
        {
            return _manager.GetBusinessEntityById(entityId);
        }
        [HttpPost]
        [Route("UpdateBusinessEntity")]
        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityDetail> UpdateBusinessEntity(BusinessEntity businessEntity)
        {
            return _manager.UpdateBusinessEntity(businessEntity);
        }

        [HttpPost]
        [Route("AddBusinessEntity")]
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityDetail> AddBusinessEntity(BusinessEntity businessEntity)
        {
            return _manager.AddBusinessEntity(businessEntity);
        }
    }
}