﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class CataleyaPostgresDataManager : BasePostgresDataManager, ICataleyaDataManager
    {
        public Guid ConfigId { get { return new Guid("CAD5F46B-F182-462A-AACC-B862ACD11AEB"); } }
        public DatabaseConnection DatabaseConnection { get; set; }

        string connectionString;
        string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                    connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);

                return connectionString;
            }
        }

        protected override string GetConnectionString()
        {
            return ConnectionString;
        }


        #region Carrier Account Mappings

        public List<CarrierAccountMapping> GetCarrierAccountMappings(bool getFromTemp)
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, connectionString);
            return carrierAccountDataManager.GetCarrierAccountMappings(getFromTemp);
        }

        public void InitializeCarrierAccountMappingTable()
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, ConnectionString);
            carrierAccountDataManager.Initialize();
        }

        public void FillTempCarrierAccountMappingTable(IEnumerable<CarrierAccountMapping> carrierAccountsMappings)
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, ConnectionString);
            carrierAccountDataManager.FillTempCarrierAccountMappingTable(carrierAccountsMappings);
        }

        #endregion

        #region Customer Identification
        public List<CustomerIdentification> GetCustomerIdentifications(bool getFromTemp)
        {
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, ConnectionString);
            return customerIdentificationDataManager.GetCustomerIdentifications(getFromTemp);
        }

        public void InitializeCustomerIdentificationTable()
        {
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, connectionString);
            customerIdentificationDataManager.Initialize();
        }

        public void FillTempCustomerIdentificationTable(IEnumerable<CustomerIdentification> customerIdentifications)
        {
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, ConnectionString);
            customerIdentificationDataManager.FillTempCustomerIdentificationTable(customerIdentifications);
        }

        #endregion

        #region Routes
        public void InitializeRouteTables(IEnumerable<CarrierAccountMapping> carrierAccountsMappings)
        {
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, ConnectionString);
            routeDataManager.Initialize(carrierAccountsMappings);
        }

        public object PrepareDataForApply(List<ConvertedRoute> routes)
        {
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, ConnectionString);
            return routeDataManager.PrepareDataForApply(routes);
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, ConnectionString);
            routeDataManager.ApplyCataleyaRoutesToDB(context);
        }

        #endregion

        public void Finalize(ICataleyaFinalizeContext context)
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, ConnectionString);
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, ConnectionString);
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, ConnectionString);

            Dictionary<string, string> queryCommands = routeDataManager.GetCreateRouteTablesIndexesQuery(context.RouteTableNamesForIndexes);

            queryCommands.Add("Drop Temporary Carrier Account Mapping Table", carrierAccountDataManager.GetDropTempCarrierAccountMappingTableQuery());
            queryCommands.Add("Drop Temporary Customer Identification Table", customerIdentificationDataManager.GetDropTempCustomerIdentificationTableQuery());

            var finalCustomerDataByAccountId = context.FinalCustomerDataByAccountId;

            foreach (var finalCustomerData in finalCustomerDataByAccountId.Values)
            {
                if (finalCustomerData.CarrierAccountMappingToAdd != null)
                {
                    queryCommands.Add($"Add Carrier Account Mapping for account {finalCustomerData.CarrierAccountMappingToAdd.CarrierId}", carrierAccountDataManager.GetAddCarrierAccountMappingQuery(finalCustomerData.CarrierAccountMappingToAdd));
                }

                if (finalCustomerData.CarrierAccountMappingToUpdate != null)
                {
                    var carrierAccountMappingToUpdate = finalCustomerData.CarrierAccountMappingToUpdate;
                    queryCommands.Add($"Update Carrier Account Mapping for account {carrierAccountMappingToUpdate.CarrierId}", carrierAccountDataManager.GetUpdateCarrierAccountMappingQuery(carrierAccountMappingToUpdate));

                    if (!string.IsNullOrEmpty(carrierAccountMappingToUpdate.RouteTableName))
                    {
                        var backupVersionNumber = (carrierAccountMappingToUpdate.Version + 1) % 3;
                        string tableName = Helper.BuildRouteTableName(carrierAccountMappingToUpdate.CarrierId, backupVersionNumber);
                        var dropQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery(tableName);
                        queryCommands.Add($"Drop Route table {tableName}", dropQuery);
                    }
                }
                if (finalCustomerData.CarrierAccountMappingToDelete != null)
                {
                    var carrierAccountMappingToDelete = finalCustomerData.CarrierAccountMappingToDelete;
                    var dropQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery(carrierAccountMappingToDelete.RouteTableName);
                    queryCommands.Add($"Drop Route table {carrierAccountMappingToDelete.RouteTableName}", dropQuery);

                    var backupVersionNumber = (carrierAccountMappingToDelete.Version + 2) % 3;
                    string tableName = Helper.BuildRouteTableName(carrierAccountMappingToDelete.CarrierId, backupVersionNumber);
                    var dropBackupQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery(tableName);

                    queryCommands.Add($"Delete Carrier Account Mapping for account {carrierAccountMappingToDelete.CarrierId}", carrierAccountDataManager.GetDeleteCarrierAccountMappingQuery(carrierAccountMappingToDelete));

                    queryCommands.Add($"Drop Route table {tableName}", dropBackupQuery);
                }

                if (finalCustomerData.CustomerIdentificationsToAdd != null && finalCustomerData.CustomerIdentificationsToAdd.Count > 0)
                {
                    foreach (var customerIdentificationToAdd in finalCustomerData.CustomerIdentificationsToAdd)
                    {
                        queryCommands.Add($"Add Customer Identification for account {customerIdentificationToAdd.CarrierId} and trunk {customerIdentificationToAdd.Trunk}", customerIdentificationDataManager.GeAddCustomerIdentificationQuery(customerIdentificationToAdd));
                    }
                }

                if (finalCustomerData.CustomerIdentificationsToDelete != null && finalCustomerData.CustomerIdentificationsToDelete.Count > 0)
                {
                    foreach (var customerIdentificationToDelete in finalCustomerData.CustomerIdentificationsToDelete)
                    {
                        queryCommands.Add($"Delete Customer Identification for account {customerIdentificationToDelete.CarrierId} and trunk {customerIdentificationToDelete.Trunk}", customerIdentificationDataManager.GetDeleteCustomerIdentificationQuery(customerIdentificationToDelete));
                    }
                }
            };

            int index = 0;

            Func<string> getNextQuery = () =>
            {
                if (index >= queryCommands.Count)
                {
                    var oldKvp = queryCommands.ElementAt(index - 1);
                    context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Concat(oldKvp.Key, " has ended"), null);
                    return null;
                }

                if (index != 0)
                {
                    var oldKvp = queryCommands.ElementAt(index - 1);
                    context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Concat(oldKvp.Key, " has ended"), null);
                }

                var currentKvp = queryCommands.ElementAt(index++);
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Concat(currentKvp.Key, " has started"), null);

                return currentKvp.Value;
            };

            ExecuteNonQuery(getNextQuery);
        }

        //public bool UpdateCarrierAccountMappingStatus(String customerId, CarrierAccountStatus status)
        //{
        //    var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, ConnectionString);
        //    return carrierAccountDataManager.UpdateCarrierAccountMappingStatus(customerId, status);
        //}

        #region Private

        private string GetConnectionStringById(Guid vrConnectionId)
        {
            var vrConnection = new VRConnectionManager().GetVRConnection(vrConnectionId);
            SQLConnection sqlConnection = vrConnection.Settings.CastWithValidate<SQLConnection>("connection.Settings", vrConnectionId);
            return sqlConnection.ConnectionString;
        }

        #endregion
    }
}