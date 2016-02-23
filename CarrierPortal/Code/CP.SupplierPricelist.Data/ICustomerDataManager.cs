using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Data
{
    public interface ICustomerDataManager : IDataManager
    {
        List<Customer> GetCustomers(ref byte[] maxTimeStamp, int nbOfRows);

        List<Customer> GetAllCustomers();

        bool AreCustomersUpdated(ref object updateHandle);
        bool AddCustomer(Customer inputCustomer, out int customerId);
    }
}
