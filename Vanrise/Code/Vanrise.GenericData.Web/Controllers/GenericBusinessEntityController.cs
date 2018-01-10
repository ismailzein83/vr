using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBusinessEntity")]
    public class GenericBusinessEntityController : BaseAPIController
    {
        GenericBusinessEntityManager _manager = new GenericBusinessEntityManager();

        [HttpPost]
        [Route("GetFilteredGenericBusinessEntities")]
        public object GetFilteredGenericBusinessEntities(Vanrise.Entities.DataRetrievalInput<GenericBusinessEntityQuery> input)
        {
            //if (!manager.DoesUserHaveViewAccess(input.Query.BusinessEntityDefinitionId))
            //    return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredGenericBusinessEntities(input));
        }

        //[HttpGet]
        //[Route("GetGenericBusinessEntity")]
        //public GenericBusinessEntity GetGenericBusinessEntity(long genericBusinessEntityId, Guid businessEntityDefinitionId)
        //{
        //    return _manager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId, true);
        //}
        ////[HttpGet]
        ////[Route("DoesUserHaveAddAccess")]
        ////public bool DoesUserHaveAddAccess(Guid businessEntityDefinitionId)
        ////{
        ////    return _manager.DoesUserHaveAddAccess(businessEntityDefinitionId);
        ////}

        [HttpPost]
        [Route("AddGenericBusinessEntity")]
        public object AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            //if (!DoesUserHaveAddAccess(genericBusinessEntity.BusinessEntityDefinitionId))
            //    return GetUnauthorizedResponse();
            return _manager.AddGenericBusinessEntity(genericBusinessEntity);
        }

        [HttpPost]
        [Route("UpdateGenericBusinessEntity")]
        public object UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            //if (!DoesUserHaveEditAccess(genericBusinessEntity.BusinessEntityDefinitionId))
            //    return GetUnauthorizedResponse();

            return _manager.UpdateGenericBusinessEntity(genericBusinessEntity);
        }

        //[HttpGet]
        //[Route("DoesUserHaveEditAccess")]
        //public bool DoesUserHaveEditAccess(Guid businessEntityDefinitionId)
        //{
        //    GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
        //    return manager.DoesUserHaveEditAccess(businessEntityDefinitionId);
        //}


        //[HttpGet]
        //[Route("GetGenericBusinessEntityInfo")]
        //public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo(Guid businessEntityDefinitionId, string serializedFilter = null)
        //{
        //    GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
        //    GenericBusinessEntityFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<GenericBusinessEntityFilter>(serializedFilter) : null;
        //    return manager.GetGenericBusinessEntityInfo(businessEntityDefinitionId,filter);
        //} 
        
        //[HttpGet]
        //[Route("GetBusinessEntityTitle")]
        //public GenericBusinessEntityTitle GetBusinessEntityTitle(Guid businessEntityDefinitionId, long? genericBussinessEntityId = null)
        //{
        //    GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
        //    return manager.GetBusinessEntityTitle(businessEntityDefinitionId, genericBussinessEntityId);
        //}
        
        //[HttpGet]
        //[Route("DeleteGenericBusinessEntity")]
        //public Vanrise.Entities.DeleteOperationOutput<object> DeleteGenericBusinessEntity(long genericBusinessEntityId, Guid businessEntityDefinitionId)
        //{
        //    GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
        //    return manager.DeleteGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
        //}
         
    }
}