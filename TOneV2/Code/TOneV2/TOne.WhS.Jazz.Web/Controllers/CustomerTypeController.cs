using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerType")]
    public class CustomerTypeController : BaseAPIController
    {
        CustomerTypeManager _manager = new CustomerTypeManager();

        [HttpGet]
        [Route("GetCustomerTypesInfo")]
        public IEnumerable<CustomerTypeDetail> GetCustomerTypesInfo(string filter=null)
        {
            CustomerTypeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<CustomerTypeInfoFilter>(filter) : null;
            return _manager.GetCustomerTypesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllCustomerTypes")]
        public IEnumerable<CustomerType> GetAllCustomerTypes()
        {
            return _manager.GetAllCustomerTypes();
        }
    }
}