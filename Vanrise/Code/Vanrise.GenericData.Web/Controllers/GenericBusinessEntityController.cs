﻿using System;
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
    public class GenericBusinessEntityController:BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredGenericBusinessEntities")]
        public object GetFilteredGenericBusinessEntities(Vanrise.Entities.DataRetrievalInput<GenericBusinessEntityQuery> input)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return GetWebResponse(input, manager.GetFilteredGenericBusinessEntities(input));
        }

        [HttpGet]
        [Route("GetGenericBusinessEntity")]
        public GenericBusinessEntity GetGenericBusinessEntity(long genericBusinessEntityId, int businessEntityDefinitionId)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
        }

        [HttpPost]
        [Route("AddGenericBusinessEntity")]
        public Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.AddGenericBusinessEntity(genericBusinessEntity);
        }

        [HttpPost]
        [Route("UpdateGenericBusinessEntity")]
        public Vanrise.Entities.UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.UpdateGenericBusinessEntity(genericBusinessEntity);
        }

        [HttpGet]
        [Route("GetGenericBusinessEntityInfo")]
        public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo(int businessEntityDefinitionId,string serializedFilter = null)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            GenericBusinessEntityFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<GenericBusinessEntityFilter>(serializedFilter) : null;
            return manager.GetGenericBusinessEntityInfo(businessEntityDefinitionId,filter);
        } 
        [HttpGet]
        [Route("GetBusinessEntityTitle")]
        public GenericBusinessEntityTitle GetBusinessEntityTitle(int businessEntityDefinitionId, long? genericBussinessEntityId = null)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.GetBusinessEntityTitle(businessEntityDefinitionId, genericBussinessEntityId);
        } 
         
    }
}