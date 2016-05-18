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
                NameSuffix = arg["NameSuffix"] as string,
                ProfileId = (short)arg["ProfileID"],
                CarrierAccountID = arg["CarrierAccountID"] as string,
                ActivationStatus = (SourceActivationStatus)arg["ActivationStatus"],
                AccountType = (SourceAccountType)arg["AccountType"],
                CurrencyID = arg["CurrencyID"] as string,
                CarrierMask = arg["CarrierMask"] as string,
            };
            return sourceCarrierAccount;
        }

        const string query_getSourceCarrierAccounts = @"SELECT  ca.CarrierAccountID CarrierAccountID, ca.ProfileID ProfileID, ca.ActivationStatus ActivationStatus,  
                                                                ca.AccountType AccountType, ca.NameSuffix NameSuffix, 
                                                                cp.CurrencyID CurrencyID, ca.CarrierMask CarrierMask 
                                                                FROM  CarrierAccount ca WITH (NOLOCK) INNER JOIN CarrierProfile cp 
                                                                ON ca.ProfileID = cp.ProfileID ";
    }
}
