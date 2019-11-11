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
        public string SchemaName { get; set; }
        public string ConnectionString { get; set; }

        protected override string GetConnectionString()
        {
            return ConnectionString;
        }

        public RouteDataManager(string schemaName, string connectionString)
        {
            SchemaName = schemaName;
            ConnectionString = connectionString;
        }

        #region Public Methods

        public object PrepareDataForApply(List<ConvertedRoute> routes)
        {
            if (routes == null || routes.Count == 0)
                return null;

            var carrierAccountDataManager = new CarrierAccountDataManager(SchemaName, ConnectionString);
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
                base.Bulk(convertedRoutesForApply.Routes, $"{SchemaName}.{convertedRoutesForApply.RouteTableName}");
            }
        }

        public void DropIfExistsCreateRouteTables(List<string> routeTableNames)
        {
            var dropIfExistsCreateTempCustomerIdentificationTableQueries = new List<string>();

            foreach (var routeTableName in routeTableNames)
            {
                var query = DropIfExistsCreateRouteTable_Query.Replace("#TABLENAME#", routeTableName);
                query = query.Replace("#SCHEMA#", SchemaName);
                dropIfExistsCreateTempCustomerIdentificationTableQueries.Add(query);
            }

            ExecuteNonQuery(dropIfExistsCreateTempCustomerIdentificationTableQueries.ToArray());
        }

        public string GetDropBackUpRouteTableIfExistsQuery(string tableName)
        {
            return DropBackUpRouteTableIfExists_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{tableName}");
        }

        public void CreateRouteTablesIndexes(List<string> routeTablesNames)
        {
            var query = new StringBuilder();
            foreach (var routeTableName in routeTablesNames)
            {
                query.AppendLine(Createindex_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{routeTableName}"));
            }

            ExecuteNonQuery(new string[] { query.ToString() });
        }

        #endregion

        #region Queries 
        const string DropIfExistsCreateRouteTable_Query = @"DROP TABLE IF EXISTS #SCHEMA#.#TABLENAME#;
                                                            CREATE TABLE  #SCHEMA#.#TABLENAME#
                                                            (Code int,
                                                             IsPercentage bool,
                                                             Options varchar );";

        const string DropBackUpRouteTableIfExists_Query = @"Drop Table IF EXISTS #TABLENAMEWITHSCHEMA#;";

        const string Createindex_Query = @"ALTER TABLE #TABLENAMEWITHSCHEMA# ADD constraint route_pkey PRIMARY KEY (Code);";

        #endregion
    }
}