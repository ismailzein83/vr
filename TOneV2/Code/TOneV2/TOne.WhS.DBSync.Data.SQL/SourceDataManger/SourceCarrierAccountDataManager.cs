using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

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

        private SourceCarrierAccount SourceCarrierAccountMapper(IDataReader reader)
        {
            SourceCarrierAccount sourceCarrierAccount = new SourceCarrierAccount()
            {
                SourceId = reader["CarrierAccountID"].ToString(),
                NameSuffix = reader["NameSuffix"] as string,
                ProfileId = (short)reader["ProfileID"],
                ServicesFlag = (short)reader["ServicesFlag"],
                ActivationStatus = (SourceActivationStatus)reader["ActivationStatus"],
                RoutingStatus = (SourceRoutingStatus)reader["RoutingStatus"],
                CustomerGMTTime = GetReaderValue<short?>(reader, "CustomerGMTTime"),
                GMTTime = GetReaderValue<short?>(reader, "GMTTime"),
                AccountType = (SourceAccountType)reader["AccountType"],
                CurrencyId = reader["CurrencyID"] as string,
                CarrierMask = reader["CarrierMask"] as string,
            };
            return sourceCarrierAccount;
        }

        const string query_getSourceCarrierAccounts = @"SELECT  ca.CarrierAccountID CarrierAccountID, ca.ProfileID ProfileID, ca.ActivationStatus ActivationStatus, ca.RoutingStatus RoutingStatus,  
                                                                ca.AccountType AccountType, ca.NameSuffix NameSuffix, ca.ServicesFlag ServicesFlag, ca.GMTTime GMTTime, ca.CustomerGMTTime CustomerGMTTime,
                                                                cp.CurrencyID CurrencyID, ca.CarrierMask CarrierMask 
                                                                FROM  CarrierAccount ca WITH (NOLOCK) INNER JOIN CarrierProfile cp 
                                                                ON ca.ProfileID = cp.ProfileID WHERE (ca.IsDeleted = 'N')";
    }
}
