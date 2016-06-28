using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class CurrencyExchangeRateWithEEDDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICurrencyExchangeRateWithEEDDataManager
    {         
        readonly string[] columns = { "CurrencyID", "Rate", "BED", "EED"};
        public void ApplyExchangeRateWithEESInDB(List<Vanrise.Entities.ExchangeRateWithEED> exchangeRates)
        {
            var streamForBulkInsert = base.InitializeStreamForBulkInsert();
            foreach(var rate in exchangeRates)
            {
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", rate.CurrencyId, rate.Rate, rate.BED, rate.EED);
            }
            streamForBulkInsert.Close();
            var bulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CurrencyExchangeRate_Temp]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns,
            };
            InsertBulkToTable(bulkInsertInfo);
        }

        protected override string GetConnectionString()
        {
            return !String.IsNullOrEmpty(_connectionString) ? _connectionString : Common.Utilities.GetExposedConnectionString(_connectionStringName);
        }

        string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        string _connectionStringName;
        public string ConnectionStringName
        {
            set { _connectionStringName =value; }
        }
    }
}
