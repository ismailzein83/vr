using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using System;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCarrierAccountDataManager : BaseSQLDataManager
    {
        public SourceCarrierAccountDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCarrierAccount> GetSourceCarrierAccounts()
        {
            return GetItemsText(query_getSourceCarrierAccounts, SourceCarrierAccountMapper, null);
        }

        private SourceCarrierAccount SourceCarrierAccountMapper(System.Data.IDataReader arg)
        {
            SourceCarrierAccount sourceCarrierAccount = new SourceCarrierAccount()
            {
                SourceId = arg["CarrierAccountID"].ToString(),
                NameSuffix = arg["NameSuffix"].ToString(),
                ProfileId = (Int16)arg["ProfileId"]
            };
            return sourceCarrierAccount;
        }

        const string query_getSourceCarrierAccounts = @"SELECT [CarrierAccountID]  ,[NameSuffix] , [ProfileId] FROM [dbo].[CarrierAccount] WITH (NOLOCK)";
    }
}
