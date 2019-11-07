using System;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Postgres
{
    public class CustomerIdentificationDataManager : BasePostgresDataManager
    {
        public string _connectionString { get; set; }
        public CustomerIdentificationDataManager(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}