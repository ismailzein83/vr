using CP.SupplierPricelist.Entities;
using System.Collections.Generic;

namespace CP.SupplierPricelist.Data
{
    public interface ICustomerUserDataManager : IDataManager
    {

        List<CustomerUser> GetAllCustomersUsers();

        bool AreCustomersUsersUpdated(ref object updateHandle);

        bool Insert(CustomerUser input);
        bool Delete(int userId);
    }
}
