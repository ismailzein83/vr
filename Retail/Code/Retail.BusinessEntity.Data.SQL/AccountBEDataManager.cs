using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountBEDataManager : BaseSQLDataManager, IAccountBEDataManager
    {
        #region Constructors

        public AccountBEDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public bool UpdateExtendedSettings(long accountId, Dictionary<string, BaseAccountExtendedSettings> extendedSettings)
        {
            int recordsEffected = ExecuteNonQuerySP("Retail.sp_Account_UpdateExtendedSettings", accountId, Vanrise.Common.Serializer.Serialize(extendedSettings));
            return (recordsEffected > 0);
        }

        public IEnumerable<Account> GetAccounts(List<Guid> accountTypeIds, byte[] afterLastTimeStamp)
        {
            string query = String.Format(@"SELECT	a.ID, a.Name, a.TypeID, a.Settings, a.StatusID, a.ParentID, a.SourceID, a.ExecutedActionsData,a.ExtendedSettings,a.CreatedTime
	                            FROM [Retail].[Account] a with(nolock)
	                            where {0} ", BuildAccountTypesFilter(accountTypeIds));
            if (afterLastTimeStamp != null)
                query += " AND a.[timestamp] > @Timestamp";
            return GetItemsText(query, AccountMapper,
                (cmd) =>
                {
                    if (afterLastTimeStamp != null)
                        cmd.Parameters.Add(new SqlParameter("@Timestamp", afterLastTimeStamp));
                });
        }

        public byte[] GetMaxTimeStamp(List<Guid> accountTypeIds)
        {
            string query = String.Format(@"SELECT MAX(a.timestamp) 
                                            FROM [Retail].[Account] a with(nolock) 
                                            where {0}", BuildAccountTypesFilter(accountTypeIds));
            Object obj = ExecuteScalarText(query, null);
            return obj != null ? (byte[])obj : null;
        }

        public bool Insert(AccountToInsert accountToInsert, out long insertedId)
        {
            object accountId;
            string serializedSettings = accountToInsert.Settings != null ? Vanrise.Common.Serializer.Serialize(accountToInsert.Settings) : null;
            string serializedExtendedSettings = accountToInsert.ExtendedSettings != null ? Vanrise.Common.Serializer.Serialize(accountToInsert.ExtendedSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Insert", out accountId, accountToInsert.Name, accountToInsert.TypeId, serializedSettings, serializedExtendedSettings, accountToInsert.ParentAccountId, accountToInsert.StatusId, accountToInsert.SourceId, ToDBNullIfDefault(accountToInsert.CreatedTime));

            if (affectedRecords > 0)
            {
                insertedId = (int)accountId;
                return true;
            }

            insertedId = -1;
            return false;
        }

        public bool Update(AccountToEdit accountToEdit)
        {
            string serializedSettings = accountToEdit.Settings != null ? Vanrise.Common.Serializer.Serialize(accountToEdit.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Update", accountToEdit.AccountId, accountToEdit.Name, accountToEdit.TypeId, serializedSettings, accountToEdit.SourceId);
            return (affectedRecords > 0);
        }

        public bool AreAccountsUpdated(List<Guid> accountTypeIds, ref object updateHandle)
        {
            string query = String.Format(@"SELECT MAX(a.timestamp) 
                                            FROM [Retail].[Account] a with(nolock) 
                                            where {0}", BuildAccountTypesFilter(accountTypeIds));

            var newReceivedDataInfo = ExecuteScalarText(query, null);
            return base.IsDataUpdated(ref updateHandle, newReceivedDataInfo);
        
        }

        public bool UpdateStatus(long accountId, Guid statusId)
        {
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_UpdateStatusID", accountId, statusId);
            return (affectedRecords > 0);
        }
        #endregion

        #region Mappers

        private Account AccountMapper(IDataReader reader)
        {
            return new Account()
            {
                AccountId = (long)reader["ID"],
                Name = reader["Name"] as string,
                TypeId = GetReaderValue<Guid>(reader, "TypeID"),
                Settings = Vanrise.Common.Serializer.Deserialize<AccountSettings>(reader["Settings"] as string),
                ParentAccountId = GetReaderValue<long?>(reader, "ParentID"),
                StatusId = GetReaderValue<Guid>(reader, "StatusID"),
                SourceId = reader["SourceID"] as string,
                ExtendedSettings = Vanrise.Common.Serializer.Deserialize(reader["ExtendedSettings"] as string) as Dictionary<string, BaseAccountExtendedSettings>,
                CreatedTime = GetReaderValue<DateTime>(reader,"CreatedTime")
            };
        }

        private string BuildAccountTypesFilter(List<Guid> accountTypeIds)
        {
            return string.Concat(" a.TypeID IN ('", String.Join("','", accountTypeIds), "') ");
        }

        #endregion
    }
}
