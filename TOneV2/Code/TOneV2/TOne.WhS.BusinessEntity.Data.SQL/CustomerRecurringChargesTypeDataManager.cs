using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CustomerRecurringChargesTypeDataManager: Vanrise.Data.SQL.BaseSQLDataManager, ICustomerRecurringChargesTypeDataManager
    {
        #region ctor/Local Variables
        public CustomerRecurringChargesTypeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<CustomerRecurringChargesType> GetAllCustomerRecurringChargesTypes()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerRecurringChargesType_GetAll", CustomerRecurringChargesTypeMapper);
        }

        public bool AreAllCustomerRecurringChargesTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[CustomerRecurringChargesType]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private CustomerRecurringChargesType CustomerRecurringChargesTypeMapper(IDataReader reader)
        {
            CustomerRecurringChargesType customerRecurringChargesType = new CustomerRecurringChargesType();

            customerRecurringChargesType.CustomerRecurringChargeTypeId = (long)reader["ID"];
            customerRecurringChargesType.Name = (string)reader["Name"];

            return customerRecurringChargesType;
        }

        #endregion
    }
}
