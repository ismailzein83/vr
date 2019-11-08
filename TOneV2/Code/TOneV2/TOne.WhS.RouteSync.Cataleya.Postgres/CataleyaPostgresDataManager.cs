using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Postgres
{
    public class CataleyaPostgresDataManager : BasePostgresDataManager, ICataleyaDataManager
    {
        public Guid ConfigId { get { return new Guid("CAD5F46B-F182-462A-AACC-B862ACD11AEB"); } }
        public DatabaseConnection DatabaseConnection { get; set; }

        public object PrepareDataForApply(List<ConvertedRoute> routes)
        {
            throw new NotImplementedException();
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public void PrepareTables(IRouteInitializeContext context)
        {
            var connectionString = GetConnectionString(DatabaseConnection.DBConnectionId);

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

        public List<CarrierAccountVersionInfo> GetCarrierAccountsPreviousVersionNumbers(IGetCarrierAccountsPreviousVersionNumbersContext context)
        {
            var connectionString = GetConnectionString(DatabaseConnection.DBConnectionId);
            var carrierAccountDataManager = new CarrierAccountDataManager(DatabaseConnection.SchemaName, connectionString);
           
            return carrierAccountDataManager.GetCarrierAccountsPreviousVersionNumbers(context.CarrierAccountIds);
        }

        #region Private

        private string GetConnectionString(Guid vrConnectionId)
        {
            var connection = new VRConnectionManager().GetVRConnection(vrConnectionId);
            var settings = connection.Settings as SQLConnection;

            return settings.ConnectionString;
        }

        #endregion
    }
}