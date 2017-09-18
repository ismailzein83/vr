using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DID")]
    [JSONWithTypeAttribute]
    public class DIDController : BaseAPIController
    {
        DIDManager manager = new DIDManager();
        
        [HttpPost]
        [Route("GetFilteredDIDs")]
        public object GetFilteredDIDs(Vanrise.Entities.DataRetrievalInput<DIDQuery> input)
        {
            return GetWebResponse(input, manager.GetFilteredDIDs(input));
        }
        [HttpPost]
        [Route("GetFilteredClientDIDs")]
        public object GetFilteredClientDIDs(Vanrise.Entities.DataRetrievalInput<DIDQuery> input)
        {
            return GetWebResponse(input, manager.GetFilteredClientDIDs(input));
        }
        [HttpGet]
        [Route("GetDIDRuntimeEditor")]
        public DIDRuntimeEditor GetDIDRuntimeEditor(int didId)
        {
            return manager.GetDIDRuntimeEditor(didId);
        }
        
        [HttpGet]
        [Route("GetDID")]
        public DID GetDID(int didId)
        {
            return manager.GetDID(didId,true);
        }

        [HttpPost]
        [Route("AddDID")]
        public Vanrise.Entities.InsertOperationOutput<DIDDetail> AddDID(DIDToInsert didToInsert)
        {
            return manager.AddDID(didToInsert);
        }

        [HttpPost]
        [Route("UpdateDID")]
        public Vanrise.Entities.UpdateOperationOutput<DIDDetail> UpdateDID(DID did)
        {
            return manager.UpdateDID(did);
        }

        [HttpGet]
        [Route("GetDIDsInfo")]
        public IEnumerable<DIDInfo> GetDIDsInfo(string serializedFilter = null)
        {
            DIDFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<DIDFilter>(serializedFilter) : null;
            return manager.GetDIDsInfo(filter);
        }

        [HttpGet]
        [Route("GetAccountDIDRelationDefinitionId")]
        public Guid GetAccountDIDRelationDefinitionId()
        {
            return manager.GetAccountDIDRelationDefinitionId();
        }

        [HttpGet]
        [Route("IsDIDAssignedToParentWithoutEED")]
        public bool IsDIDAssignedToParentWithoutEED(Guid accountDIDRelationDefinitionId, string childId)
        {
            return manager.IsDIDAssignedToParentWithoutEED(accountDIDRelationDefinitionId, childId);
        }

        [HttpGet]
        [Route("GetAccountDIDRelationDefinition")]
        public BEParentChildRelationDefinition GetAccountDIDRelationDefinition()
        {
            return manager.GetAccountDIDRelationDefinition();
        }
    }
}