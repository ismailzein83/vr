using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerSellingProduct")]
    public class CustomerSellingProductController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCustomerSellingProducts")]
        public object GetFilteredCustomerSellingProducts(Vanrise.Entities.DataRetrievalInput<CustomerSellingProductQuery> input)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return GetWebResponse(input, manager.GetFilteredCustomerSellingProducts(input), "Customer Selling Products");
        }

        [HttpGet]
        [Route("GetCustomerSellingProduct")]
        public CustomerSellingProduct GetCustomerSellingProduct(int customerSellingProductId)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return manager.GetCustomerSellingProduct(customerSellingProductId);
        }

        [HttpPost]
        [Route("AddCustomerSellingProduct")]
        public InsertOperationOutput<List<CustomerSellingProductDetail>> AddCustomerSellingProduct(List<CustomerSellingProduct> customerSellingProducts)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return manager.AddCustomerSellingProduct(customerSellingProducts);
        }

        [HttpPost]
        [Route("UpdateCustomerSellingProduct")]
        public UpdateOperationOutput<CustomerSellingProductDetail> UpdateCustomerSellingProduct(CustomerSellingProductToEdit customerSellingProduct)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return manager.UpdateCustomerSellingProduct(customerSellingProduct);
        }

        [HttpGet]
        [Route("IsCustomerAssignedToSellingProduct")]
        public bool IsCustomerAssignedToSellingProduct(int customerId)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return manager.IsCustomerAssignedToSellingProduct(customerId);
        }

		[HttpGet]
		[Route("GetCustomersBySellingProductId")]
		public IEnumerable<CarrierAccountInfo> GetCustomersBySellingProductId(int sellingProductId)
		{
			CustomerSellingProductManager manager = new CustomerSellingProductManager();
			return manager.GetOrderedCustomersBySellingProductId(sellingProductId);
		}
    }
}