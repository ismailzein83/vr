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
    public class DIDController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredDIDs")]
        public object GetFilteredDIDs(Vanrise.Entities.DataRetrievalInput<DIDQuery> input)
        {
            DIDManager manager = new DIDManager();
            return GetWebResponse(input, manager.GetFilteredDIDs(input));
        }
        
        [HttpGet]
        [Route("GetDID")]
        public DID GetDID(int dIDId)
        {
            DIDManager manager = new DIDManager();
            return manager.GetDID(dIDId);
        }

        [HttpGet]
        [Route("GetDIDsInfo")]
        public IEnumerable<DIDInfo> GetDIDsInfo()
        {
            DIDManager manager = new DIDManager();
            return manager.GetDIDsInfo();
        }
        [HttpPost]
        [Route("AddDID")]
        public Vanrise.Entities.InsertOperationOutput<DIDDetail> AddDID(DID DID)
        {

            DIDManager manager = new DIDManager();
            return manager.AddDID(DID);
        }

        [HttpPost]
        [Route("UpdateDID")]
        public Vanrise.Entities.UpdateOperationOutput<DIDDetail> UpdateDID(DID DID)
        {
            DIDManager manager = new DIDManager();
            return manager.UpdateDID(DID);
        }
       
    }
}