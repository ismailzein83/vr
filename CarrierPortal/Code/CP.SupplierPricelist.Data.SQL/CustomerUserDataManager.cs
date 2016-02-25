using System.Collections.Generic;
using CP.SupplierPricelist.Entities;
using Vanrise.Data.SQL;
using System.Data;
using Vanrise.Common;

namespace CP.SupplierPricelist.Data.SQL
{
    public class CustomerUserDataManager : BaseSQLDataManager, ICustomerUserDataManager
    {
        public CustomerUserDataManager() :
            base(GetConnectionStringName("CP_DBConnStringKey", "CP_DBConnString"))
        {

        }
        public List<CustomerUser> GetAllCustomersUsers()
        {
            return GetItemsSP("[CP_SupPriceList].[sp_CustomerUser_GetAll]", CustomerUserMapper);
        }
        public bool AreCustomersUsersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[CP_SupPriceList].[CustomerUser]", ref updateHandle);
        }
        CustomerUser CustomerUserMapper(IDataReader reader)
        {
            CustomerUser customerUser = new CustomerUser
            {
                CustomerId = (int)reader["CustomerID"],
                UserId = (int)reader["UserID"]

            };

            return customerUser;
        }

        public bool Insert(CustomerUser input)
        {
            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_CustomerUser_Insert]", input.CustomerId, input.UserId);
            return (recordsEffected > 0);
        }

        public bool Delete(int userId)
        {
            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_Customeruser_Delete]", userId);
            return (recordsEffected > 0);
        }
    }
}
