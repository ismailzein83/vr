﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountDataManager : BaseSQLDataManager, IAccountDataManager
    {
        #region Constructors

        public AccountDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<Account> GetAccounts()
        {
            return GetItemsSP("Retail.sp_Account_GetAll", AccountMapper);
        }

        public bool Insert(Account account, out long insertedId)
        {
            object accountId;
            string serializedSettings = account.Settings != null ? Vanrise.Common.Serializer.Serialize(account.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Insert", out accountId, account.Name, account.Type, serializedSettings, account.ParentAccountId);

            if (affectedRecords > 0)
            {
                insertedId = (int)accountId;
                return true;
            }

            insertedId = -1;
            return false;
        }

        public bool Update(AccountToEdit account, long? parentId)
        {
            string serializedSettings = account.Settings != null ? Vanrise.Common.Serializer.Serialize(account.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Update", account.AccountId, account.Name, account.Type, serializedSettings, parentId);
            return (affectedRecords > 0);
        }

        public bool AreAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.Account", ref updateHandle);
        }

        #endregion

        #region Mappers

        private Account AccountMapper(IDataReader reader)
        {
            return new Account()
            {
                AccountId = (long)reader["ID"],
                Name = reader["Name"] as string,
                Type = (AccountType)reader["Type"],
                Settings = Vanrise.Common.Serializer.Deserialize<AccountSettings>(reader["Settings"] as string),
                ParentAccountId = GetReaderValue<int?>(reader, "ParentID")
            };
        }

        #endregion
    }
}
