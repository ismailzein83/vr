using System;
using System.Collections.Generic;
using System.Text;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class CataleyaPostgresDataManager : BasePostgresDataManager, ICataleyaDataManager
    {
        public Guid ConfigId { get { return new Guid("CAD5F46B-F182-462A-AACC-B862ACD11AEB"); } }
        public DatabaseConnection DatabaseConnection { get; set; }

        protected override string GetConnectionString()
        {
            return GetConnectionStringById(DatabaseConnection.DBConnectionId);
        }

        #region Public Methods

        public object PrepareDataForApply(List<ConvertedRoute> routes)
        {
            var connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);

            return routeDataManager.PrepareDataForApply(routes);
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            var connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);

            routeDataManager.ApplyCataleyaRoutesToDB(context);
        }

        public void PrepareTables(IRouteInitializeContext context)
        {
            var connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);

            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, connectionString);
            var carrierAccountDataManager = new CarrierAccountDataManager(DatabaseConnection.SchemaName, connectionString);
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);

            customerIdentificationDataManager.CreateCustomerIdentificationTableIfNotExist();
            carrierAccountDataManager.CreateCarrierAccountTableIfNotExist();

            customerIdentificationDataManager.DropIfExistsCreateTempCustomerIdentificationTable();
            carrierAccountDataManager.CreateCarrierAccountTableIfNotExist();

            customerIdentificationDataManager.AddCustomerIdentificationsToTempTable(context.CustomersIdentification);
            carrierAccountDataManager.AddCarrierAccountsMappingToTempTable(context.CarrierAccountsMapping);

            routeDataManager.DropIfExistsCreateRouteTables(context.RouteTableNames);
        }

        public List<CarrierAccountVersionInfo> GetCarrierAccountsPreviousVersion(IGetCarrierAccountsPreviousVersionNumbersContext context)
        {
            var connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);
            var carrierAccountDataManager = new CarrierAccountDataManager(DatabaseConnection.SchemaName, connectionString);

            return carrierAccountDataManager.GetCarrierAccountsPreviousVersionNumbers(context.CarrierAccountIds);
        }

        public List<CarrierAccountMapping> GetAllCarrierAccountsMapping(bool getFromTemp)
        {
            var connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);
            var carrierAccountDataManager = new CarrierAccountDataManager(DatabaseConnection.SchemaName, connectionString);

            return carrierAccountDataManager.GetAllCarrierAccountsMapping(getFromTemp);
        }

        public List<CustomerIdentification> GetAllCustomerIdentifications(bool getFromTemp)
        {
            var connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, connectionString);

            return customerIdentificationDataManager.GetAllCustomerIdentifications(getFromTemp);
        }

        public void Finalize(CataleyaFinalizeContext context)
        {
            var carrierFinalizationDataByCustomer = context.CarrierFinalizationDataByCustomer;
            var connectionString = GetConnectionStringById(DatabaseConnection.DBConnectionId);

            var carrierAccountDataManager = new CarrierAccountDataManager(DatabaseConnection.SchemaName, connectionString);
            var customerIdentificationDataManager = new CustomerIdentificationDataManager(DatabaseConnection.SchemaName, connectionString);
            var routeDataManager = new RouteDataManager(DatabaseConnection.SchemaName, connectionString);

            foreach (var keyValuePair in carrierFinalizationDataByCustomer)
            {
                var queryCommands = new StringBuilder();
                queryCommands.AppendLine("Begin;");
                var carrierFinalizationData = keyValuePair.Value;

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
                    var dropQuery = routeDataManager.GetDropBackUpRouteTableIfExistsQuery($"Rt_{carrierAccountMappingToUpdate.CarrierId.ToString()}_{backupVersionNumber.ToString()}");
                    queryCommands.AppendLine(dropQuery);
                }

                queryCommands.AppendLine("Commit;");

                ExecuteNonQuery(new string[] { queryCommands.ToString() });
            };

            ExecuteNonQuery(new string[] { carrierAccountDataManager.GetDropTempCarrierAccountMappingTableQuery(), customerIdentificationDataManager.GetDropTempCustomerIdentificationTableQuery() });

            //Create Indexes
        }

        #endregion

        #region Private

        private string GetConnectionStringById(Guid vrConnectionId)
        {
            var connection = new VRConnectionManager().GetVRConnection(vrConnectionId);
            var settings = connection.Settings as SQLConnection;

            return settings.ConnectionString;
        }

        #endregion
    }
}