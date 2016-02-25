using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Entities;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.SupplierPricelist.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerUser")]
    [JSONWithTypeAttribute]
    public class CustomerUserController : BaseAPIController
    {
        [HttpPost]
        [Route("AddUser")]
        public UpdateOperationOutput<CustomerUser> AddUser(CustomerUser input)
        {
            CustomerUserManager manager = new CustomerUserManager();
            return manager.AddUser(input);
        }
    }
}