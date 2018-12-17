﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CustomerZoneDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICustomerZoneDataManager
    {
        #region ctor/Local Variables
        public CustomerZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<CustomerZones> GetAllCustomerZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerZone_GetAll", CustomerZonesMapper);
        }
        public bool AreAllCustomerZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[CustomerZone]", ref updateHandle);
        }
        public bool AddCustomerZones(CustomerZones customerZones, out int insertedId)
        {
            object customerZonesId;

            string serializedCountries = Vanrise.Common.Serializer.Serialize(customerZones.Countries);

            int recordsAffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerZone_Insert", out customerZonesId, customerZones.CustomerId, serializedCountries, customerZones.StartEffectiveTime);

            insertedId = (int)customerZonesId;

            return (recordsAffected > 0);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers

        private CustomerZones CustomerZonesMapper(IDataReader reader)
        {
            CustomerZones customerZones = new CustomerZones();

            customerZones.CustomerZonesId = (int)reader["ID"];
            customerZones.CustomerId = (int)reader["CustomerID"];
            customerZones.Countries = Vanrise.Common.Serializer.Deserialize<List<CustomerCountry>>(reader["Details"] as string);
            customerZones.StartEffectiveTime = (DateTime)reader["BED"];

            return customerZones;
        }

        #endregion
    }

    public class CustomerCountryDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICustomerCountryDataManager
    {
        #region Constructors

        public CustomerCountryDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<CustomerCountry2> GetAll()
        {
            return base.GetItemsSP("TOneWhS_BE.sp_CustomerCountry_GetAll", CustomerCountryMapper);
        }

        public bool AreAllCustomerCountriesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CustomerCountry", ref updateHandle);
        }

        #endregion
        #region StateBackup Methods
        public string BackupAllSaleEntityCustomerCountryBySellingNumberPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[CustomerCountry] WITH (TABLOCK)
                                                   ([ID]
                                                   ,[CustomerID]
                                                   ,[CountryID]
                                                   ,[BED]
                                                   ,[EED]
                                                   ,[StateBackupID]
                                                   ,[ProcessInstanceID]
                                                   ,LastModifiedTime)
                                           SELECT  CC.[ID]
                                                  ,CC.[CustomerID]
                                                  ,CC.[CountryID]
                                                  ,CC.[BED]
                                                  ,CC.[EED]
	                                              ,{1} AS StateBackupID 
                                                  ,CC.[ProcessInstanceID]
                                                  ,cc.[LastModifiedTime]
                                              FROM [TOneWhS_BE].[CustomerCountry] CC WITH (NOLOCK)
                                              JOIN [TOneWhS_BE].[CarrierAccount] CA ON CA.ID = CC.[CustomerID]  
                                              WHERE CA.SellingNumberPlanID ={2}", backupDatabase, stateBackupId,
                sellingNumberPlanId);
        }

        public string BackupSaleEntityCustomerCountryByOwner(long stateBackupId, string backupDatabase, IEnumerable<int> customerIds)
        {
            string customerIdsString = null;
            if (customerIds != null && customerIds.Any())
                customerIdsString = string.Join<int>(",", customerIds);

            return String.Format(@"INSERT INTO {0}.[TOneWhS_BE_Bkup].[CustomerCountry] WITH (TABLOCK)
                                               (ID
                                               ,CustomerID
                                               ,CountryID
                                               ,BED
                                               ,EED
                                               ,ProcessInstanceID
                                               ,StateBackupID
                                               ,LastModifiedTime)
                                            SELECT cc.ID
                                              ,cc.CustomerID
                                              ,cc.CountryID
                                              ,cc.BED
                                              ,cc.EED
                                              ,cc.ProcessInstanceID
                                              ,{1}
                                              ,cc.LastModifiedTime
                                          FROM [TOneWhS_BE].[CustomerCountry] cc  WITH(NOLOCK) where CustomerID IN ( {2} )",
                backupDatabase,
                stateBackupId, customerIdsString);
        }
        public string GetDeleteCommandsByOwner(IEnumerable<int> customerIds)
        {
            string deleteQuery = string.Empty;
            if (customerIds != null && customerIds.Any())
            {
                string customerIdsString = string.Join<int>(",", customerIds);
                deleteQuery = String.Format(@"DELETE FROM [TOneWhS_BE].[CustomerCountry]
                                           Where CustomerID IN ({0})", customerIdsString);
            }
            return deleteQuery;
        }
        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"
                                    DELETE FROM [TOneWhS_BE].[CustomerCountry] 
                                    WHERE CustomerID IN ( SELECT ID FROM [TOneWhS_BE].[CarrierAccount]
                                    WHERE SellingNumberPlanID = {0})",
                sellingNumberPlanId);
        }
        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[CustomerCountry]
                                               ([ID]
                                               ,[CustomerID]
                                               ,[CountryID]
                                               ,[BED]
                                               ,[EED]
                                               ,[ProcessInstanceID]
                                               ,[LastModifiedTime])
                                    SELECT [ID]
                                          ,[CustomerID]
                                          ,[CountryID]
                                          ,[BED]
                                          ,[EED]
                                          ,[ProcessInstanceID]
                                          ,[LastModifiedTime]
                                      FROM {0}.[TOneWhS_BE_Bkup].[CustomerCountry] WITH (NOLOCK) Where StateBackupID = {1} ",
                backupDatabase, stateBackupId);
        }
        #endregion

        #region Mappers

        private CustomerCountry2 CustomerCountryMapper(IDataReader reader)
        {
            return new CustomerCountry2
            {
                CustomerCountryId = (int)reader["ID"],
                CustomerId = (int)reader["CustomerID"],
                CountryId = (int)reader["CountryID"],
                BED = (DateTime)reader["BED"],
                EED = base.GetReaderValue<DateTime?>(reader, "EED"),
                ProcessInstanceId = GetReaderValue<long?>(reader, "processInstanceId")
            };
        }

        #endregion
    }
}
