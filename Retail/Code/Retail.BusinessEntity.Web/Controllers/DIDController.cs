using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
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
        
        [HttpGet]
        [Route("GetDID")]
        public DID GetDID(int dIDId)
        {
            return manager.GetDID(dIDId);
        }

        [HttpPost]
        [Route("AddDID")]
        public Vanrise.Entities.InsertOperationOutput<DIDDetail> AddDID(DID DID)
        {
            return manager.AddDID(DID);
        }

        [HttpPost]
        [Route("UpdateDID")]
        public Vanrise.Entities.UpdateOperationOutput<DIDDetail> UpdateDID(DID DID)
        {
            return manager.UpdateDID(DID);
        }

        [HttpGet]
        [Route("GetDIDsInfo")]
        public IEnumerable<DIDInfo> GetDIDsInfo(string serializedFilter = null)
        {
            DIDFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<DIDFilter>(serializedFilter) : null;
            return manager.GetDIDsInfo(filter);
        }
    }
}