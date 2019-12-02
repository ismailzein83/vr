using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        int? parallelIndexCount { get; set; }
        public int ParallelIndexCount
        {
            get
            {
                if (!parallelIndexCount.HasValue)
                    parallelIndexCount = 1;

                return parallelIndexCount.Value;
            }
            set
            {
                parallelIndexCount = value;
            }
        }

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

        protected override string GetConnectionString() { return ConnectionString; }

        object obj = new object();
        #region Carrier Account Mappings

        public List<CarrierAccountMapping> GetCarrierAccountMappings(bool getFromTemp)
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, ConnectionString);
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
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, ConnectionString);
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

            ConcurrentQueue<FinalizeItem> queryCommands = new ConcurrentQueue<FinalizeItem>();
            List<FinalizeItem> routeTableFinalizeItems = routeDataManager.GetCreateRouteTablesIndexesQuery(context.RouteTableNamesForIndexes);
            if (routeTableFinalizeItems != null)
            {
                foreach (FinalizeItem item in routeTableFinalizeItems)
                {
                    queryCommands.Enqueue(item);
                }
            }

            ConcurrentQueue<FinalizeItem> queryCommands2 = new ConcurrentQueue<FinalizeItem>();
            queryCommands2.Enqueue(new FinalizeItem() { Description = "Drop Temporary Carrier Account Mapping Table", Query = carrierAccountDataManager.GetDropTempCarrierAccountMappingTableQuery() });
            queryCommands2.Enqueue(new FinalizeItem() { Description = "Drop Temporary Customer Identification Table", Query = customerIdentificationDataManager.GetDropTempCustomerIdentificationTableQuery() });

            var finalCustomerDataByAccountId = context.FinalCustomerDataByAccountId;

            foreach (var finalCustomerData in finalCustomerDataByAccountId.Values)
            {
                if (finalCustomerData.CarrierAccountMappingToAdd != null)
                {
                    queryCommands2.Enqueue(new FinalizeItem() { Description = $"Add Carrier Account Mapping for account {finalCustomerData.CarrierAccountMappingToAdd.CarrierId}", Query = carrierAccountDataManager.GetAddCarrierAccountMappingQuery(finalCustomerData.CarrierAccountMappingToAdd) });
                }

                if (finalCustomerData.CarrierAccountMappingToUpdate != null)
                {
                    var carrierAccountMappingToUpdate = finalCustomerData.CarrierAccountMappingToUpdate;
                    queryCommands2.Enqueue(new FinalizeItem() { Description = $"Update Carrier Account Mapping for account {carrierAccountMappingToUpdate.CarrierId}", Query = carrierAccountDataManager.GetUpdateCarrierAccountMappingQuery(carrierAccountMappingToUpdate) });

                    if (!string.IsNullOrEmpty(carrierAccountMappingToUpdate.RouteTableName))
                    {
                        var backupVersionNumber = (carrierAccountMappingToUpdate.Version + 1) % 3;
                        string tableName = Helper.BuildRouteTableName(carrierAccountMappingToUpdate.CarrierId, backupVersionNumber);
                        var dropQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery(tableName);
                        queryCommands2.Enqueue(new FinalizeItem() { Description = $"Drop Route table {tableName}", Query = dropQuery });
                    }
                }
                if (finalCustomerData.CarrierAccountMappingToDelete != null)
                {
                    var carrierAccountMappingToDelete = finalCustomerData.CarrierAccountMappingToDelete;
                    var dropQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery(carrierAccountMappingToDelete.RouteTableName);
                    queryCommands2.Enqueue(new FinalizeItem() { Description = $"Drop Route table {carrierAccountMappingToDelete.RouteTableName}", Query = dropQuery });

                    var backupVersionNumber = (carrierAccountMappingToDelete.Version + 2) % 3;
                    string tableName = Helper.BuildRouteTableName(carrierAccountMappingToDelete.CarrierId, backupVersionNumber);
                    var dropBackupQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery(tableName);

                    queryCommands2.Enqueue(new FinalizeItem() { Description = $"Delete Carrier Account Mapping for account {carrierAccountMappingToDelete.CarrierId}", Query = carrierAccountDataManager.GetDeleteCarrierAccountMappingQuery(carrierAccountMappingToDelete) });

                    queryCommands2.Enqueue(new FinalizeItem() { Description = $"Drop Route table {tableName}", Query = dropBackupQuery });
                }

                if (finalCustomerData.CustomerIdentificationsToAdd != null && finalCustomerData.CustomerIdentificationsToAdd.Count > 0)
                {
                    foreach (var customerIdentificationToAdd in finalCustomerData.CustomerIdentificationsToAdd)
                    {
                        queryCommands2.Enqueue(new FinalizeItem() { Description = $"Add Customer Identification for account {customerIdentificationToAdd.CarrierId} and trunk {customerIdentificationToAdd.Trunk}", Query = customerIdentificationDataManager.GeAddCustomerIdentificationQuery(customerIdentificationToAdd) });
                    }
                }

                if (finalCustomerData.CustomerIdentificationsToDelete != null && finalCustomerData.CustomerIdentificationsToDelete.Count > 0)
                {
                    foreach (var customerIdentificationToDelete in finalCustomerData.CustomerIdentificationsToDelete)
                    {
                        queryCommands2.Enqueue(new FinalizeItem() { Description = $"Delete Customer Identification for account {customerIdentificationToDelete.CarrierId} and trunk {customerIdentificationToDelete.Trunk}", Query = customerIdentificationDataManager.GetDeleteCustomerIdentificationQuery(customerIdentificationToDelete) });
                    }
                }
            };

            List<Task> tasks = new List<Task>();
            int remainingIndexes = queryCommands.Count;
            for (int i = 0; i < ParallelIndexCount; i++)
            {
                Task task = new Task(() =>
                {
                    FinalizeItem item = null;
                    Func<string> getNextQuery = () =>
                        {
                            if (!queryCommands.TryDequeue(out item))
                                return null;
                            return item.Query;
                        };

                    ExecuteNonQuery(getNextQuery, () =>
                    {
                        lock (obj)
                        {
                            remainingIndexes--;
                            string text = remainingIndexes > 1 ? "es" : "";
                            context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, $"{item.Description} is done. {remainingIndexes} index{text} remaining.", null);
                        }
                    });
                });
                tasks.Add(task);
                task.Start();
            }
            Task.WaitAll(tasks.ToArray());

            int remainingQueries = queryCommands2.Count;
            FinalizeItem item2 = null;
            Func<string> getNextQuery2 = () =>
            {
                if (!queryCommands2.TryDequeue(out item2))
                    return null;
                return item2.Query;
            };

            ExecuteNonQuery(getNextQuery2, () =>
            {
                remainingQueries--;
                string text = remainingQueries > 1 ? "ies" : "y";
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, $"{item2.Description} is done. {remainingQueries} quer{text} remaining.", null);
            });
        }

        public void ApplyDifferentialRoutes(ICataleyaApplyDifferentialRoutesContext context)
        {
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, ConnectionString);
            var updateRoutesQueryByRouteTableName = new ConcurrentQueue<FinalizeItem>();

            foreach (var kvp in context.RoutesByRouteTableName)
            {
                var query = routeDataManager.GetUpdateCarrierRoutesQuery(kvp.Key, kvp.Value);
                updateRoutesQueryByRouteTableName.Enqueue(new FinalizeItem() { Description = kvp.Key, Query = query });
            }

            int remainingTables = updateRoutesQueryByRouteTableName.Count;
            FinalizeItem item = null;
            Func<string> getNextQuery2 = () =>
            {
                if (!updateRoutesQueryByRouteTableName.TryDequeue(out item))
                    return null;
                return item.Query;
            };

            ExecuteNonQuery(getNextQuery2, () =>
            {
                remainingTables--;
                string text = remainingTables > 1 ? "s" : "";
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, $"Updating table {item.Description} is done. {remainingTables} table{text} remaining.", null);
            });
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