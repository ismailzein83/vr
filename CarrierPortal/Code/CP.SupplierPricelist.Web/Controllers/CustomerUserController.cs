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
        [Route("GetFilteredCustomerUsers")]
        public object GetFilteredCustomerUsers(DataRetrievalInput<CustomerUserQuery> input)
        {
            CustomerUserManager manager = new CustomerUserManager();
            return GetWebResponse(input, manager.GetFilteredCustomerUsers(input));
        } 

        [HttpPost]
        [Route("AddCustomerUser")]
        public InsertOperationOutput<CustomerUserDetail> AddCustomerUser(CustomerUser input)
        {
            CustomerUserManager manager = new CustomerUserManager();
            return manager.AddCustomerUser(input);
        }

        [HttpGet]
        [Route("DeleteCustomerUser")]
        public DeleteOperationOutput<CustomerUserDetail> DeleteCustomerUser(int userId)
        {

            CustomerUserManager manager = new CustomerUserManager();
            return manager.DeleteCustomerUser(userId);
        }
    }
}