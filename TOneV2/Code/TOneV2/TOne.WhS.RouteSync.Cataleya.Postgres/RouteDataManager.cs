using System;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Postgres
{
    public class RouteDataManager : BasePostgresDataManager
    {
        public string _connectionString { get; set; }

        public RouteDataManager(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}