using CP.SupplierPricelist.Data;
using System.Collections.Generic;
using CP.SupplierPricelist.Entities;

namespace CP.SupplierPricelist.Business
{
    public class CustomerManager
    {
        public CustomerOutput GetCustomers(ref byte[] maxTimeStamp, int nbOfRows)
        {
            CustomerOutput output = new CustomerOutput();
            ICustomerDataManager dataManager =
             CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
            List<Customer> customers = dataManager.GetCustomers(ref maxTimeStamp, nbOfRows);
            output.Customers = customers;
            output.MaxTimeStamp = maxTimeStamp;
            return output;
        }
    }
}
