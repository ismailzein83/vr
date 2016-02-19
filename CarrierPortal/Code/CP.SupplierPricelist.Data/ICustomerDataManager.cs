using CP.SupplierPricelist.Entities;
using System.Collections.Generic;

namespace CP.SupplierPricelist.Data
{
    public interface ICustomerDataManager : IDataManager
    {
        List<Customer> GetCustomers(ref byte[] maxTimeStamp, int nbOfRows);
    }
}
