using System;
using System.Collections.Generic;
using System.Text;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Data.Postgres;
using Vanrise.Common;
using System.Linq;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class CataleyaPostgresDataManager : BasePostgresDataManager, ICataleyaDataManager
    {
        public Guid ConfigId { get { return new Guid("CAD5F46B-F182-462A-AACC-B862ACD11AEB"); } }
        public DatabaseConnection DatabaseConnection { get; set; }

        string connectionString;
        public CataleyaPostgresDataManager()
        {
            connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);
        }

        protected override string GetConnectionString()
        {
            return connectionString;
        }

        #region Public Methods
        public List<CarrierAccountMapping> GetCarrierAccountMappings(bool getFromTemp)
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, connectionString);
            return carrierAccountDataManager.GetCarrierAccountMappings(getFromTemp);
        }

        public object PrepareDataForApply(List<ConvertedRoute> routes)
        {
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);
            return routeDataManager.PrepareDataForApply(routes);
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);

            routeDataManager.ApplyCataleyaRoutesToDB(context);
        }

        public void Initialize(ICataleyaInitializeContext context)
        {
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, connectionString);
            customerIdentificationDataManager.Initialize(context.CustomersIdentification);

            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, connectionString);
            carrierAccountDataManager.Initialize(context.CarrierAccountsMapping);

            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);
            routeDataManager.Initialize(context.CarrierAccountsMapping);
        }

        public List<CarrierAccountMapping> GetAllCarrierAccountsMapping(bool getFromTemp)
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, connectionString);
            return carrierAccountDataManager.GetCarrierAccountMappings(getFromTemp);
        }

        public List<CustomerIdentification> GetAllCustomerIdentifications(bool getFromTemp)
        {
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, connectionString);
            return customerIdentificationDataManager.GetAllCustomerIdentifications(getFromTemp);
        }

        public void Finalize(ICataleyaFinalizeContext context)
        {
            var carrierFinalizationDataByCustomer = context.CarrierFinalizationDataByCustomer;

            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, connectionString);
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, connectionString);
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);

            foreach (var carrierFinalizationData in carrierFinalizationDataByCustomer.Values)
            {
                var queryCommands = new StringBuilder();
                queryCommands.AppendLine("Begin;");

                if (carrierFinalizationData.CarrierAccountMappingToAdd != null)
                {
                    queryCommands.AppendLine(carrierAccountDataManager.GetAddCarrierAccountMappingQuery(carrierFinalizationData.CarrierAccountMappingToAdd));
                }

                if (carrierFinalizationData.CustomerIdentificationsToAdd != null && carrierFinalizationData.CustomerIdentificationsToAdd.Count > 0)
                {
                    foreach (var customerIdentificationToAdd in carrierFinalizationData.CustomerIdentificationsToAdd)
                    {
                        queryCommands.AppendLine(customerIdentificationDataManager.GeAddCustomerIdentificationQuery(customerIdentificationToAdd));
                    }
                }

                if (carrierFinalizationData.CustomerIdentificationsToDelete != null && carrierFinalizationData.CustomerIdentificationsToDelete.Count > 0)
                {
                    foreach (var customerIdentificationToDelete in carrierFinalizationData.CustomerIdentificationsToDelete)
                    {
                        queryCommands.AppendLine(customerIdentificationDataManager.GetDeleteCustomerIdentificationQuery(customerIdentificationToDelete));
                    }
                }

                if (carrierFinalizationData.CarrierAccountMappingToUpdate != null)
                {
                    var carrierAccountMappingToUpdate = carrierFinalizationData.CarrierAccountMappingToUpdate;
                    queryCommands.AppendLine(carrierAccountDataManager.GetUpdateCarrierAccountMappingQuery(carrierAccountMappingToUpdate));

                    var backupVersionNumber = (carrierAccountMappingToUpdate.CarrierId + 1) % 2;
                    string tableName = Helper.BuildRouteTableName(carrierAccountMappingToUpdate.CarrierId, backupVersionNumber);
                    var dropQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery(tableName);
                    queryCommands.AppendLine(dropQuery);
                }

                queryCommands.AppendLine("Commit;");

                ExecuteNonQuery(new string[] { queryCommands.ToString() });
            };

            ExecuteNonQuery(new string[] { carrierAccountDataManager.GetDropTempCarrierAccountMappingTableQuery(),
                customerIdentificationDataManager.GetDropTempCustomerIdentificationTableQuery(),
                routeDataManager.GetCreateRouteTablesIndexesQuery(context.RouteTableNames) });
        }

        public bool UpdateCarrierAccountMappingStatus(String customerId, CarrierAccountStatus status)
        {
            var carrierAccountDataManager = new CarrierAccountMappingDataManager(DatabaseConnection.SchemaName, connectionString);
            return carrierAccountDataManager.UpdateCarrierAccountMappingStatus(customerId, status);
        }

        #endregion

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