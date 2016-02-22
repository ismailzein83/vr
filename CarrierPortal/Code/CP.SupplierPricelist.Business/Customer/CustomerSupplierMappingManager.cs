using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;

namespace CP.SupplierPricelist.Business
{
    public class CustomerSupplierMappingManager
    {
        public List<SupplierInfo> GetCustomerSuppliers()
        {
            int userId = new SecurityContext().GetLoggedInUserId();
            int customerId = new CustomerUserManager().GetCustomerIdByUserId(userId);
            Customer customer = new CustomerManager().GetCustomer(customerId);
            if (customer != null)
            {
                SupplierPriceListConnectorBase supplierPriceListConnectorBase = customer.Settings.PriceListConnector as SupplierPriceListConnectorBase;

                return supplierPriceListConnectorBase.GetSuppliers(null);
            }
            else 
                throw new NotSupportedException();          
            
        }
    }
}
