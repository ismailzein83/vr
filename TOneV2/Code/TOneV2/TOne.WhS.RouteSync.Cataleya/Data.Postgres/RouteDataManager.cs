using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class RouteDataManager : BasePostgresDataManager
    {
        string schemaName;
        string connectionString;

        protected override string GetConnectionString()
        {
            return connectionString;
        }

        public RouteDataManager(string _schemaName, string _connectionString)
        {
            schemaName = _schemaName;
            connectionString = _connectionString;
        }

        #region Public Methods

        public object PrepareDataForApply(List<ConvertedRoute> routes)
        {
            if (routes == null || routes.Count == 0)
                return null;

            var carrierAccountDataManager = new CarrierAccountMappingDataManager(schemaName, connectionString);
            var convertedRoutesForApplyByCustomer = new Dictionary<int, CataleyaConvertedRoutesForApply>();
            var convertedRoutes = routes.VRCast<CataleyaConvertedRoute>();

            foreach (var convertedRoute in convertedRoutes)
            {
                if (convertedRoutesForApplyByCustomer.TryGetValue(convertedRoute.CarrierID, out var cataleyaConvertedRoutesForApply))
                {
                    cataleyaConvertedRoutesForApply.Routes.Add(convertedRoute);
                }
                else
                {
                    var carrierAccount = carrierAccountDataManager.GetCarrierAccountMapping(convertedRoute.CarrierID);

                    convertedRoutesForApplyByCustomer.Add(carrierAccount.CarrierId, new CataleyaConvertedRoutesForApply()
                    {
                        Routes = new List<CataleyaConvertedRoute>() { convertedRoute },
                        RouteTableName = carrierAccount.RouteTableName
                    });
                }
            }
            return convertedRoutesForApplyByCustomer;
        }

        public void ApplyCataleyaRoutesToDB(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            var convertedRoutesForApplyByCustomer = context.PreparedItemsForApply as Dictionary<int, CataleyaConvertedRoutesForApply>;

            foreach (var keyValuePair in convertedRoutesForApplyByCustomer)
            {
                var convertedRoutesForApply = keyValuePair.Value;
                var tableName = string.IsNullOrEmpty(schemaName) ? convertedRoutesForApply.RouteTableName : $"{schemaName}.{convertedRoutesForApply.RouteTableName}";
                base.Bulk(convertedRoutesForApply.Routes, tableName);
            }
        }

        public void Initialize(List<CarrierAccountMapping> carrierAccountsMapping)
        {
            DropIfExistsCreateRouteTables(carrierAccountsMapping.Select(item => item.RouteTableName).ToList());
        }

        public string GetDropBackUpRouteTableIfExistsQuery(string tableName)
        {
            var tableNameWithSchema = string.IsNullOrEmpty(schemaName) ? tableName : $"{schemaName}.{tableName}";
            return DropBackUpRouteTableIfExists_Query.Replace("#TABLENAMEWITHSCHEMA#", tableNameWithSchema);
        }

        public string GetCreateRouteTablesIndexesQuery(List<string> routeTablesNames)
        {
            var query = new StringBuilder();
            foreach (var routeTableName in routeTablesNames)
            {
                var tableNameWithSchema = string.IsNullOrEmpty(schemaName) ? routeTableName : $"{schemaName}.{routeTableName}";
                query.AppendLine(Createindex_Query.Replace("#TABLENAMEWITHSCHEMA#", tableNameWithSchema));
            }

            return query.ToString();
        }

        #endregion


        #region Private Methods

        void DropIfExistsCreateRouteTables(List<string> routeTableNames)
        {
            var dropIfExistsCreateTempCustomerIdentificationTableQueries = new List<string>();

            foreach (var routeTableName in routeTableNames)
            {
                var tableNameWithSchema = string.IsNullOrEmpty(schemaName) ? routeTableName : $"{schemaName}.{routeTableName}";
                var query = DropIfExistsCreateRouteTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tableNameWithSchema);

                dropIfExistsCreateTempCustomerIdentificationTableQueries.Add(query);
            }

            ExecuteNonQuery(dropIfExistsCreateTempCustomerIdentificationTableQueries.ToArray());
        }

        #endregion

        #region Queries 
        const string DropIfExistsCreateRouteTable_Query = @"DROP TABLE IF EXISTS #TABLENAMEWITHSCHEMA#;
                                                            CREATE TABLE  #TABLENAMEWITHSCHEMA#
                                                            (Code int,
                                                             IsPercentage bool,
                                                             Options varchar );";

        const string DropBackUpRouteTableIfExists_Query = @"Drop Table IF EXISTS #TABLENAMEWITHSCHEMA#;";

        const string Createindex_Query = @"ALTER TABLE #TABLENAMEWITHSCHEMA# ADD constraint route_pkey PRIMARY KEY (Code);";

        #endregion
    }
}