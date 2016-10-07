using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceMarketPriceDataManager : BaseSQLDataManager
    {
        public SourceMarketPriceDataManager(string connectionString)
            : base(connectionString, false)
        {

        }

        public IEnumerable<SourceMarketPrice> GetSourceMarketPrices()
        {
            return GetItemsText(query_getMarketPrices, SourceMarketPrice, null);
        }


        SourceMarketPrice SourceMarketPrice(IDataReader reader)
        {
            return new SourceMarketPrice
            {
                SaleZoneMarketPriceID = (int)reader["SaleZoneMarketPriceID"],
                SaleZoneID = (int)reader["SaleZoneID"],
                ServicesFlag = (short)reader["ServicesFlag"],
                FromRate = GetReaderValue<decimal>(reader, "FromRate"),
                ToRate = GetReaderValue<decimal>(reader, "ToRate")
            };
        }

        const string query_getMarketPrices = @"SELECT  [SaleZoneMarketPriceID]
                                                      ,[SaleZoneID]
                                                      ,[ServicesFlag]
                                                      ,[FromRate]
                                                      ,[ToRate]
                                                  FROM [SaleZoneMarketPrice]";
    }
}
