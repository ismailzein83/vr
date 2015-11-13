using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class AccountManagerCarrierDataManager : BaseTOneDataManager, IAccountManagerCarrierDataManager
    {
        public List<AccountManagerCarrier> GetCarriers(int userId)
        {
            return GetItemsSP("BEntity.sp_AccountManager_GetCarriers", AccountManagerCarrier, userId);
        }
        public IEnumerable<AccountManager> GetAccountManagers()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_AccountManager_GetAll]", AccountManagerMapper);
        }


        AccountManager AccountManagerMapper(IDataReader reader)
        {
            AccountManager accountManager = new AccountManager
            {
                CarrierAccountId = GetReaderValue<int>(reader,"CarrierAccountId") ,
                UserId = GetReaderValue<int>(reader, "UserId"),
                RelationType = (CarrierAccountType) reader["RelationType"]
            };

            return accountManager;
        }


        AccountManagerCarrier AccountManagerCarrier(IDataReader reader)
        {
            AccountManagerCarrier carrier = new AccountManagerCarrier
            {
              //  CarrierAccountId = reader["CarrierAccountId"] as string,
                IsCustomerAvailable = (bool)reader["IsCustomerAvailable"],
                IsSupplierAvailable = (bool)reader["IsSupplierAvailable"]
            };

            return carrier;
        }


        public bool AreAccountManagerUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.AccountManager", ref updateHandle);
        }
    }
}
