﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public List<TimeZonesByProfile> GetTimeZonesByProfile()
        {
            return GetItemsText(query_getTimeZonesByProfile, TimeZonesByProfileMapper, null);
        }

        TimeZonesByProfile TimeZonesByProfileMapper(IDataReader reader)
        {
            return new TimeZonesByProfile
            {
                CarrierProfileId = (short)reader["ProfileID"],
                CustomerTimeZoneId = (short)reader["CustomerGMTTimeID"],
                SupplierTimeZoneId = (short)reader["SupplierGMTTimeID"]
            };
        }
        private SourceCarrierAccount SourceCarrierAccountMapper(IDataReader reader)
        {
            string isNettingEnabled = reader["IsNettingEnabled"] as string;
            SourceCarrierAccount sourceCarrierAccount = new SourceCarrierAccount()
            {
                SourceId = reader["CarrierAccountID"].ToString(),
                NameSuffix = reader["NameSuffix"] as string,
                ProfileId = (short)reader["ProfileID"],
                ServicesFlag = (short)reader["ServicesFlag"],
                ActivationStatus = (SourceActivationStatus)reader["ActivationStatus"],
                RoutingStatus = (SourceRoutingStatus)reader["RoutingStatus"],
                NominalCapacityInE1s = GetReaderValue<int?>(reader, "NominalCapacityInE1s"),
                CustomerGMTTime = GetReaderValue<short?>(reader, "CustomerGMTTime"),
                GMTTime = GetReaderValue<short?>(reader, "GMTTime"),
                AccountType = (SourceAccountType)reader["AccountType"],
                CurrencyId = reader["CurrencyID"] as string,
                CarrierMask = reader["CarrierMask"] as string,
                IsDeleted = (reader["IsDeleted"] as string).Equals("Y", System.StringComparison.InvariantCultureIgnoreCase),
                IsAToZ = (reader["IsAToZ"] as string).Equals("Y", System.StringComparison.InvariantCultureIgnoreCase),
                RepresentsASwitch = (reader["RepresentsASwitch"]).Equals("Y"),
                CustomerActivateDate = GetReaderValue<DateTime?>(reader, "CustomerActivateDate"),
                SupplierActivateDate = GetReaderValue<DateTime?>(reader, "SupplierActivateDate"),
                CustomerDeactivateDate = GetReaderValue<DateTime?>(reader, "CustomerDeactivateDate"),
                SupplierDeactivateDate = GetReaderValue<DateTime?>(reader, "SupplierDeactivateDate"),
                IsNettingEnabled = string.IsNullOrEmpty(isNettingEnabled) || isNettingEnabled.Equals("N") ? false : true,
                CustomerCreditLimit = GetReaderValue<int?>(reader, "CustomerCreditLimit"),
                SupplierCreditLimit = GetReaderValue<int?>(reader, "SupplierCreditLimit"),
                CustomerPaymentType = (SourcePaymentType)GetReaderValue<byte>(reader, "CustomerPaymentType"),
                SupplierPaymentType = (SourcePaymentType)GetReaderValue<byte>(reader, "SupplierPaymentType")
            };
            return sourceCarrierAccount;
        }

        const string query_getSourceCarrierAccounts = @"SELECT  ca.CarrierAccountID CarrierAccountID, ca.ProfileID ProfileID, ca.ActivationStatus ActivationStatus, ca.RoutingStatus RoutingStatus,  
                                                                ca.AccountType AccountType, ca.NameSuffix NameSuffix, ca.ServicesFlag ServicesFlag, ca.GMTTime GMTTime, ca.CustomerGMTTime CustomerGMTTime,
                                                                cp.CurrencyID CurrencyID, ca.CarrierMask CarrierMask , ca.IsDeleted IsDeleted, ca.NominalCapacityInE1s NominalCapacityInE1s, ca.IsAToZ IsAToZ, ca.RepresentsASwitch,
                                                                ca.[CustomerActivateDate]
                                                              ,ca.[CustomerDeactivateDate]
                                                              ,ca.[SupplierActivateDate]
                                                              ,ca.[SupplierDeactivateDate]
                                                              ,ca.[IsNettingEnabled]
                                                              ,ca.[CustomerCreditLimit]
                                                              ,ca.[CustomerPaymentType]
                                                              ,ca.[SupplierPaymentType]
                                                              ,ca.[SupplierCreditLimit]
                                                                FROM  CarrierAccount ca WITH (NOLOCK) INNER JOIN CarrierProfile cp 
                                                                ON ca.ProfileID = cp.ProfileID 
                                                                Where ca.CarrierAccountID <> 'SYS'";

        const string query_getTimeZonesByProfile = @"   SELECT	ca.ProfileID ProfileID, tzs.ID SupplierGMTTimeID, tzc.ID CustomerGMTTimeID
                                                        FROM    CarrierAccount ca WITH (NOLOCK)
                                                        join	CustomTimeZoneInfo tzc on tzc.BaseUtcOffset = ca.CustomerGMTTime
                                                        join	CustomTimeZoneInfo tzs on tzs.BaseUtcOffset = ca.GMTTime
                                                        Where   ca.CarrierAccountID <> 'SYS'";
    }
}
