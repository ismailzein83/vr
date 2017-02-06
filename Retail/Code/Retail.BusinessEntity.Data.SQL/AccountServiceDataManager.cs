using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountServiceDataManager:BaseSQLDataManager,IAccountServiceDataManager
    {
           
        #region ctor/Local Variables
        public AccountServiceDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(AccountService accountService, out long insertedId)
        {
            object accountServiceId;
            string serializedSettings = null;
            if (accountService.Settings != null)
            {
                serializedSettings = Vanrise.Common.Serializer.Serialize(accountService.Settings);
            }
            int recordsEffected = ExecuteNonQuerySP("Retail.sp_AccountService_Insert", out accountServiceId, accountService.AccountId, accountService.ServiceTypeId, accountService.ServiceChargingPolicyId, serializedSettings, accountService.StatusId);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (long)accountServiceId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(AccountService accountService)
        {
            string serializedSettings = null;
            if(accountService.Settings !=null)
            {
                serializedSettings = Vanrise.Common.Serializer.Serialize(accountService.Settings);
            }
            int recordsEffected = ExecuteNonQuerySP("Retail.sp_AccountService_Update", accountService.AccountServiceId, accountService.AccountId, accountService.ServiceTypeId, accountService.ServiceChargingPolicyId,serializedSettings);
            return (recordsEffected > 0);
        }
        public bool AreAccountServicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.AccountService", ref updateHandle);
        }
        public List<AccountService> GetAccountServices()
        {
            return GetItemsSP("Retail.sp_AccountService_GetAll", AccountServiceMapper);
        }

        public bool UpdateStatus(long accountServiceId, Guid statusId)
        {
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_AccountService_UpdateStatusID", accountServiceId, statusId);
            return (affectedRecords > 0);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private AccountService AccountServiceMapper(IDataReader reader)
        {
            AccountService accountService = new AccountService
            {
                AccountServiceId = (long)reader["ID"],
                AccountId = (long)reader["AccountID"],
                ServiceTypeId = GetReaderValue<Guid>(reader,"ServiceTypeId"),
                ServiceChargingPolicyId = GetReaderValue<int?>(reader,"ServiceChargingPolicyId"),
                Settings = Vanrise.Common.Serializer.Deserialize<AccountServiceSettings>(reader["Settings"] as string),
                StatusId = GetReaderValue<Guid>(reader, "StatusID")
            };
            return accountService;
        }

        #endregion
    }
}
