using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
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



        #region Queries 
        const string DropIfExistsCreateRouteTable_Query = @"DROP TABLE IF EXISTS #SCHEMA#.#TABLENAME#;
                                                            CREATE TABLE  #SCHEMA#.#TABLENAME#
                                                            (Code int,
                                                             IsPercentage bool,
                                                             Options varchar );";
        #endregion

    }
}