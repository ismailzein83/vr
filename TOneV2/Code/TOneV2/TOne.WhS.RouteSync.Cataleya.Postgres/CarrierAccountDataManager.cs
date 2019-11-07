using System;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Postgres
{
   public class CarrierAccountDataManager : BasePostgresDataManager
    {
        public string _connectionString { get; set; }
        public CarrierAccountDataManager(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}