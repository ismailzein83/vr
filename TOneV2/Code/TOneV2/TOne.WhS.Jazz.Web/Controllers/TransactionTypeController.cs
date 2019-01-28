using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TransactionType")]
    public class TransactionTypeController : BaseAPIController
    {
        TransactionTypeManager _manager = new TransactionTypeManager();

        [HttpGet]
        [Route("GetTransactionTypesInfo")]
        public IEnumerable<TransactionTypeDetail> GetTransactionTypesInfo(string filter=null)
        {
            TransactionTypeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<TransactionTypeInfoFilter>(filter) : null;
            return _manager.GetTransactionTypesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllTransactionTypes")]
        public IEnumerable<TransactionType> GetAllTransactionTypes()
        {
            return _manager.GetAllTransactionTypes();
        }
    }
}