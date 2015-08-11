using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class AccountManagerCarrierDataManager : BaseTOneDataManager, IAccountManagerCarrierDataManager
    {
        public List<AccountManagerCarrier> GetCarriers(int userId)
        {
            return GetItemsSP("BEntity.sp_AccountManager_GetCarriers", AccountManagerCarrier, userId);
        }

        AccountManagerCarrier AccountManagerCarrier(IDataReader reader)
        {
            AccountManagerCarrier carrier = new AccountManagerCarrier
            {
                CarrierAccountId = reader["CarrierAccountId"] as string,
                CarrierName = CarrierAccountDataManager.GetCarrierAccountName(reader["Name"] as string, reader["NameSuffix"] as string),
                IsCustomerAvailable = (bool)reader["IsCustomerAvailable"],
                IsSupplierAvailable = (bool)reader["IsSupplierAvailable"]
            };

            return carrier;
        }
    }
}
