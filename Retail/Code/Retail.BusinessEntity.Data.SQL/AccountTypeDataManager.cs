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
    public class AccountTypeDataManager : BaseSQLDataManager, IAccountTypeDataManager
    {
        #region Constructors

        public AccountTypeDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<AccountType> GetAccountTypes()
        {
            return GetItemsSP("Retail_BE.sp_AccountType_GetAll", AccountTypeMapper);
        }

        public bool Insert(AccountType accountType, out int insertedId)
        {
            object accountTypeId;
            string serializedSettings = accountType.Settings != null ? Vanrise.Common.Serializer.Serialize(accountType.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_AccountType_Insert", out accountTypeId, accountType.Name, accountType.Title, serializedSettings);

            if (affectedRecords > 0)
            {
                insertedId = (int)accountTypeId;
                return true;
            }

            insertedId = -1;
            return false;
        }

        public bool Update(AccountTypeToEdit accountType)
        {
            string serializedSettings = accountType.Settings != null ? Vanrise.Common.Serializer.Serialize(accountType.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_AccountType_Update", accountType.AccountTypeId, accountType.Name, accountType.Title, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreAccountTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.AccountType", ref updateHandle);
        }

        #endregion

        #region Mappers

        private AccountType AccountTypeMapper(IDataReader reader)
        {
            return new AccountType()
            {
                AccountTypeId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<AccountTypeSettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}
