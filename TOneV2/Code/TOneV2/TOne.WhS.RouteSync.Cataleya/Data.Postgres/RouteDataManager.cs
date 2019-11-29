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
                var cataleyaConvertedRoutesForApply = convertedRoutesForApplyByCustomer.GetOrCreateItem(convertedRoute.CarrierID, () =>
                {
                    var carrierAccount = carrierAccountDataManager.GetCarrierAccountMapping(convertedRoute.CarrierID);
                    return new CataleyaConvertedRoutesForApply()
                    {
                        Routes = new List<CataleyaConvertedRoute>(),
                        RouteTableName = carrierAccount.RouteTableName
                    };
                });
                cataleyaConvertedRoutesForApply.Routes.Add(convertedRoute);
            }

            return convertedRoutesForApplyByCustomer;
        }

        public void ApplyCataleyaRoutesToDB(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            var convertedRoutesForApplyByCustomer = context.PreparedItemsForApply as Dictionary<int, CataleyaConvertedRoutesForApply>;
            string tableNamePrefix = string.IsNullOrEmpty(schemaName) ? "" : $"{schemaName}.";

            foreach (var keyValuePair in convertedRoutesForApplyByCustomer)
            {
                var convertedRoutesForApply = keyValuePair.Value;
                base.Bulk(convertedRoutesForApply.Routes, $"{tableNamePrefix}{convertedRoutesForApply.RouteTableName}");
            }
        }

        public void Initialize(IEnumerable<CarrierAccountMapping> carrierAccountsMapping)
        {
            DropIfExistsCreateRouteTables(carrierAccountsMapping.Select(item => item.RouteTableName));
        }

        public string GetDropBackUpRouteTableIfExistsQuery(string tableName)
        {
            var tableNameWithSchema = string.IsNullOrEmpty(schemaName) ? tableName : $"{schemaName}.{tableName}";
            return DropBackUpRouteTableIfExists_Query.Replace("#TABLENAMEWITHSCHEMA#", tableNameWithSchema);
        }

        public Dictionary<string, string> GetCreateRouteTablesIndexesQuery(List<string> routeTablesNames)
        {
            Dictionary<string, string> queries = new Dictionary<string, string>();
            string tableNamePrefix = string.IsNullOrEmpty(schemaName) ? "" : $"{schemaName}.";

            foreach (var routeTableName in routeTablesNames)
            {
                var createIndexQuery = Createindex_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{tableNamePrefix}{routeTableName}");
                createIndexQuery = createIndexQuery.Replace("#GUID#", Guid.NewGuid().ToString("N"));

                queries.Add($"Build index for {routeTableName}", createIndexQuery);
            }

            return queries;
        }

        #endregion

        #region Private Methods

        void DropIfExistsCreateRouteTables(IEnumerable<string> routeTableNames)
        {
            var queries = new List<string>();
            string tableNamePrefix = string.IsNullOrEmpty(schemaName) ? "" : $"{schemaName}.";

            foreach (var routeTableName in routeTableNames)
            {
                var query = DropIfExistsCreateRouteTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{tableNamePrefix}{routeTableName}");
                queries.Add(query);
            }

            ExecuteNonQuery(queries.ToArray());
        }

        #endregion

        #region Queries 
        const string DropIfExistsCreateRouteTable_Query = @"DROP TABLE IF EXISTS #TABLENAMEWITHSCHEMA#;
                                                            CREATE TABLE  #TABLENAMEWITHSCHEMA#
                                                            (Code character varying(30),
                                                             IsPercentage character varying(10),
                                                             Options varchar,
                                                             Statics varchar);";

        const string DropBackUpRouteTableIfExists_Query = @"Drop Table IF EXISTS #TABLENAMEWITHSCHEMA#;";

        const string Createindex_Query = @"ALTER TABLE #TABLENAMEWITHSCHEMA# ADD constraint route_pkey_#GUID# PRIMARY KEY (Code);";

        #endregion
    }
}